using Core.Shared.Containers;
using Core.Shared.Foundation;
using Core.Shared.Loaders;
using Game;

namespace Scenes.Core
{
	public class CorePm : BaseDisposable
	{
		public struct Ctx
		{
			public InitSt initSt;
			public LoadSt loadSt;
			public IResourceLoader resourceLoader;
			public ISceneLoaderMaster sceneLoaderMaster;
		}

		private readonly Ctx _ctx;

		public CorePm(Ctx ctx)
		{
			_ctx = ctx;
		}

		public void Init()
		{
			GamePm sceneGamePm = AddDispose(new GamePm(new GamePm.Ctx
			{
				initSt = _ctx.initSt,
				loadSt = _ctx.loadSt,
				resourceLoader = _ctx.resourceLoader,
				sceneLoaderMaster = _ctx.sceneLoaderMaster,
				getScene = SceneFactory.GetScene
			}));
			sceneGamePm.Init();
		}
	}
}