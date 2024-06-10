using System.Linq;
using Core.Shared.Containers;
using Core.Shared.Foundation;
using Core.Shared.Loaders;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Core.Shared.Scenes
{
	public abstract class ScenePm : BaseDisposable
	{
		public struct Ctx
		{
			public SceneImage sceneImage;
			public IResourceLoader resourceLoader;
			public ISceneLoader sceneLoader;
		}

		public abstract void Init();
	}

	public abstract class ScenePm<TImage, TSceneContext> : ScenePm
		where TImage : SceneImage where TSceneContext : SceneContext
	{
		
		protected readonly Ctx _ctx;
		protected TImage _sceneImage;
		protected TSceneContext _sceneContext;
		
		protected ScenePm(Ctx ctx)
		{
			_ctx = ctx;
		}

		public override void Init()
		{
			_sceneImage = _ctx.sceneImage as TImage;
			if (_sceneImage == null)
				Log.Err($"_ctx.sceneImage is wrong type. Got {_ctx.sceneImage.GetType().Name}");
			if (_sceneContext == null)
				_sceneContext = findContext();
			if (_sceneContext == null)
				Log.Err($"Couldn't find {typeof(TSceneContext).Name} in scene");
		}
		
		private static TSceneContext findContext()
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
			//Debug.Log($"Scene is {SceneManager.GetActiveScene().name} {SceneManager.GetActiveScene().path}");
			TSceneContext[] sceneContexts = Object.FindObjectsOfType<TSceneContext>();
			return sceneContexts.FirstOrDefault();
		}
	}
}