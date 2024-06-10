using Core.Shared.Containers;
using Core.Shared.Foundation;
using UniRx;

namespace Scenes.Core
{
	public class RootInit : BaseMonoBehaviour
	{

		protected virtual void Start()
		{
			DontDestroyOnLoad(this);
			Init();
		}

		protected void Init()
		{
			RootPm root = new RootPm(new RootPm.Ctx
			{
				getParams = GetInitParams
			});
			root.AddTo(this);
			// ReSharper disable ConstantConditionalAccessQualifier
			Observable.OnceApplicationQuit().Subscribe(_ => root?.Dispose()).AddTo(this);
			// ReSharper restore ConstantConditionalAccessQualifier
			root.Init();
		}

		protected virtual InitSt GetInitParams()
		{
			return new InitSt();
		}
	}
}