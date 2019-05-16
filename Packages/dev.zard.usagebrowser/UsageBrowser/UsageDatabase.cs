namespace J.EditorOnly.UsageBrowser
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	[PreferBinarySerialization]
	public sealed class UsageDatabase : ScriptableObject, ISerializationCallbackReceiver
	{
		public bool LogUpdateInfo;
		public bool LogChangedAssets;
		public bool LogAssetWithContext;

		[SerializeField, HideInInspector] List<Entry> entries;
		readonly Dict referDict = new Dict();
		readonly Dict dependDict = new Dict();
		bool dirty;

		internal string EntryInfo => $"dep={dependDict.Count} ref={referDict.Count}";

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

		internal void AddRefer(string path, string id = null)
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

		internal void RemoveRefer(string id)
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

		static readonly IReadOnlyCollection<string> Empty = new string[0];

		static UsageDatabase Instance;

		public static UsageDatabase Load(bool createIfNotExists = false)
		{
			if (Instance == null)
			{
				Instance = AssetDatabase.LoadAssetAtPath<UsageDatabase>(Define.DatabasePath);
				if (Instance == null && createIfNotExists) Create(null);
			}

			return Instance;
		}

		public static void Rebuild() => Create(Load());

		static void Create(UsageDatabase old)
		{
			var db = Instance = CreateInstance<UsageDatabase>();
			if (old)
			{
				db.LogUpdateInfo = old.LogUpdateInfo;
				db.LogChangedAssets = old.LogChangedAssets;
				db.LogAssetWithContext = old.LogAssetWithContext;
			}

			string title = $"[{nameof(UsageBrowser)}] Creating Database";
			var paths = AssetDatabase.GetAllAssetPaths();
			for (int i = 0, count = paths.Length; i < count; i++)
			{
				if (EditorUtilityJ.DisplayIndexProgress(title, i, count, 200, true))
				{
					DestroyImmediate(db);
					return;
				}

				db.AddRefer(paths[i]);
			}

			Save(db);
			Debug.Log($"[{nameof(UsageBrowser)}] Database created. {db.EntryInfo}");
		}

		static void Save(UsageDatabase db)
		{
			string tempPath = AssetDatabase.GenerateUniqueAssetPath("Assets/UsageDatabase.asset");
			AssetDatabase.CreateAsset(db, tempPath);
			string metaPath = Define.DatabasePath + ".meta";
			if (File.Exists(Define.DatabasePath)) File.Delete(Define.DatabasePath);
			if (File.Exists(metaPath)) File.Delete(metaPath);
			File.Move(tempPath, Define.DatabasePath);
			File.Move(tempPath + ".meta", metaPath);
			AssetDatabase.Refresh();
		}
	}
}