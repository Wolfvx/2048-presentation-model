using Core.Shared.ReactiveExtensions;
using UniRx;
using UnityEngine;

namespace Scenes.Battle.Logic.Input
{
	public class InputModel : IInputModel
	{
		public ReactiveTrigger onLaunchProjectile;
		public ReactiveProperty<float> onMoveProjectile;

		public InputModel()
		{
			onLaunchProjectile = new ReactiveTrigger();
			onMoveProjectile = new ReactiveProperty<float>(0f);
		}

		IReadOnlyReactiveTrigger IInputModel.onLaunchProjectile => onLaunchProjectile;
		IReadOnlyReactiveProperty<float> IInputModel.onMoveProjectile => onMoveProjectile;
	}
}
