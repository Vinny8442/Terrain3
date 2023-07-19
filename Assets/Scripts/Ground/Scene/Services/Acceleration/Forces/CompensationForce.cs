using UnityEngine;

namespace Game.Ground.Services
{
	public class CompensationForce : AccelerationService.IForce
	{
		private readonly AccelerationService.IForce _target;
		private readonly float _compensationDuration;
		private Vector3 _aggSpeed = Vector3.zero;
		private Vector3 _compensation = Vector3.zero;

		private float _time = 0;

		public CompensationForce(AccelerationService.IForce target, float compensationDuration)
		{
			_compensationDuration = compensationDuration;
			_target = target;
		}
		
		public void Rotate(Quaternion value)
		{
			
		}

		public bool Completed { get; private set; }
		
		public Vector3 Update(float dt)
		{
			if (Completed) return Vector3.zero;
			
			if (_compensation == Vector3.zero  && _target.Completed)
			{
				_compensation = _aggSpeed / -_compensationDuration;
			}

			if (_compensation == Vector3.zero)
			{
				var targetAcc = _target.Update(dt);
				_aggSpeed += targetAcc * dt;
				return targetAcc;
			}

			_aggSpeed += _compensation * dt;
			if (Vector3.Dot(_aggSpeed, _compensation) > 0)
			{
				Completed = true;
				return (_compensation - _aggSpeed / dt);
			}
			
			_time += dt;
			return _compensation;
		}
	}
}