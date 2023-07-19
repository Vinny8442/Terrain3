using System;
using UnityEngine;

namespace Game.Ground.Services.Movement
{
	public class TransitionMovement : AccelerationService.Movement
	{
		private AccelerationService.Movement _from;
		private AccelerationService.Movement _to;
		private float _duration;
		private float _time = 0;
		
		public TransitionMovement(AccelerationService.Movement from, AccelerationService.Movement to, float duration)
		{
			_duration = duration;
			_to = to;
			_from = from;
		}
		public override Vector3 Update(float dt)
		{
			if (Completed) return Vector3.zero;
			if (_from == null) return _to.Update(dt);
			
			if (_from.Completed)
			{
				_time = _duration;
			}
			
			var factor = _time / _duration;
			_time += dt;
			var result = _from.Update(dt) * (1 - factor) + _to.Update(dt) * factor;
			if (_time >= _duration)
			{
				_from = null;
			}

			return result;
		}

		public override void Stop()
		{
			Completed = true;
		}

		public override void Rotate(Quaternion value)
		{
			_from?.Rotate(value);
			_to.Rotate(value);
		}
	}
}