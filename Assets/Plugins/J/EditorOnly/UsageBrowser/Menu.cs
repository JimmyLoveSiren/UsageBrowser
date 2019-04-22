#if UNITY_EDITOR
namespace J.EditorOnly.UsageBrowser
{
	using J.Algorithm;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	partial class UsageDatabase
	{
		const string MenuRoot = "Assets/Usage Browser/";

		[MenuItem(MenuRoot + "Find References")]
		public static void FindReferences() => FindReferences(Selection.assetGUIDs);

		public static void FindReferences(IEnumerable<string> assetGUIDs)
		{
			var db = Load(true);
			if (db) UsageWindow.Show(Search.BreadthFirst(assetGUIDs, db.GetReferIds));
		}

		[MenuItem(MenuRoot + "Find Dependencies")]
		public static void FindDependencies() => FindDependencies(Selection.assetGUIDs);

		public static void FindDependencies(IEnumerable<string> assetGUIDs)
		{
			var db = Load(true);
			if (db) UsageWindow.Show(Search.BreadthFirst(assetGUIDs, db.GetDependIds));
		}

		[MenuItem(MenuRoot + "Rebuild Database")]
		public static void RebuildDatabase() => RebuildDatabase(Load());

		static void RebuildDatabase(UsageDatabase db)
		{
			Instance = CreateInstance<UsageDatabase>();
			if (db)
			{
				Instance.LogUpdateInfo = db.LogUpdateInfo;
				Instance.LogChangedAssets = db.LogChangedAssets;
				Instance.LogAssetWithContext = db.LogAssetWithContext;
			}

			db = Instance;
			var paths = AssetDatabase.GetAllAssetPaths();
			string title = $"[{nameof(UsageBrowser)}] Creating Database";
			for (int i = 0, iCount = paths.Length; i < iCount; i++)
			{
				if (ShowProgress(title, i, iCount, true))
				{
					DestroyImmediate(db);
					return;
				}

				db.AddRefer(paths[i]);
			}

			AssetDatabase.CreateAsset(db, DataPath);
			Debug.Log($"[{nameof(UsageBrowser)}] Database created. {db.EntryInfo}", db);
		}

		static bool ShowProgress(string title, int index, int count, bool cancelable = false)
		{
			if (index + 1 >= count)
			{
				EditorUtility.ClearProgressBar();
				return false;
			}

			if ((index & 63) == 0)
			{
				string info = $"{index}/{count}";
				float progress = (float) index / count;
				if (cancelable)
				{
					if (EditorUtility.DisplayCancelableProgressBar(title, info, progress))
					{
						EditorUtility.ClearProgressBar();
						return true;
					}
				}
				else
				{
					EditorUtility.DisplayProgressBar(title, info, progress);
				}
			}

			return false;
		}
	}
}
#endif