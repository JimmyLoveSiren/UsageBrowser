#if UNITY_EDITOR
namespace J.EditorOnly
{
	using System.IO;
	using UnityEngine;

	public abstract class GitIgnore<T> : ScriptableObject where T : GitIgnore<T>
	{
		public static void OnLoad()
		{
			var instance = CreateInstance<T>();
			var script = UnityEditor.MonoScript.FromScriptableObject(instance);
			string path = UnityEditor.AssetDatabase.GetAssetPath(script);
			if (!path.Contains("Assets/")) return;
			path = path.Substring(0, path.LastIndexOf('/')) + "/.gitignore";
			if (File.Exists(path)) return;
			Debug.Log("Create " + path);
			File.WriteAllText(path, instance.Content);
		}

		public abstract string Content { get; }
	}
}
#endif
