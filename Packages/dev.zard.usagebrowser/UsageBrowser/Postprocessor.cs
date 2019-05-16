namespace J.EditorOnly.UsageBrowser
{
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	sealed class Postprocessor : AssetPostprocessor
	{
		static readonly HashSet<string> Changed = new HashSet<string>();
		static int Version;

		static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
		{
			if (imported.Length == 0 && deleted.Length == 0) return;
			var db = UsageDatabase.Load();
			if (db == null) return;
			foreach (string path in deleted)
			{
				if (path == Define.DatabasePath) continue;
				Changed.Add(path);
				db.RemoveRefer(AssetDatabase.AssetPathToGUID(path));
			}

			foreach (string path in imported)
			{
				if (path == Define.DatabasePath) continue;
				Changed.Add(path);
				string id = AssetDatabase.AssetPathToGUID(path);
				db.RemoveRefer(id);
				db.AddRefer(path, id);
			}

			if (Changed.Count > 0)
			{
				int version = ++Version;
				EditorApplication.delayCall += () =>
				{
					if (version == Version) Save();
				};
			}
		}

		static void Save()
		{
			var db = UsageDatabase.Load();
			if (db == null) return;
			if (db.LogUpdateInfo)
				Debug.Log($"[{nameof(UsageBrowser)}] Database updated. changed={Changed.Count} {db.EntryInfo}", db);
			if (db.LogChangedAssets)
				if (db.LogAssetWithContext)
					foreach (string path in Changed)
						Debug.Log(path, AssetDatabase.LoadMainAssetAtPath(path));
				else
					foreach (string path in Changed)
						Debug.Log(path);
			Changed.Clear();
			EditorUtility.SetDirty(db);
		}
	}
}