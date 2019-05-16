namespace J.EditorOnly.UsageBrowser
{
	using UnityEditor;

	static class Menu
	{
		const string Name = "Usage Browser";
		const string ContextRoot = "Assets/" + Name + "/";
		const string WindowRoot = "Window/" + Name + "/";

		[MenuItem(ContextRoot + "Find References")]
		public static void FindReferences()
		{
			if (UsageDatabase.Load(true)) Show().New(Selection.assetGUIDs, true);
		}

		[MenuItem(ContextRoot + "Find Dependencies")]
		public static void FindDependencies()
		{
			if (UsageDatabase.Load(true)) Show().New(Selection.assetGUIDs, false);
		}

		[MenuItem(WindowRoot + "Show Window")]
		public static Window Show() => EditorWindow.GetWindow<Window>(Name);

		[MenuItem(WindowRoot + "Rebuild Database")]
		public static void RebuildDatabase() => UsageDatabase.Rebuild();
	}
}