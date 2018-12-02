namespace J.UsageBrowser
{
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	partial class UsageDatabase
	{
		class Postprocessor : AssetPostprocessor
		{
			static readonly HashSet<string> Changed = new HashSet<string>();
			static int Version;

			static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
			{
				if (imported.Length == 0 && deleted.Length == 0) return;
				var db = Load();
				if (db == null) return;
				foreach (string path in deleted)
				{
					if (path == DataPath) continue;
					Changed.Add(path);
					db.RemoveRefer(AssetDatabase.AssetPathToGUID(path));
				}
				foreach (string path in imported)
				{
					if (path == DataPath) continue;
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
				var db = Load();
				if (db == null) return;
				if (db.LogUpdate)
					Debug.Log($"[{nameof(UsageBrowser)}] Database updated. changed={Changed.Count} {db.CountInfo}", db);
				if (db.LogChangedFiles)
					foreach (string path in Changed)
						if (db.LogWithContext)
							Debug.Log(path, AssetDatabase.LoadMainAssetAtPath(path));
						else
							Debug.Log(path);
				Changed.Clear();
				EditorUtility.SetDirty(db);
			}
		}
	}
}
