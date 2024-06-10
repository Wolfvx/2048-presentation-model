using Core.Shared.Scenes;
using Core.Shared.Timers;
using Core.Shared.ReactiveExtensions;
using Scenes.Battle.Logic;
using Scenes.Battle.Logic.Input;
using Scenes.Battle.Shared.Containers;
using Scenes.Battle.UI;
using UniRx;

namespace Scenes.Battle
{
	public class BattleScenePm : ScenePm<BattleSceneImage, BattleSceneContext>
	{

		private const int TICK_RATE = 60;

		private ReactiveTrigger _increaseScore;
		private ReactiveTrigger<bool> _onGameOver;

		public BattleScenePm(Ctx ctx) : base(ctx)
		{
			_increaseScore = new ReactiveTrigger();
			_onGameOver = new ReactiveTrigger<bool>();
		}

		public override void Init()
		{
			base.Init();
			TickStreamPm tickStreamPm = new TickStreamPm(new TickStreamPm.Ctx
			{
				tps = TICK_RATE
			});
			AddDispose(tickStreamPm);

			ReactiveProperty<bool> runGame = new ReactiveProperty<bool>(false);
			InputModel inputModel = new InputModel();

			CreateInput(inputModel, tickStreamPm);
			CreateUI();

			BattlePm battlePm = new BattlePm(new BattlePm.Ctx
			{
				sceneImage = _sceneImage,
				sceneContext = _sceneContext,
				resourceLoader = _ctx.resourceLoader,
				tickStream = tickStreamPm,
				runGame = runGame,
				increaseScore = _increaseScore,
				onGameOver = _onGameOver,
				inputModel = inputModel
			});
			AddDispose(battlePm);
			battlePm.Init();

			runGame.SetValueAndForceNotify(true);
		}

		protected override void OnDispose()
		{
			Log.Info("Disposed");
			base.OnDispose();
		}

		private void CreateUI()
		{
			ScorePm scorePm = new ScorePm(new ScorePm.Ctx
			{
				increaseScore = _increaseScore,
				resourceLoader = _ctx.resourceLoader,
				uiParent = _sceneContext.MainUI
			});
			AddDispose(scorePm);
			scorePm.Init();

			WinLooseScreenPm winLooseScreenPm = new WinLooseScreenPm(new WinLooseScreenPm.Ctx
			{
				resourceLoader = _ctx.resourceLoader,
				uiParent = _sceneContext.OverlayUI,
				onGameOver = _onGameOver
			});
			AddDispose(winLooseScreenPm);
			winLooseScreenPm.Init();
		}

		private void CreateInput(InputModel inputModel, ITickStream tickStream)
		{
			KeyboardInputPm keyboardInputPm = new KeyboardInputPm(new KeyboardInputPm.Ctx
			{
				inputModel = inputModel,
				tickStream = tickStream
			});
			AddDispose(keyboardInputPm);
			keyboardInputPm.Init();

			UIInputPm uiInputPm = new UIInputPm(new UIInputPm.Ctx
			{
				inputModel = inputModel,
				resourceLoader = _ctx.resourceLoader,
				uiParent = _sceneContext.MainUI
			});
			AddDispose(uiInputPm);
			uiInputPm.Init();
		}

	}
}