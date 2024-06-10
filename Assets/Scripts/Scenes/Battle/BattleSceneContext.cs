using Core.Shared.Scenes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scenes.Battle
{
	public class BattleSceneContext : SceneContext
	{
		public Transform MainUI;
		public Transform OverlayUI;
		public Transform PlayerSpawnLocation;
		public Transform[] CubeSpawnLoactions;
		public Transform ProjectileParent;
		public Camera Camera;
		public EventSystem EventSystem;
	}
}