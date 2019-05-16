namespace J.EditorOnly.UsageBrowser
{
	using UnityEditor;
	using UnityEditor.IMGUI.Controls;

	sealed class TreeItem : TreeViewItem
	{
		public readonly string Path;

		public TreeItem(int id, int depth, string guid) : base(id, depth)
		{
			Path = AssetDatabase.GUIDToAssetPath(guid);
			string type = null;
			if (string.IsNullOrEmpty(Path)) Path = null;
			else type = AssetDatabase.GetMainAssetTypeAtPath(Path)?.Name;
			if (string.IsNullOrEmpty(type)) type = "MISSING";
			displayName = $"[{type}] {Path ?? guid}";
		}
	}
}