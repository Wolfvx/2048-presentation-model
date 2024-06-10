using UnityEngine;

namespace Core.Shared.Foundation
{
	public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{

		public static T Instance { get; private set; }

		protected virtual void Awake()
		{
			Instance = this as T;
		}

		protected virtual void OnDestroy()
		{
			Instance = default;
		}

	}

}
