using System;
using System.Collections;
using System.Collections.Generic;
using Core.Shared.Foundation;
using Core.Shared.Loaders;
using Core.Shared.Timers;
using Core.Shared.ReactiveExtensions;
using Scenes.Battle.Logic.Projectiles;
using UniRx;
using UnityEngine;

namespace Scenes.Battle.Logic
{
	public class FieldManagerPm : BaseDisposable
	{
		public struct Ctx
		{
			public IReadOnlyReactiveEvent<Action<ILaunchableProjectile>, Vector3, bool> getNextProjectile;
			public IResourceLoader resourceLoader;
			public BattleSceneContext sceneContext;
			public ReactiveProperty<int> cubeValueNotify;
			public IReadOnlyReactiveProperty<bool> runGame;
			public ReactiveTrigger onLoose;
			public ITickStream tickStream;
		}

		private ReactiveEvent<Action<ICubeProjectile>, Vector3> _createCubeProjectile;
		private List<ICubeProjectile> _cubeProjectiles;
		private IDisposable _coroutine;
		private CompositeDisposable _gameDisposables;

		private readonly Ctx _ctx;
		private readonly Vector3 _playerSpawnLocation;

		private const int MIN_CUBE_VALUE = 2; 
		private const float NEXT_PROJECTILE_TIME = 1f;
		private const float CONDITION_CHECK_WAIT_TIME = 0.2f;
		private const int MAX_CUBE_PROJECTILES = 30;
		private const float CLOSEST_DISTANCE_TO_PLAYER = 1f;

		public FieldManagerPm(Ctx ctx)
		{
			_ctx = ctx;
			_cubeProjectiles = new List<ICubeProjectile>(MAX_CUBE_PROJECTILES);
			_createCubeProjectile = new ReactiveEvent<Action<ICubeProjectile>, Vector3>();
			_playerSpawnLocation = _ctx.sceneContext.PlayerSpawnLocation.position;
		}

		public void Init()
		{

			CubeProjectileFactoryPm projectileFactory = new CubeProjectileFactoryPm(new CubeProjectileFactoryPm.Ctx
			{
				resourceLoader = _ctx.resourceLoader,
				projectileParent = _ctx.sceneContext.ProjectileParent,
				createCubeProjectile = _createCubeProjectile,
				trackProjectile = trackProjectile,
				untrackProjectile = untrackProjectile,
				cubeValueNotify = _ctx.cubeValueNotify,
				maxProjectiles = MAX_CUBE_PROJECTILES
			});
			AddDispose(projectileFactory);
			projectileFactory.Init();

			ReactiveTrigger populateField = new ReactiveTrigger();
			BasicPopulateFieldPm basicPopulateFieldPm = new BasicPopulateFieldPm(new BasicPopulateFieldPm.Ctx
			{
				createCubeProjectile = _createCubeProjectile,
				populateField = populateField,
				sceneContext = _ctx.sceneContext,
				trackProjectile = trackProjectile
			});
			AddDispose(basicPopulateFieldPm);
			basicPopulateFieldPm.Init();

			populateField.Notify();

			AddDispose(_ctx.runGame.Subscribe(run =>
			{
				if (run)
				{
					_gameDisposables = new CompositeDisposable();
					_gameDisposables.Add(_ctx.getNextProjectile.SubscribeWithSkip(getNextProjectile));
					_gameDisposables.Add(_ctx.tickStream.Subscribe(ITickStream.StreamType.UPDATE, Update));
				} else
				{
					_gameDisposables?.Dispose();
				}
			}));
		}

		protected override void OnDispose()
		{
			_gameDisposables?.Dispose();
			_coroutine?.Dispose();
			base.OnDispose();
		}

		private void trackProjectile(ICubeProjectile projectile)
		{
			if (_cubeProjectiles.Contains(projectile)) return; // Make sure only 1 instance per projectile gets registered
			_cubeProjectiles.Add(projectile);
		}

		private void untrackProjectile(ICubeProjectile projectile)
		{
			if (!_cubeProjectiles.Contains(projectile)) return;
			_cubeProjectiles.Remove(projectile);
		}

		private void getNextProjectile(Action<ILaunchableProjectile> callback, Vector3 position, bool create_now)
		{
			if (create_now)
			{
				createProjectile(callback, position);
			} else
			{
				_coroutine?.Dispose();
				_coroutine = spawnNextProjectile(callback, position).ToObservable().Subscribe();
			}
		}

		private IEnumerator spawnNextProjectile(Action<ILaunchableProjectile> callback, Vector3 spawn_position)
		{
			yield return new WaitForSeconds(NEXT_PROJECTILE_TIME);
			if (_cubeProjectiles.Count >= MAX_CUBE_PROJECTILES)
			{
				int wait_count = 1000;
				while (true)
				{
					if (wait_count-- < 0)
					{
						Debug.LogError("WAITING TOO LONG!!");
						break;
					}
					yield return new WaitForSeconds(CONDITION_CHECK_WAIT_TIME);
					if (_cubeProjectiles.Count >= MAX_CUBE_PROJECTILES)
					{
						break;
					}
					if (!areThereMovingProjectiles())
					{
						_ctx.onLoose.Notify();
						yield break;
					}
				}
			}
			createProjectile(callback, spawn_position);
			_coroutine?.Dispose();
		}

		private void createProjectile(Action<ILaunchableProjectile> callback, Vector3 spawn_position)
		{
			_createCubeProjectile.Notify(projectile =>
			{
				if (projectile == null)
				{
					throw new NullReferenceException();
				}
				projectile.CubeValue = getNextBestCubeValue();
				callback.Invoke(projectile);
			}, spawn_position);
		}

		private int getNextBestCubeValue()
		{
			int result = getClosesCubeValue();
			if (result == -1) result = MIN_CUBE_VALUE;
			return result;
		}

		private int getClosesCubeValue()
		{
			float distance = 10000f;
			int result = -1;
			for (int i = 0; i < _cubeProjectiles.Count; i++)
			{
				var cube = _cubeProjectiles[i];
				if (cube == null) continue;
				float next_dist = Mathf.Abs(cube.Position.z - _playerSpawnLocation.z);
				if (next_dist < distance)
				{
					distance = next_dist;
					result = cube.CubeValue;
				}
			}
			return result;
		}

		private bool checkIfAnyCubeGotTooClose()
		{
			for (int i = 0; i < _cubeProjectiles.Count; i++)
			{
				var cube = _cubeProjectiles[i];
				if (cube == null) continue;
				float next_dist = Mathf.Abs(cube.Position.z - _playerSpawnLocation.z);
				if (next_dist < CLOSEST_DISTANCE_TO_PLAYER)
				{
					return true;
				}
			}
			return false;
		}

		private bool areThereMovingProjectiles()
		{
			for (int i = 0; i < _cubeProjectiles.Count; i++)
			{
				if (_cubeProjectiles[i].isMoving) return true;
			}
			return false;
		}

		private void Update(float delta)
		{
			if (checkIfAnyCubeGotTooClose())
			{
				_ctx.onLoose.Notify();
			}
		}
	}
}
