using System;
using Core.Shared.Scenes;
using Scenes.Battle;

namespace Scenes.Core
{
	public static class SceneFactory
	{
		public static ScenePm GetScene(ref ScenePm.Ctx ctx)
		{
			if (ctx.sceneImage == null)
			{
				throw new NullReferenceException();
			}
			return ctx.sceneImage.sceneName switch
			{
				SceneName.Battle => new BattleScenePm(ctx),
				_ => throw new ArgumentOutOfRangeException(nameof(ctx.sceneImage.sceneName), ctx.sceneImage.sceneName, null)
			};
		}
	}
}