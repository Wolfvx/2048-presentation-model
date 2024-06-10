using System;
using Core.Shared.Foundation;
using Core.Shared.ReactiveExtensions;
using Scenes.Battle.Logic.Projectiles;
using Scenes.Battle.Logic.Input;
using UniRx;
using UnityEngine;

namespace Scenes.Battle.Logic.Player
{
	public class PlayerPm : BaseDisposable
	{
		public struct Ctx
		{
			public Transform spawnLocation;
			public float launchStrength;
			public float maxSideDisplacement;
			public ReactiveEvent<Action<ILaunchableProjectile>, Vector3, bool> getNextProjectile;
			public IReadOnlyReactiveProperty<bool> runGame;
			public IInputModel inputModel;
			public ReactiveTrigger increaseScore;
		}

		private Vector3 _launchPosition = Vector3.zero;
		private ILaunchableProjectile _current_projectile = null;
		private CompositeDisposable _gameDisposables;

		private readonly Ctx _ctx;

		private const float MAX_SIDE_DISPLACEMENT = 2.4f;

		public PlayerPm(Ctx ctx)
		{
			_ctx = ctx;
		}

		public void Init()
		{
			_launchPosition = _ctx.spawnLocation.position;
			_ctx.spawnLocation.gameObject.SetActive(false);

			AddDispose(_ctx.inputModel.onLaunchProjectile.Subscribe(onLaunch));
			AddDispose(_ctx.inputModel.onMoveProjectile.Subscribe(onMoveProjectile));
			AddDispose(_ctx.runGame.Subscribe(run =>
			{
				if (run)
				{
					_gameDisposables = new CompositeDisposable();
					_gameDisposables.Add(_ctx.inputModel.onLaunchProjectile.Subscribe(onLaunch));
					_gameDisposables.Add(_ctx.inputModel.onMoveProjectile.Subscribe(onMoveProjectile));
					if (_current_projectile == null)
					{
						_ctx.getNextProjectile.Notify(projectile => _current_projectile = projectile, _launchPosition, true);
					}
				}
				else
				{
					_gameDisposables?.Dispose();
				}
			}));
		}

		private void onLaunch()
		{
			if (_current_projectile == null) return;
			_current_projectile.Launch(_ctx.launchStrength);
			_current_projectile = null;
			_launchPosition = _ctx.spawnLocation.position;
			_ctx.getNextProjectile.Notify(projectile => _current_projectile = projectile, _launchPosition, false);
			_ctx.increaseScore.Notify();
		}

		private void onMoveProjectile(float direction)
		{
			if (_current_projectile == null) return;
			_launchPosition.x += direction;
			_launchPosition.x = Mathf.Clamp(_launchPosition.x, -MAX_SIDE_DISPLACEMENT, MAX_SIDE_DISPLACEMENT);
			_current_projectile.Position = _launchPosition;
		}
	}
}
