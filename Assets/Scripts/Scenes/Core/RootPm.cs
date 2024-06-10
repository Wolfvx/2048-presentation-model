using System;
using Core.Loaders.ResourceLoaders;
using Core.Loaders.SceneLoaders;
using Core.Shared.Containers;
using Core.Shared.Foundation;
using Core.Shared.Loaders;
using Core.Shared.ReactiveExtensions;

namespace Scenes.Core
{
	public class RootPm : BaseDisposable
	{
		public struct Ctx
		{
			public Func<InitSt> getParams;
		}

		public static bool Exists;
		
		private readonly Ctx _ctx;
		private CorePm _corePm;
		private readonly ReactiveEvent<LoadSt> _restart;

		private IResourceLoader _resourceLoader;
		private IDisposable _resourceLoaderDisposable;
		private ISceneLoaderMaster _sceneLoaderMaster;
		private IDisposable _sceneLoaderDisposable;

		public RootPm(Ctx ctx)
		{
			_ctx = ctx;
			_restart = new ReactiveEvent<LoadSt>();
			Exists = true;
		}

		public void Init()
		{
			FakeBundleLoaderPm resourceLoaderPm = new FakeBundleLoaderPm(new FakeBundleLoaderPm.Ctx
			{
				minLoadDelay = 0,
				maxLoadDelay = 0
			});
			_resourceLoader = resourceLoaderPm;
			_resourceLoaderDisposable = resourceLoaderPm;
			_restart.Notify(new LoadSt());
			AddDispose(_restart.Subscribe(restart));
		}

		protected override void OnDispose()
		{
			_corePm?.Dispose();
			_sceneLoaderDisposable?.Dispose();
			_resourceLoaderDisposable?.Dispose();
			Exists = false;
			base.OnDispose();
		}

		private void restart(LoadSt loadSt)
		{
			_sceneLoaderDisposable?.Dispose();
			InitSt initSt = _ctx.getParams();
			if (initSt.sceneImage == null)
				createRealSceneLoader();
			else
				createFakeSceneLoader();
			_corePm = new CorePm(new CorePm.Ctx
			{
				initSt = initSt,
				loadSt = loadSt,
				resourceLoader =  _resourceLoader,
				sceneLoaderMaster = _sceneLoaderMaster
			});
			_corePm.Init();
		}

		private void createRealSceneLoader()
		{
			SceneLoaderPm sceneLoaderPm = new SceneLoaderPm();
			_sceneLoaderMaster = sceneLoaderPm;
			_sceneLoaderDisposable = sceneLoaderPm;
		}
		
		private void createFakeSceneLoader()
		{
			FakeSceneLoaderPm sceneLoaderPm = new FakeSceneLoaderPm();
			_sceneLoaderMaster = sceneLoaderPm;
			_sceneLoaderDisposable = sceneLoaderPm;
		}
	}
}