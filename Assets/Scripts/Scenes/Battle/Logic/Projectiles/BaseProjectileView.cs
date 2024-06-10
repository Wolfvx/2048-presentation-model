using Core.Shared.Foundation;
using UnityEngine;

namespace Scenes.Battle.Logic.Projectiles
{
	[RequireComponent(typeof(Rigidbody))]
	public abstract class BaseProjectileView : BaseMonoBehaviour
	{
		public virtual PROJECTILE_TYPE ProjectileType => PROJECTILE_TYPE.NONE;

		protected Rigidbody _rigidbody = null;
		protected bool _alive = false;

		protected override void Awake()
		{
			base.Awake();
			if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
		}

		protected void OnEnable()
		{
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.angularVelocity = Vector3.zero;
			_rigidbody.ResetInertiaTensor();
		}

		protected void Launch(float strength)
		{
			_rigidbody.AddForce(new Vector3(0, 0, strength));
			//StartCoroutine(trackProjectileAfter(0.2f));
		}

		protected void SetPosition(Vector3 position)
		{
			transform.position = position;
		}
	}
}
