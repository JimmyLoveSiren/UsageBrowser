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
	}
}
