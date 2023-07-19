using UnityEngine;

namespace Game.Ground.Services.Movement
{
	public class SimpleMovement : AccelerationService.Movement
	{
		private readonly float _duration;
		private Vector3 _direction;
		private float _time = 0;

		public SimpleMovement(Vector3 direction, float duration = float.MaxValue)
		{
			_duration = duration;
			_direction = direction;
		}
		public override Vector3 Update(float dt)
		{
			if (Completed)
			{
				return Vector3.zero;
			}
			
			_time += dt;
			if (_time > _duration)
			{
				Completed = true;
			}
			return _direction * dt;
		}

		public override void Stop()
		{
			Completed = true;
		}

		public override void Rotate(Quaternion value)
		{
			_direction = value * _direction;
		}
	}
}