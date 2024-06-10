using Core.Shared.Containers;
using Core.Shared.Foundation;
using Core.Shared.ReactiveExtensions;
using Core.Shared.Scenes;
using Game.Shared;
using Scenes.Battle.Shared.Containers;

namespace Game
{
	public class GameProfile : BaseClass, IGameProfile
	{
		public struct Ctx
		{
			public InitSt initSt;
			public ReactiveEvent<SceneImage> changeScene;
		}

		private readonly Ctx _ctx;
		private readonly ReactiveEvent<SceneImage> _changeScene;
		
		public GameProfile(Ctx ctx)
		{
			_ctx = ctx;
			_changeScene = ctx.changeScene;
			if (_ctx.initSt.sceneImage != null)
				_changeScene.Notify(_ctx.initSt.sceneImage);
			else
				setDefaultScene();
		}

		private void setDefaultScene()
		{
			_changeScene.Notify(new BattleSceneImage()
			{
				sceneName = SceneName.Battle
			});
		}
	}
}