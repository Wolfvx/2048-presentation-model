using Core.Shared.Containers;
using Core.Shared.Foundation;
using Core.Shared.Loaders;
using Core.Shared.ReactiveExtensions;
using Core.Shared.Scenes;

namespace Game
{
	public class GamePm : BaseDisposable
	{
		public delegate ScenePm GetSceneDelegate(ref ScenePm.Ctx sceneCtx);
		
		public struct Ctx
		{
			public InitSt initSt;
			public LoadSt loadSt;
			public IResourceLoader resourceLoader;
			public ISceneLoaderMaster sceneLoaderMaster;
			public GetSceneDelegate getScene;
		}

		private readonly Ctx _ctx;
		private ScenePm _scene;

		public GamePm(Ctx ctx)
		{
			_ctx = ctx;
		}
		
		public void Init()
		{
			ReactiveEvent<SceneImage> changeScene = new ReactiveEvent<SceneImage>();
			GameProfile gameProfile = new GameProfile(new GameProfile.Ctx
			{
				initSt = _ctx.initSt,
				changeScene = changeScene
			});
			AddDispose(changeScene.Subscribe(sceneImage =>
			{
				_ctx.sceneLoaderMaster.ChangeScene(sceneImage.sceneName.ToString(), () =>
				{
					_scene?.Dispose();
				}, () =>
				{
					ScenePm.Ctx sceneCtx = new ScenePm.Ctx
					{
						sceneImage = sceneImage,
						resourceLoader = _ctx.resourceLoader,
						sceneLoader = _ctx.sceneLoaderMaster
					};
					_scene = _ctx.getScene(ref sceneCtx);
					_scene.Init();
				});
			}));
		}

		protected override void OnDispose()
		{
			_scene?.Dispose();
			base.OnDispose();
		}
	}
}