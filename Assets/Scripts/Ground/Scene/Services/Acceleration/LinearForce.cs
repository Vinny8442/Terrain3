using UnityEngine;

namespace Game.Ground.Services
{
	public class LinearForce : AccelerationService.IForceHandle, AccelerationService.IForce
	{
		private Vector3 _force = Vector3.zero;
		private readonly AccelerationService.VelocityProvider _velocityProvider;
		private readonly bool _completeOnTimeout;
		private readonly float _maxSpeed;
		private float _duration;
		private float _time = 0;

		public LinearForce(AccelerationService.VelocityProvider velocityProvider, Vector3 force, float maxSpeed = 0, bool completeOnTimeout = true)
		{
			_maxSpeed = maxSpeed;
			_velocityProvider = velocityProvider;
			_completeOnTimeout = completeOnTimeout;
			_force = force;
			_time = 0;
			_duration = -1;
		}

		public void Rotate(Quaternion rotation)
		{
			_force = rotation * _force;
		}

		public bool Completed => _completeOnTimeout && _time > _duration;


		public Vector3 Update(float dt)
		{
			if (Completed || IsPending)
			{
				return Vector3.zero;
			}

			_time += dt;
			
			if (_maxSpeed > 0 && GetSpeedProjection().magnitude > _maxSpeed)
			{
				return Vector3.zero;
			}

			return _force;
		}

		public void Proceed(float duration)
		{
			_time = 0;
			_duration = duration;
		}

		public bool IsPending => _time > _duration; 

		private Vector3 GetSpeedProjection()
		{
			var projection = Vector3.Project(_velocityProvider.GetVelocity(), _force);
			if (Vector3.Dot(_force, projection) > 0)
			{
				return projection;
			}
			return Vector3.zero;
		}
	}
}