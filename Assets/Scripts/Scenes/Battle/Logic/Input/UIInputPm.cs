using Core.Shared.Containers;
using Core.Shared.Foundation;
using Core.Shared.Loaders;
using UnityEngine;

namespace Scenes.Battle.Logic.Input
{
	public class UIInputPm : BaseDisposable
	{
		public struct Ctx
		{
			public IResourceLoader resourceLoader;
			public Transform uiParent;
			public InputModel inputModel;

		}

		private readonly Ctx _ctx; 
		private readonly ResourceImage _uiRes = new ResourceImage()
		{
			prefabName = "InputPanel"
		};

		public UIInputPm(Ctx ctx)
		{
			_ctx = ctx;
		}

		public void Init()
		{
			AddDispose(_ctx.resourceLoader.LoadPrefab(_uiRes, prefab =>
			{
				GameObject obj = AddComponent(GameObject.Instantiate(prefab, _ctx.uiParent));
				UIInputView view = obj.GetComponent<UIInputView>();
				view.SetCtx(new UIInputView.Ctx
				{
					inputModel = _ctx.inputModel
				});
			}));
		}
	}
}
