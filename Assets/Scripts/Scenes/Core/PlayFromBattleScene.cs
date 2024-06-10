using System;
using Core.Shared.Containers;
using Scenes.Battle.Shared.Containers;
using UnityEngine;

namespace Scenes.Core
{
	public class PlayFromBattleScene : RootInit
	{

		[Serializable]
		public class BattleSceneImageS : BattleSceneImage
		{ }

		[SerializeField]
		private BattleSceneImageS _sceneImage;
		
		protected override void Start()
		{
			if (RootPm.Exists)
				return;
			Init();
		}

		protected override InitSt GetInitParams()
		{
			return new InitSt
			{
				sceneImage = _sceneImage,
				debug = true
			};
		}
	}
}