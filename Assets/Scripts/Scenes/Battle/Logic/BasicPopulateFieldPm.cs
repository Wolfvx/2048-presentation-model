using System;
using Core.Shared.Foundation;
using Core.Shared.ReactiveExtensions;
using Scenes.Battle.Logic.Projectiles;
using UnityEngine;

namespace Scenes.Battle.Logic
{
	public class BasicPopulateFieldPm : BaseDisposable
	{
		public struct Ctx
		{
			public IReadOnlyReactiveTrigger populateField;
			public BattleSceneContext sceneContext;
			public ReactiveEvent<Action<ICubeProjectile>, Vector3> createCubeProjectile;
			public Action<ICubeProjectile> trackProjectile;
		}

		private readonly Ctx _ctx;

		public BasicPopulateFieldPm(Ctx ctx)
		{
			_ctx = ctx;
		}

		public void Init()
		{
			for (int i = 0; i < _ctx.sceneContext.CubeSpawnLoactions.Length; i++)
			{
				_ctx.sceneContext.CubeSpawnLoactions[i].gameObject.SetActive(false); // hide spawn locations
			}
			AddDispose(_ctx.populateField.Subscribe(populateField));
		}

		private void populateField()
		{
			int length = _ctx.sceneContext.CubeSpawnLoactions.Length;
			for (int i = 0; i < length; i++)
			{
				int cache = i;
				_ctx.createCubeProjectile.Notify(projectile =>
				{
					if (cache / 5 == 0) projectile.CubeValue = 16;
					if (cache / 5 == 1) projectile.CubeValue = 8;
					if (cache / 5 == 2) projectile.CubeValue = 4;
					if (cache / 5 == 3) projectile.CubeValue = 2;
					_ctx.trackProjectile.Invoke(projectile);
				}, _ctx.sceneContext.CubeSpawnLoactions[i].position);
			}
		}
	}
}
