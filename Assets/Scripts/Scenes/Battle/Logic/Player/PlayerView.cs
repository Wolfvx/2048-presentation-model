using Core.Shared.Foundation;

namespace Scenes.Battle.Logic.Player
{
	public class PlayerView : BaseMonoBehaviour
	{
		public struct Ctx
		{

		}

		private Ctx _ctx;

		void SetCtx(Ctx ctx)
		{
			_ctx = ctx;
		}
	}
}
