using Core.Shared.Foundation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scenes.Battle.Logic.Input
{
	public class UIInputView : BaseMonoBehaviour, IDragHandler
	{
		public struct Ctx
		{
			public InputModel inputModel;
		}

		[SerializeField] private float _launch_threshhold = 25f;
		[SerializeField] private float _horizontal_authority = 0.01f;

		private Ctx _ctx;

		public void SetCtx(Ctx ctx)
		{
			_ctx = ctx;
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			_ctx.inputModel.onMoveProjectile.Value = eventData.delta.x * _horizontal_authority;
			if (eventData.delta.y > _launch_threshhold)
			{
				_ctx.inputModel.onLaunchProjectile.Notify();
			}
		}
	}
}
