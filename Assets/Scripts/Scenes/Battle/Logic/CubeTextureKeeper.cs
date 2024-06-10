using Core.Shared.Foundation;
using UnityEngine;

namespace Scenes.Battle.Logic
{
	public class CubeTextureKeeper : SingletonBehaviour<CubeTextureKeeper>
	{

		[SerializeField] private Texture[] _cube_textures = null;

		public static Texture GetCubeTextureByValue(int cube_value)
		{
			int tex_index = (int)Mathf.Log(cube_value, 2) - 1;
			if (tex_index < 0 || tex_index > Instance._cube_textures.Length)
			{
				Debug.LogError("cube_value " + cube_value.ToString() + " is off");
			}
			return Instance._cube_textures[tex_index];
		}

	}
}
