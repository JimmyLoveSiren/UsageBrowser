namespace J.EditorOnly
{
	using System.IO;
	using UnityEngine;

	public abstract class GitIgnore<T> : ScriptableObject where T : GitIgnore<T>
	{
		public static void OnLoad(string contents)
		{
			string path = EditorUtilityJ.GetScriptPath<T>();
			if (!path.Contains("Assets/")) return;
			path = path.Substring(0, path.LastIndexOf('/')) + "/.gitignore";
			if (File.Exists(path)) return;
			Debug.Log("Create " + path);
			File.WriteAllText(path, contents);
		}
	}
}