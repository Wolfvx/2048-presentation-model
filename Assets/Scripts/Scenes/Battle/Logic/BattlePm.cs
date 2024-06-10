using System;
using Core.Shared.Foundation;
using Core.Shared.Loaders;
using Core.Shared.Timers;
using Core.Shared.ReactiveExtensions;
using Scenes.Battle.Shared.Containers;
using Scenes.Battle.Logic.Player;
using Scenes.Battle.Logic.Projectiles;
using Scenes.Battle.Logic.Input;
using UniRx;
using UnityEngine;

namespace Scenes.Battle.Logic
{
	public class BattlePm : BaseDisposable
	{
		public struct Ctx
		{
			public BattleSceneImage sceneImage;
			public BattleSceneContext sceneContext;
			public IResourceLoader resourceLoader;
			public ITickStream tickStream;
			public ReactiveProperty<bool> runGame;
			public ReactiveTrigger<bool> onGameOver;
			public IInputModel inputModel;
			public ReactiveTrigger increaseScore;
		}

		private readonly Ctx _ctx;

		public BattlePm(Ctx ctx)
		{
			_ctx = ctx;
		}

		public void Init()
		{
			ReactiveEvent<Action<ILaunchableProjectile>, Vector3, bool> getNextProjectile = new ReactiveEvent<Action<ILaunchableProjectile>, Vector3, bool>();
			ReactiveProperty<int> cubeValueNotify = new ReactiveProperty<int>(0);
			ReactiveTrigger onLoose = new ReactiveTrigger();

			GameStateTrackerPm gameStateTrackerPm = new GameStateTrackerPm(new GameStateTrackerPm.Ctx
			{
				cubeValueNotify = cubeValueNotify,
				onLoose = onLoose,
				runGame = _ctx.runGame,
				onGameOver = _ctx.onGameOver,
				winValue = 2048
			});
			AddDispose(gameStateTrackerPm);
			gameStateTrackerPm.Init();

			FieldManagerPm fieldManagerPm = new FieldManagerPm(new FieldManagerPm.Ctx()
			{
				getNextProjectile = getNextProjectile,
				resourceLoader = _ctx.resourceLoader,
				sceneContext = _ctx.sceneContext,
				cubeValueNotify = cubeValueNotify,
				runGame = _ctx.runGame,
				onLoose = onLoose,
				tickStream = _ctx.tickStream
			});
			AddDispose(fieldManagerPm);
			fieldManagerPm.Init();

			PlayerPm playerPm = new PlayerPm(new PlayerPm.Ctx
			{
				spawnLocation = _ctx.sceneContext.PlayerSpawnLocation,
				launchStrength = 1000f,
				maxSideDisplacement = 2.4f,
				getNextProjectile = getNextProjectile,
				runGame = _ctx.runGame,
				inputModel = _ctx.inputModel,
				increaseScore = _ctx.increaseScore
			});
			AddDispose(playerPm);
			playerPm.Init();
		}
	}
}