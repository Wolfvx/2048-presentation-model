using UnityEngine;

namespace Core.Loaders.ResourceLoaders.Configs
{
	[CreateAssetMenu(fileName = "ResourceConfigPrefab.asset", menuName = "Resource Configs/Create ResourceConfigPrefab")]
	public class ResourceConfigPrefab : ScriptableObject
	{
		public GameObject[] _Prefabs;
	}
}