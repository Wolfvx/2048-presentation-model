using System;
using UnityEngine.SceneManagement;

namespace Core.Shared.Loaders
{
	public interface ISceneLoaderMaster : ISceneLoader
	{
		void ChangeScene(string sceneName, Action onUnload, Action onComplete);
	}
}