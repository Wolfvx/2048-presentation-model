using Core.Shared.Containers;
using Core.Shared.Foundation;
using Core.Shared.Loaders;
using Core.Shared.ReactiveExtensions;
using UniRx;
using UnityEngine;

namespace Scenes.Battle.UI
{
	public class ScorePm : BaseDisposable
	{
		public struct Ctx
		{
			public IResourceLoader resourceLoader;
			public Transform uiParent;
			public IReadOnlyReactiveTrigger increaseScore;
		}

		private readonly Ctx _ctx;
		private readonly ResourceImage _uiRes = new ResourceImage()
		{
			prefabName = "Score"
		};

		public ScorePm(Ctx ctx)
		{
			_ctx = ctx;
		}

		public void Init()
		{
			AddDispose(_ctx.resourceLoader.LoadPrefab(_uiRes, prefab =>
			{
				GameObject obj = AddComponent(GameObject.Instantiate(prefab, _ctx.uiParent));
				ScoreView view = obj.GetComponent<ScoreView>();
				loadView(view);
			}));
		}

		private void loadView(ScoreView view)
		{
			CompositeDisposable viewDisposable = AddDispose(new CompositeDisposable());
			view.SetCtx(new ScoreView.Ctx
			{
				viewDisposable = viewDisposable,
				increaseScore = _ctx.increaseScore
			});
		}
	}
}
