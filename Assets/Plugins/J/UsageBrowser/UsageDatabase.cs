#if UNITY_EDITOR
namespace J.UsageBrowser
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	[PreferBinarySerialization]
	public sealed partial class UsageDatabase : ScriptableObject, ISerializationCallbackReceiver
	{
		public bool LogUpdateInfo;
		public bool LogChangedAssets;
		public bool LogAssetWithContext;

		[SerializeField, HideInInspector] List<Entry> entries;
		readonly Dict referDict = new Dict();
		readonly Dict dependDict = new Dict();
		bool dirty;

		string EntryInfo => $"dep={dependDict.Count} ref={referDict.Count}";

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (!dirty) return;
			if (entries != null) entries.Clear();
			else entries = new List<Entry>();
			foreach (var pair in dependDict)
				if (pair.Value.Count > 0)
					entries.Add(new Entry(pair.Key, pair.Value.ToList()));
			dirty = false;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			referDict.Clear();
			dependDict.Clear();
			foreach (var entry in entries)
			foreach (string dependId in entry.DependIds)
				AddPair(entry.Id, dependId);
			dirty = false;
		}

		void AddRefer(string path, string id = null)
		{
			if (Path.IsPathRooted(path)) return;
			if (id == null) id = AssetDatabase.AssetPathToGUID(path);
			foreach (string dependPath in AssetDatabase.GetDependencies(path, false))
			{
				if (dependPath == path || Path.IsPathRooted(dependPath)) continue;
				AddPair(id, AssetDatabase.AssetPathToGUID(dependPath));
			}
		}

		void AddPair(string referId, string dependId)
		{
			referDict.GetOrAdd(dependId).Add(referId);
			dependDict.GetOrAdd(referId).Add(dependId);
			dirty = true;
		}

		void RemoveRefer(string id)
		{
			var dependIds = dependDict.GetOrDefault(id);
			if (dependIds == null) return;
			dependDict.Remove(id);
			foreach (string dependId in dependIds)
			{
				var referIds = referDict.GetOrDefault(dependId);
				if (referIds == null) continue;
				referIds.Remove(id);
				if (referIds.Count == 0) referDict.Remove(dependId);
			}

			dirty = true;
		}

		public IReadOnlyCollection<string> GetReferIds(string id) => referDict.GetOrDefault(id) ?? Empty;

		public IReadOnlyCollection<string> GetDependIds(string id) => dependDict.GetOrDefault(id) ?? Empty;

		static readonly IReadOnlyCollection<string> Empty = Array.AsReadOnly(new string[0]);

		static UsageDatabase Instance;

		public static UsageDatabase Load(bool createIfNotExists = false)
		{
			if (Instance == null)
			{
				Instance = AssetDatabase.LoadAssetAtPath<UsageDatabase>(DataPath);
				if (Instance == null && createIfNotExists) RebuildDatabase(null);
			}

			return Instance;
		}

		public static readonly string DataPath = GetDataPath(true);

		static string GetDataPath(bool relative = false)
		{
			string dataPath = Path.ChangeExtension(CallerInfo.FilePath(), "asset");
			if (relative)
			{
				string cwd = Directory.GetCurrentDirectory();
				cwd = cwd.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				cwd += Path.DirectorySeparatorChar;
				dataPath = new Uri(cwd).MakeRelativeUri(new Uri(dataPath)).ToString();
			}

			return dataPath;
		}
	}
}
#endif