using UnityEngine;

namespace Core.Loaders.ResourceLoaders.Configs
{
	[CreateAssetMenu(fileName = "ResourceConfigRoot.asset", menuName = "Resource Configs/Create ResourceConfigRoot")]
	public class ResourceConfigRoot : ScriptableObject
	{
		public ResourceConfigPrefab[] _PrefabConfigs;
	}
}