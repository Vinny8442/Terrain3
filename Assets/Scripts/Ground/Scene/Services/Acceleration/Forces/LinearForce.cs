using Game.Ground.Services.Movement;
using UnityEngine;

namespace Game.Ground.Services
{
	public class LinearForce : AccelerationService.IForce
	{
		private Vector3 _force = Vector3.zero;
		private readonly float _duration;
		private float _time = 0;

		public LinearForce(Vector3 force, float duration)
		{
			_force = force;
			_time = 0;
			_duration = duration;
		}

		public void Rotate(Quaternion rotation)
		{
			_force = rotation * _force;
		}

		public bool Completed => _time > _duration;


		public Vector3 Update(float dt)
		{
			if (Completed)
			{
				return Vector3.zero;
			}

			_time += dt;
			
			return _force;
		}
	}
}