using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scenes.Battle.Logic.Projectiles
{
	public interface ICubeProjectile : ILaunchableProjectile
	{
		public int CubeValue { get; set; }

		public bool isMoving { get; }
	}
}
