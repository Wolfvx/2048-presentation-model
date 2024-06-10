using System;
using Core.Shared.Foundation;
using Core.Shared.Loaders;
using UnityEngine;

namespace Core.Loaders.SceneLoaders
{
	public class FakeSceneLoaderPm : SceneLoaderPm, ISceneLoaderMaster
	{
		void ISceneLoaderMaster.ChangeScene(string sceneName, Action onUnload, Action onComplete)
		{
			Log.Info($"ChangeScene {sceneName}");
			onComplete?.Invoke();
		}
	}
}