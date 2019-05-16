namespace J.EditorOnly
{
	using System;
	using System.IO;
	using UnityEngine;

	public abstract class GitIgnore<T> : ScriptableObject where T : GitIgnore<T>
	{
		public static void OnLoad(string text) => OnLoad(() => text);

		public static void OnLoad(Func<string> text)
		{
			string path = EditorUtilityJ.GetScriptPathOfScriptableObject<T>();
			path = path.Substring(0, path.LastIndexOf('/')) + "/.gitignore";
			if (File.Exists(path)) return;
			Debug.Log("Create " + path);
			File.WriteAllText(path, text?.Invoke());
		}
	}
}