using System;
using Core.Shared.ReactiveExtensions;
using UniRx;
using UnityEngine;

namespace Scenes.Battle.Logic.Projectiles
{
	[RequireComponent(typeof(Renderer))]
	public class CubeProjectileView : BaseProjectileView
	{
		public struct Ctx
		{
			public CompositeDisposable viewDisposable;
			public Action onReturnProjectile;
			public IReadOnlyReactiveTrigger<float> launchProjectile;
			public ReactiveProperty<int> cubeValue;
		};

		public override PROJECTILE_TYPE ProjectileType => PROJECTILE_TYPE.CUBE;
		public bool IsMoving => _rigidbody.velocity.magnitude > 0.01f;

		private int cubeValue { get => _ctx.cubeValue.Value; set => _ctx.cubeValue.Value = value; }

		private Renderer _renderer = null;

		private Ctx _ctx;

		public void SetCtx(Ctx ctx)
		{
			_ctx = ctx;
			_alive = true;
			_ctx.viewDisposable.Add(_ctx.launchProjectile.Subscribe(Launch));
			_ctx.viewDisposable.Add(_ctx.cubeValue.Subscribe(updateCubeTexture));
		}

		protected override void Awake()
		{
			base.Awake();
			if (_renderer == null) _renderer = GetComponent<Renderer>();
		}

		private void updateCubeTexture(int value)
		{
			if (value == 0) return;
			_renderer.material.mainTexture = CubeTextureKeeper.GetCubeTextureByValue(value);
		}

		private void beConsumed()
		{
			_ctx.onReturnProjectile.Invoke();
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!_alive) return;
			var other_cube = collision.gameObject.GetComponent<CubeProjectileView>();
			if (other_cube != null)
			{
				if (other_cube.cubeValue == cubeValue)
				{
					cubeValue *= 2;
					other_cube.beConsumed();
				}
			}
		}
	}
}
