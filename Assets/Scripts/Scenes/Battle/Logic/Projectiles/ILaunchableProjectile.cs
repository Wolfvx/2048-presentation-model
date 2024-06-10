using UnityEngine;

namespace Scenes.Battle.Logic.Projectiles
{
	public interface ILaunchableProjectile
	{

		void Launch(float strength);

		Vector3 Position { get; set; }

	}
}
