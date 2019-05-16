namespace J.EditorOnly.UsageBrowser
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	sealed partial class Window : EditorWindow, IHasCustomMenu
	{
		const int MaxHistoryCount = 20;

		static readonly string[] ModeTexts = {"Dependency", "Reference"};

		[SerializeField] List<Query> m_History;
		[SerializeField] int m_Index;
		TreeView m_Tree;

		void OnEnable()
		{
			if (m_History == null) m_History = new List<Query>();
		}

		Query Get(int? index = null) => m_History.ElementAtOrDefault(index ?? m_Index);

		bool CanBack => Get(m_Index - 1) != null;
		bool CanForward => Get(m_Index + 1) != null;

		public void Back()
		{
			if (!CanBack) return;
			--m_Index;
			Refresh();
		}

		public void Forward()
		{
			if (!CanForward) return;
			++m_Index;
			Refresh();
		}

		public void New(string[] source, bool reference)
		{
			if (source?.Length > 0)
			{
				if (Get() != null) ++m_Index;
				if (m_Index < m_History.Count) m_History.RemoveRange(m_Index, m_History.Count - m_Index);
				if (m_History.Count >= MaxHistoryCount)
				{
					m_History.RemoveAt(0);
					--m_Index;
				}

				m_History.Add(new Query(source, reference));
				Refresh();
			}
		}

		public void ClearHistory()
		{
			m_History.Clear();
			m_Index = 0;
			Refresh();
		}

		void Refresh() => m_Tree = null;

		void OnGUI()
		{
			var query = OnNavigation();
			if (query == null) return;
			if (m_Tree == null) m_Tree = new TreeView(query);
			var rect = new Rect(Vector2.zero, position.size);
			rect.yMin += 24;
			m_Tree.OnGUI(rect);
		}

		Query OnNavigation()
		{
			EditorGUILayout.BeginHorizontal();
			bool enabled = GUI.enabled;
			if (enabled) GUI.enabled = CanBack;
			if (GUILayout.Button("<")) Back();
			if (enabled) GUI.enabled = CanForward;
			if (GUILayout.Button(">")) Forward();

			var query = Get();
			if (enabled) GUI.enabled = query != null;
			if (GUILayout.Button("Refresh"))
			{
				UsageDatabase.Load(true);
				Refresh();
			}

			int mode = query != null ? query.Reference ? 1 : 0 : -1;
			if (mode != GUILayout.Toolbar(mode, ModeTexts, EditorStyles.radioButton) && query != null)
			{
				query.Reference = mode == 0;
				Refresh();
			}

			string search = EditorGUILayout.TextField(query?.TreeState.searchString);
			if (m_Tree != null) m_Tree.searchString = search;
			GUI.enabled = enabled;
			EditorGUILayout.EndHorizontal();
			return query;
		}

		void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Clear History"), false, ClearHistory);
		}
	}
}