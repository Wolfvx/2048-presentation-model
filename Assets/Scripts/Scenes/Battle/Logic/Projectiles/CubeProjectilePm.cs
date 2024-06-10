using System;
using Core.Shared.ReactiveExtensions;
using UniRx;
using UnityEngine;

namespace Scenes.Battle.Logic.Projectiles
{
	public class CubeProjectilePm : BaseProjectilePm, ICubeProjectile
	{
		public struct Ctx
		{
			public Vector3 spawnPosition;
			public CubeProjectileView view;
			public Action<CubeProjectilePm, CubeProjectileView> returnProjectile;
			public Action<ICubeProjectile> trackProjectile;
			public Action<ICubeProjectile> untrackProjectile;
			public ReactiveProperty<int> cubeValueNotify;
		}

		private ReactiveTrigger<float> _launchProjectile;
		private ReactiveProperty<int> _cubeValue;
		private IDisposable _timer;

		private readonly Ctx _ctx;

		private const float TRACK_DELAY = 0.2f;

		Vector3 ILaunchableProjectile.Position { get => _ctx.view.transform.position; set => _ctx.view.transform.position = value; }

		int ICubeProjectile.CubeValue { get => _cubeValue.Value; set => _cubeValue.Value = value; }

		bool ICubeProjectile.isMoving => _ctx.view.IsMoving;

		public CubeProjectilePm(Ctx ctx)
		{
			_ctx = ctx;
			_launchProjectile = new ReactiveTrigger<float>();
			_cubeValue = new ReactiveProperty<int>(0);
		}

		public void Init()
		{
			_ctx.view.transform.rotation = Quaternion.identity;
			_ctx.view.transform.position = _ctx.spawnPosition;
			_ctx.view.gameObject.SetActive(true);
			CompositeDisposable viewDisposable = AddDispose(new CompositeDisposable());
			_ctx.view.SetCtx(new CubeProjectileView.Ctx
			{
				viewDisposable = viewDisposable,
				onReturnProjectile = onReturnProjectile,
				launchProjectile = _launchProjectile,
				cubeValue = _cubeValue
			});
			AddDispose(_cubeValue.Subscribe(value => _ctx.cubeValueNotify.Value = value));
		}

		protected override void OnDispose()
		{
			_timer?.Dispose();
			base.OnDispose();
		}

		private void onReturnProjectile()
		{
			_ctx.untrackProjectile(this);
			_ctx.returnProjectile(this, _ctx.view);
		}

		void ILaunchableProjectile.Launch(float strength)
		{
			_launchProjectile.Notify(strength);
			_timer = Observable.Timer(TimeSpan.FromSeconds(TRACK_DELAY)).Subscribe(_ =>
			{
				_timer?.Dispose();
				_ctx.trackProjectile(this);
			});
		}
	}
}
