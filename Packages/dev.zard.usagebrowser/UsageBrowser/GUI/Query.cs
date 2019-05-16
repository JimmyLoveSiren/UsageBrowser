namespace J.EditorOnly.UsageBrowser
{
	using System;
	using UnityEditor.IMGUI.Controls;

	[Serializable]
	sealed class Query
	{
		public string[] Source;
		public bool Reference;
		public TreeViewState TreeState;

		Query()
		{
		}

		public Query(string[] source, bool reference, TreeViewState treeState = null)
		{
			Source = source;
			Reference = reference;
			TreeState = treeState ?? new TreeViewState();
		}
	}
}