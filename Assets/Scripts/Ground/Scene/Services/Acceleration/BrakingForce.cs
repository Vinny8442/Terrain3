using UnityEngine;

namespace Game.Ground.Services
{
	public class BrakingForce : AccelerationService.IForceHandle, AccelerationService.IForce
	{
		private readonly AccelerationService.VelocityProvider _velocityProvider;
		private Vector3 _force;

		public BrakingForce(AccelerationService.VelocityProvider velocityProvider, Vector3 force, bool temporary = false)
		{
			_force = force;
			_velocityProvider = velocityProvider;
		}
			
		public void Rotate(Quaternion rotation)
		{
			_force = rotation * _force;
		}

		public bool Completed => false;
			
		public Vector3 Update(float dt)
		{
			Vector3 v = _velocityProvider.GetVelocity();
			var velocity = Vector3.Project(v, _force);
			
			var dot = Vector3.Dot(velocity, _force);
			if (dot >= 0) return Vector3.zero;

			var dv = _force * dt;
			if (velocity.magnitude < dv.magnitude)
			{
				return velocity / -dt;
			}

			return _force;
		}
	}
}