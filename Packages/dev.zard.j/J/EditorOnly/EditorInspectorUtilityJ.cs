namespace J.EditorOnly
{
	using System.Linq;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	public static class EditorInspectorUtilityJ
	{
		static readonly System.Type TypeInspectorWindow = Assembly.GetAssembly(typeof(EditorWindow))
			?.GetType("UnityEditor.InspectorWindow");

		static readonly MethodInfo MethodGetInspectors = TypeInspectorWindow
			?.GetMethod("GetInspectors", BindingFlags.Static | BindingFlags.NonPublic);

		static readonly PropertyInfo PropertyIsLocked = TypeInspectorWindow
			?.GetProperty("isLocked");

		static readonly PropertyInfo PropertyTracker = TypeInspectorWindow
			?.GetProperty("tracker", BindingFlags.Instance | BindingFlags.NonPublic);

		public static bool IsInspector(EditorWindow window) =>
			TypeInspectorWindow != null && window.GetType() == TypeInspectorWindow;

		public static List<EditorWindow> GetInspectors()
		{
			var results = new List<EditorWindow>();
			GetInspectors(results);
			return results;
		}

		public static void GetInspectors(List<EditorWindow> results)
		{
			results.Clear();
			if (MethodGetInspectors?.Invoke(null, null) is IEnumerable enumerable)
				results.AddRange(enumerable.Cast<EditorWindow>());
		}

		public static bool GetIsLocked(EditorWindow window)
		{
			if (PropertyIsLocked != null && IsInspector(window))
				return (bool) PropertyIsLocked.GetValue(window);
			return false;
		}

		public static void SetIsLocked(EditorWindow window, bool value, bool repaint = false)
		{
			if (PropertyIsLocked != null && IsInspector(window))
			{
				PropertyIsLocked.SetValue(window, value);
				if (repaint) window.Repaint();
			}
		}

		public static ActiveEditorTracker GetTracker(EditorWindow window)
		{
			if (PropertyTracker != null && IsInspector(window))
				return (ActiveEditorTracker) PropertyTracker.GetValue(window);
			return null;
		}

		public static Object GetTarget(EditorWindow window) => GetTargets(window).ElementAtOrDefault(0);

		public static List<Object> GetTargets(EditorWindow window)
		{
			var result = new List<Object>();
			GetTargets(window, result);
			return result;
		}

		public static void GetTargets(EditorWindow window, List<Object> results)
		{
			results.Clear();
			var tracker = GetTracker(window);
			var main = tracker?.activeEditors.ElementAtOrDefault(0)?.targets;
			if (main == null) return;
			var sub = tracker.activeEditors.ElementAtOrDefault(1)?.targets;
			for (int i = 0, count = main.Length; i < count; i++)
			{
				var target = main[i];
				if (target is AssetImporter) target = sub?.ElementAtOrDefault(i);
				if (target) results.Add(target);
			}
		}
	}
}