using Core.Shared.Foundation;
using Core.Shared.ReactiveExtensions;
using UniRx;
using UnityEngine;

namespace Scenes.Battle.UI
{
	public class WinLooseScreenView : BaseMonoBehaviour
	{
		public struct Ctx
		{
			public CompositeDisposable viewDisposable;
			public IReadOnlyReactiveTrigger<bool> onGameOver;
		}

		[SerializeField] private GameObject _win_panel = null;
		[SerializeField] private GameObject _loose_panel = null;

		private Ctx _ctx;

		public void SetCtx(Ctx ctx)
		{
			_ctx = ctx;
			_win_panel.SetActive(false);
			_loose_panel.SetActive(false);

			_ctx.viewDisposable.Add(_ctx.onGameOver.Subscribe(win =>
			{
				_win_panel.SetActive(win);
				_loose_panel.SetActive(!win);
			}));
		}
	}
}
