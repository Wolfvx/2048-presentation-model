using Core.Shared.Foundation;
using Core.Shared.Timers;
using UnityEngine;

namespace Scenes.Battle.Logic.Input
{
	public class KeyboardInputPm : BaseDisposable
	{
		public struct Ctx
		{
			public InputModel inputModel;
			public ITickStream tickStream;
		}

		private readonly Ctx _ctx;

		private const float HORIZONTAL_AUTHORITY = 0.2f;

		public KeyboardInputPm(Ctx ctx)
		{
			_ctx = ctx;
		}

		public void Init()
		{
			AddDispose(_ctx.tickStream.Subscribe(ITickStream.StreamType.UPDATE, Update));
		}

		private void Update(float delta)
		{
			float horizontal = UnityEngine.Input.GetAxis("Horizontal");
			if (horizontal > 0.1f || horizontal < -0.1f)
			{
				horizontal *= HORIZONTAL_AUTHORITY;
				_ctx.inputModel.onMoveProjectile.Value = horizontal;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
			{
				_ctx.inputModel.onLaunchProjectile.Notify();
			}
		}
	}
}
