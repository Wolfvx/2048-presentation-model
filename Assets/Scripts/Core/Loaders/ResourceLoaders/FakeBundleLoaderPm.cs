using System;
using System.Collections.Generic;
using Core.Loaders.ResourceLoaders.Configs;
using Core.Shared.Containers;
using Core.Shared.Foundation;
using Core.Shared.Loaders;
using Core.Shared.ReactiveExtensions;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Core.Loaders.ResourceLoaders
{
	public class FakeBundleLoaderPm : BaseDisposable, IResourceLoader
	{
		public struct Ctx
		{
			public float minLoadDelay;
			public float maxLoadDelay;
		}

		private readonly Ctx _ctx;
		private readonly ResourceConfigRoot _configRoot;
		private readonly Dictionary<string, GameObject> _prefabsCache;
		private readonly HashSet<string> _cacheImitator;
		
		private const string PRELOADED_FILES_PATH = "FakeBundles/ResourceConfigRoot";

		public FakeBundleLoaderPm(Ctx ctx)
		{
			_ctx = ctx;
			_prefabsCache = new Dictionary<string, GameObject>();
			_configRoot = Resources.Load<ResourceConfigRoot>(PRELOADED_FILES_PATH);
			foreach (ResourceConfigPrefab config in _configRoot._PrefabConfigs)
			{
				if (config == null)
					continue;
				fillDictionaryFromArray(_prefabsCache, config._Prefabs);
			}
			_cacheImitator = new HashSet<string>();
		}

		protected override void OnDispose()
		{
			Resources.UnloadAsset(_configRoot);
			base.OnDispose();
		}

		IDisposable IResourceLoader.LoadPrefab(ResourceImage resourceImage, Action<GameObject> onComplete)
		{
			GameObject prefab = getResource(_prefabsCache, resourceImage.prefabName);
			return imitateLoadingBundle(resourceImage.bundleName, () => onComplete?.Invoke(prefab));
		}
		
		private T getResource<T>(IReadOnlyDictionary<string, T> dict, string resourceName) where T : Object
		{
			if (dict == null)
			{
				Log.Err($"Can't get resource from nullable map by '{resourceName}'");
				return null;
			}
			string key = resourceName;
			if (!dict.TryGetValue(key, out T ret))
			{
				Log.Err($"Can't get resource '{resourceName}'");
				return null;
			}
			return ret;
		}
		
		private IDisposable imitateLoadingBundle(string bundleName, Action onComplete)
		{
			if (string.IsNullOrEmpty(bundleName))
			{
				onComplete?.Invoke();
				return null;
			}
			if (_cacheImitator.Contains(bundleName))
			{
				onComplete?.Invoke();
				return null;
			}
      
			_cacheImitator.Add(bundleName);
			if (_ctx.maxLoadDelay < 0.01f)
			{
				onComplete?.Invoke();
				return null;
			}
			float delay = Random.Range(_ctx.minLoadDelay, _ctx.maxLoadDelay);
			IDisposable delayedCall = ReactiveExtensions.DelayedCall(delay, () => { onComplete?.Invoke(); });
			AddDispose(delayedCall);
			return delayedCall;
		}
		
		private void fillDictionaryFromArray<T>(IDictionary<string, T> dict, IReadOnlyList<T> resourceObjects) where T : Object
		{
			if (resourceObjects == null || resourceObjects.Count <= 0)
				return;
			for (int i = 0, ik = resourceObjects.Count; i < ik; ++i)
			{
				T resourceObj = resourceObjects[i];
				if (resourceObj != null)
				{
					if (dict.ContainsKey(resourceObj.name))
						Log.Info($"duplicating '{resourceObj.name}' in content");
					dict[resourceObj.name] = resourceObj;
				}
			}
		}
	}
}