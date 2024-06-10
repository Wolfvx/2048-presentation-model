using Core.Shared.Containers;
using Core.Shared.Foundation;
using Core.Shared.Loaders;
using Core.Shared.ReactiveExtensions;
using UnityEngine;
using UniRx;

namespace Scenes.Battle.UI
{
	public class WinLooseScreenPm : BaseDisposable
	{
		public struct Ctx
		{
			public IResourceLoader resourceLoader;
			public Transform uiParent;
			public IReadOnlyReactiveTrigger<bool> onGameOver;
		}

		private readonly Ctx _ctx;
		private readonly ResourceImage _uiRes = new ResourceImage()
		{
			prefabName = "WinLoosePanels"
		};

		public WinLooseScreenPm(Ctx ctx)
		{
			_ctx = ctx;
		}

		public void Init()
		{
			AddDispose(_ctx.resourceLoader.LoadPrefab(_uiRes, prefab =>
			{
				GameObject obj = AddComponent(GameObject.Instantiate(prefab, _ctx.uiParent));
				WinLooseScreenView view = obj.GetComponent<WinLooseScreenView>();
				loadView(view);
			}));
		}

		private void loadView(WinLooseScreenView view)
		{
			CompositeDisposable viewDisposable = AddDispose(new CompositeDisposable());
			view.SetCtx(new WinLooseScreenView.Ctx
			{
				viewDisposable = viewDisposable,
				onGameOver = _ctx.onGameOver
			});
		}
	}
}
