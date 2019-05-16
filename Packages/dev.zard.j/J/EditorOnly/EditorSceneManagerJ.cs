namespace J.EditorOnly
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEngine.SceneManagement;

	public static class EditorSceneManagerJ
	{
		public static SceneOpenState BackupSceneOpenState()
		{
			var state = new SceneOpenState();
			var active = SceneManager.GetActiveScene();
			int count = SceneManager.sceneCount;
			for (int i = 0; i < count; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				string path = scene.path;
				if (string.IsNullOrEmpty(path)) continue;
				var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
				state.opened.Add(asset);
				state.loaded.Add(scene.isLoaded);
				if (active == scene) state.active = asset;
			}

			return state;
		}

		public static void RestoreSceneOpenState(SceneOpenState state)
		{
			var empty = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			Scene active = default;
			int count = state.opened.Count;
			for (int i = 0; i < count; i++)
			{
				var asset = state.opened[i];
				if (asset == null) continue;
				var scene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(asset),
					state.loaded[i] ? OpenSceneMode.Additive : OpenSceneMode.AdditiveWithoutLoading);
				if (state.active == asset) active = scene;
			}

			if (active.IsValid())
			{
				SceneManager.SetActiveScene(active);
				EditorSceneManager.CloseScene(empty, true);
			}
			else EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
		}
	}

	[Serializable]
	public class SceneOpenState
	{
		public SceneAsset active;
		public List<SceneAsset> opened = new List<SceneAsset>();
		public List<bool> loaded = new List<bool>();
	}
}