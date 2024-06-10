using System;
using System.Collections.Generic;
using Core.Shared.Foundation;
using Core.Shared.Loaders;
using Core.Shared.ReactiveExtensions;
using Core.Shared.Containers;
using UnityEngine;
using UniRx;

namespace Scenes.Battle.Logic.Projectiles
{
	public class CubeProjectileFactoryPm : BaseDisposable
	{
		public struct Ctx
		{
			public IResourceLoader resourceLoader;
			public Transform projectileParent;
			public IReadOnlyReactiveEvent<Action<ICubeProjectile>, Vector3> createCubeProjectile;
			public Action<ICubeProjectile> trackProjectile;
			public Action<ICubeProjectile> untrackProjectile;
			public ReactiveProperty<int> cubeValueNotify;
			public int maxProjectiles;
		}

		private LinkedList<CubeProjectileView> _cubePool = null;
		private List<IDisposable> _disposables = null;
		private GameObject _cubePrefab;

		private readonly Ctx _ctx;
		private readonly ResourceImage _cubeRes = new ResourceImage() { 
			prefabName = "CubeProjectile"
		};

		public CubeProjectileFactoryPm(Ctx ctx)
		{
			_ctx = ctx;
		}

		public void Init()
		{
			_disposables = new List<IDisposable>();
			AddDispose(_ctx.resourceLoader.LoadPrefab(_cubeRes, prefab =>
			{
				_cubePrefab = prefab;
				createAndPopulatePool();
			}));
			AddDispose(_ctx.createCubeProjectile.SubscribeWithSkip(CreateProjectile));
		}

		protected override void OnDispose()
		{
			foreach(IDisposable disposable in _disposables)
			{
				disposable.Dispose();
			}
			_disposables.Clear();
			base.OnDispose();
		}

		private void CreateProjectile(Action<ICubeProjectile> callback, Vector3 position)
		{

			if (_cubePool.Count == 0)
			{
				// LOOSE Condition?
				Debug.LogError("No more free cubes!");
				return;
			}
			var view = _cubePool.First.Value;
			_cubePool.RemoveFirst();
			CubeProjectilePm cubePm = new CubeProjectilePm(new CubeProjectilePm.Ctx
			{
				spawnPosition = position,
				view = view,
				returnProjectile = returnProjectile,
				trackProjectile = _ctx.trackProjectile,
				untrackProjectile = _ctx.untrackProjectile,
				cubeValueNotify = _ctx.cubeValueNotify
			});
			_disposables.Add(cubePm);
			cubePm.Init();

			callback.Invoke(cubePm);
		}

		private void returnProjectile(CubeProjectilePm pm, CubeProjectileView view)
		{
			_disposables.Remove(pm);
			pm.Dispose();
			view.gameObject.SetActive(false);
			_cubePool.AddLast(view);
		}

		private void createAndPopulatePool()
		{
			_cubePool = new LinkedList<CubeProjectileView>();
			for (int i = 0; i < _ctx.maxProjectiles; i++)
			{
				var obj = AddComponent(GameObject.Instantiate(_cubePrefab, new Vector3(-1000, 0, 0), Quaternion.identity, _ctx.projectileParent));
				var projectile = obj.GetComponent<CubeProjectileView>();
				_cubePool.AddLast(projectile);
				obj.SetActive(false);
			}
		}
	}
}
