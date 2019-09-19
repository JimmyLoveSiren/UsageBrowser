namespace J.EditorOnly.UsageBrowser
{
	using J.Algorithm;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEditor.IMGUI.Controls;
	using UnityEngine;

	sealed class TreeView : UnityEditor.IMGUI.Controls.TreeView
	{
		readonly Query m_Query;
		readonly List<TreeItem> m_List;

		public TreeView(Query query) : base(query?.TreeState ?? new TreeViewState())
		{
			m_Query = query;
			m_List = new List<TreeItem>();
			SetRowHeight(EditorGUIUtility.singleLineHeight);
			Reload();
			ExpandAll();
		}

		protected override TreeViewItem BuildRoot()
		{
			m_List.Clear();
			var root = new TreeViewItem(-1, -1);
			if (m_Query?.Source?.Length > 0)
			{
				var db = UsageDatabase.Load();
				if (db)
				{
					foreach (var node in Search.BreadthFirst(m_Query.Source,
						db.GetRelationExpander(m_Query.Reference).ToNodeExpander()))
					{
						var item = new TreeItem(node.Index, node.Depth, node.Value);
						m_List.Add(item);
						if (node.Parent == null) root.AddChild(item);
						else m_List[node.Parent.Index].AddChild(item);
					}
				}
				else
				{
					root.AddChild(new TreeViewItem(-2, 0, "Database not found, please refresh."));
					int count = m_Query.Source.Length;
					for (int i = 0; i < count; i++) root.AddChild(new TreeItem(i, 0, m_Query.Source[i]));
				}
			}
			else
			{
				root.AddChild(new TreeViewItem(-2, 0, "No assets selected."));
			}

			return root;
		}

		float m_Padding;

		public void SetRowHeight(float height)
		{
			rowHeight = height;
			extraSpaceBeforeIconAndLabel = rowHeight + 4;
			m_Padding = (rowHeight - 16) / 2;
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			DrawIcon(args);
			args.rowRect.yMin += m_Padding;
			args.rowRect.yMax -= m_Padding;
			base.RowGUI(args);
		}

		void DrawIcon(RowGUIArgs args)
		{
			var item = args.item as TreeItem;
			if (item?.Path == null) return;
			var icon = AssetDatabase.GetCachedIcon(item.Path);
			if (icon == null) return;
			var rect = args.rowRect;
			rect.xMin += GetContentIndent(item);
			rect.width = rect.height;
			GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);
		}

		bool Select(int id)
		{
			var item = m_List.ElementAtOrDefault(id);
			if (item?.Path != null)
			{
				var asset = AssetDatabase.LoadMainAssetAtPath(item.Path);
				if (asset)
				{
					Selection.activeObject = asset;
					return true;
				}
			}

			return false;
		}

		protected override void SingleClickedItem(int id) => Select(id);

		protected override void DoubleClickedItem(int id)
		{
			if (Select(id)) Window.ShowWindow().New(Selection.assetGUIDs, !m_Query.Reference);
		}

		static readonly char[] SearchSeparator = {' '};
		string m_CachedSearch;
		string[] m_CachedWords;

		protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
		{
			if (m_CachedSearch != search)
			{
				m_CachedSearch = search;
				m_CachedWords = search.ToLower().Split(SearchSeparator, StringSplitOptions.RemoveEmptyEntries);
				m_CachedWords = m_CachedWords.Distinct().ToArray();
			}

			return m_CachedWords.All(word => base.DoesItemMatchSearch(item, word));
		}
	}
}