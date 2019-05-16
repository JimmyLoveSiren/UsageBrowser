namespace J.EditorOnly.UsageBrowser
{
	using UnityEditor;

	partial class Window
	{
		const string Name = "Usage Browser";
		const string ContextRoot = "Assets/" + Name + "/";
		const string WindowRoot = "Window/" + Name + "/";
		const string LogUpdateInfo = WindowRoot + "Log Update Info";
		const string LogChangedAssets = WindowRoot + "Log Changed Assets";
		const string LogAssetWithContext = WindowRoot + "Log Asset with Context";

		[MenuItem(ContextRoot + "Find References")]
		public static void FindReferences()
		{
			if (UsageDatabase.Load(true)) ShowWindow().New(Selection.assetGUIDs, true);
		}

		[MenuItem(ContextRoot + "Find Dependencies")]
		public static void FindDependencies()
		{
			if (UsageDatabase.Load(true)) ShowWindow().New(Selection.assetGUIDs, false);
		}

		[MenuItem(WindowRoot + "Show Window")]
		public static Window ShowWindow() => GetWindow<Window>(Name);

		[MenuItem(LogUpdateInfo)]
		public static void ToggleLogUpdateInfo()
		{
			var db = UsageDatabase.Load();
			if (db == null) return;
			db.LogUpdateInfo = !db.LogUpdateInfo;
			EditorUtility.SetDirty(db);
		}

		[MenuItem(LogChangedAssets)]
		public static void ToggleLogChangedAssets()
		{
			var db = UsageDatabase.Load();
			if (db == null) return;
			db.LogChangedAssets = !db.LogChangedAssets;
			EditorUtility.SetDirty(db);
		}

		[MenuItem(LogAssetWithContext)]
		public static void ToggleLogAssetWithContext()
		{
			var db = UsageDatabase.Load();
			if (db == null) return;
			db.LogAssetWithContext = !db.LogAssetWithContext;
			EditorUtility.SetDirty(db);
		}

		[MenuItem(LogUpdateInfo, true)]
		[MenuItem(LogChangedAssets, true)]
		static bool Valid()
		{
			var db = UsageDatabase.Load();
			bool valid = db;
			Menu.SetChecked(LogUpdateInfo, valid && db.LogUpdateInfo);
			Menu.SetChecked(LogChangedAssets, valid && db.LogChangedAssets);
			Menu.SetChecked(LogAssetWithContext, valid && db.LogAssetWithContext);
			return valid;
		}

		[MenuItem(LogAssetWithContext, true)]
		static bool ValidLogAssetWithContext() => Menu.GetChecked(LogChangedAssets);

		[MenuItem(WindowRoot + "Rebuild Database")]
		public static void RebuildDatabase() => UsageDatabase.Rebuild();
	}
}