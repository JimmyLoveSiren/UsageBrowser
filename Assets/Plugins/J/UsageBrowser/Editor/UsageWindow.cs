namespace J.UsageBrowser
{
	using System;
	using Algorithm;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEditor.IMGUI.Controls;
	using UnityEngine;

	public class UsageWindow : EditorWindow
	{
		public static void Show(IEnumerable<SearchNode<string>> search)
		{
			GetWindow<UsageWindow>(nameof(UsageDatabase)).Init(search);
		}

		[SerializeField] TreeViewState state;
		[SerializeField] List<UsageTreeEntry> entries;
		[SerializeField] float rowHeight;
		UsageTree tree;

		void Init(IEnumerable<SearchNode<string>> search)
		{
			state = null;
			entries = new List<UsageTreeEntry>(search.Select(node => new UsageTreeEntry
			{
				AssetId = node.Value,
				Parent = node.Parent?.Index ?? -1,
				Depth = node.Depth
			}));
			tree = null;
		}

		void OnGUI()
		{
			if (state == null) state = new TreeViewState();
			if (entries == null) entries = new List<UsageTreeEntry>();
			if (tree == null) tree = new UsageTree(state, entries, rowHeight);
			EditorGUILayout.BeginHorizontal();
			tree.searchString = EditorGUILayout.TextField(tree.searchString);
			EditorGUI.BeginChangeCheck();
			rowHeight = GUILayout.HorizontalSlider(rowHeight, 16, 80, GUILayout.Width(64));
			if (EditorGUI.EndChangeCheck()) tree.SetRowHeight(rowHeight);
			EditorGUILayout.EndHorizontal();
			tree.OnGUI(new Rect(0, 20, position.width, position.height));
		}
	}

	[Serializable]
	public struct UsageTreeEntry
	{
		public string AssetId;
		public int Parent;
		public int Depth;
	}

	public class UsageTree : TreeView
	{
		readonly TreeViewItem root;
		readonly UsageTreeItem[] items;

		public UsageTree(TreeViewState state, IReadOnlyList<UsageTreeEntry> entries, float rowHeight) : base(state)
		{
			root = new TreeViewItem(-1, -1);
			items = new UsageTreeItem[entries.Count];
			for (var i = 0; i < entries.Count; i++)
			{
				var entry = entries[i];
				var item = items[i] = new UsageTreeItem(i, entry.Depth, entry.AssetId);
				if (entry.Parent < 0) root.AddChild(item);
				else items[entry.Parent].AddChild(item);
			}
			SetRowHeight(rowHeight);
			Reload();
			ExpandAll();
		}

		float padding;
		public void SetRowHeight(float height)
		{
			rowHeight = height;
			extraSpaceBeforeIconAndLabel = rowHeight + 4;
			padding = (rowHeight - 16) / 2;
		}

		protected override TreeViewItem BuildRoot() => root;

		protected override void RowGUI(RowGUIArgs args)
		{
			DrawIcon(args);
			args.rowRect.yMin += padding;
			args.rowRect.yMax -= padding;
			base.RowGUI(args);
		}

		void DrawIcon(RowGUIArgs args)
		{
			var item = (UsageTreeItem)args.item;
			if (item?.Path == null) return;
			var icon = AssetDatabase.GetCachedIcon(item.Path);
			if (icon == null) return;
			var rect = args.rowRect;
			rect.xMin += GetContentIndent(item);
			rect.width = rect.height;
			GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);
		}

		protected override void SingleClickedItem(int id)
		{
			var item = items.ElementAtOrDefault(id);
			if (item?.Path == null) return;
			var asset = AssetDatabase.LoadMainAssetAtPath(item.Path);
			if (asset) Selection.activeObject = asset;
		}

		static readonly char[] SearchSeparator = { ' ' };
		string cachedSearch;
		string[] cachedWords;
		protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
		{
			if (cachedSearch != search)
			{
				cachedSearch = search;
				cachedWords = search.ToLower().Split(SearchSeparator, StringSplitOptions.RemoveEmptyEntries);
				cachedWords = cachedWords.Distinct().ToArray();
			}
			return cachedWords.All(word => base.DoesItemMatchSearch(item, word));
		}
	}

	sealed class UsageTreeItem : TreeViewItem
	{
		public readonly string Path;
		public readonly string Type;

		public UsageTreeItem(int id, int depth, string assetId) : base(id, depth)
		{
			Path = AssetDatabase.GUIDToAssetPath(assetId);
			if (string.IsNullOrEmpty(Path)) Path = null;
			else Type = AssetDatabase.GetMainAssetTypeAtPath(Path)?.Name;
			displayName = $"[{Type}] {Path ?? assetId}";
		}
	}
}
