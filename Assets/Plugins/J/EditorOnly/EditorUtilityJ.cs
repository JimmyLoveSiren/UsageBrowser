namespace J.EditorOnly
{
	using UnityEditor;
	using UnityEngine;

	public static class EditorUtilityJ
	{
		public static string GetScriptPath<T>() where T : ScriptableObject
		{
			var instance = ScriptableObject.CreateInstance<T>();
			var script = MonoScript.FromScriptableObject(instance);
			return AssetDatabase.GetAssetPath(script);
		}

		public static string GetMonoBehaviourPath<T>() where T : MonoBehaviour
		{
			var go = new GameObject();
			go.SetActive(false);
			var script = MonoScript.FromMonoBehaviour(go.AddComponent<T>());
			Object.DestroyImmediate(go);
			return AssetDatabase.GetAssetPath(script);
		}

		public static bool DisplayIndexProgress(string title, int index, int count,
			int step = 1, bool cancelable = false)
		{
			if (index >= count - 1)
			{
				EditorUtility.ClearProgressBar();
				return false;
			}

			if (index % step == 0)
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