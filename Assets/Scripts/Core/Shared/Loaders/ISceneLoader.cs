using System;
using UnityEngine.SceneManagement;

namespace Core.Shared.Loaders
{
	public interface ISceneLoader
	{
		void LoadScene(string sceneName, Action<Scene> onComplete);
		void UnloadScene(Scene scene);
	}
}