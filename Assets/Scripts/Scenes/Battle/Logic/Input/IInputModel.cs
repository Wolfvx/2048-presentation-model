using Core.Shared.ReactiveExtensions;
using UniRx;
using UnityEngine;

namespace Scenes.Battle.Logic.Input
{
	public interface IInputModel
	{
		public IReadOnlyReactiveTrigger onLaunchProjectile { get; }
		public IReadOnlyReactiveProperty<float> onMoveProjectile { get; }
	}
}
