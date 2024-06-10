using Core.Shared.Foundation;
using Core.Shared.ReactiveExtensions;
using UniRx;

namespace Scenes.Battle.Logic
{
	public class GameStateTrackerPm : BaseDisposable
	{
		public struct Ctx
		{
			public IReadOnlyReactiveProperty<int> cubeValueNotify;
			public IReadOnlyReactiveTrigger onLoose;
			public ReactiveProperty<bool> runGame;
			public ReactiveTrigger<bool> onGameOver;
			public int winValue;
		}

		private CompositeDisposable _disposables;

		private readonly Ctx _ctx;

		public GameStateTrackerPm(Ctx ctx)
		{
			_ctx = ctx;
		}

		public void Init()
		{
			AddDispose(_ctx.runGame.Subscribe(run =>
			{
				if (run)
				{
					_disposables = new CompositeDisposable();
					_disposables.Add(_ctx.cubeValueNotify.Subscribe(value =>
					{
						if (value >= _ctx.winValue)
						{
							gameWIn();
						}
					}));
					_disposables.Add(_ctx.onLoose.Subscribe(() =>
					{
						gameLoose();
					}));
				} else
				{
					_disposables?.Dispose();
				}
			}));
		}

		private void gameWIn()
		{
			_ctx.runGame.SetValueAndForceNotify(false);
			_ctx.onGameOver.Notify(true);
		}

		private void gameLoose()
		{
			_ctx.runGame.SetValueAndForceNotify(false);
			_ctx.onGameOver.Notify(false);
		}
	}
}
