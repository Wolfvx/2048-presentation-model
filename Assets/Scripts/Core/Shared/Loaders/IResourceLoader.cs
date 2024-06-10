using System;
using Core.Shared.Containers;
using UnityEngine;

namespace Core.Shared.Loaders
{
	public interface IResourceLoader : IDisposable
	{
		IDisposable LoadPrefab(ResourceImage resourceImage, Action<GameObject> onComplete);
	}
}