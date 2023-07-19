using UnityEngine;

namespace Game.Ground.Services.Movement
{
	public class CurveMovement : AccelerationService.Movement
	{
		private readonly AnimationCurve _curve;
		private readonly bool _stopOnLastKey;
		private Vector3 _direction;
		private float _time = 0;

		public CurveMovement(Vector3 direction, AnimationCurve curve, bool stopOnLastKey = false)
		{
			_stopOnLastKey = stopOnLastKey;
			_direction = direction;
			_curve = curve;
			Duration = stopOnLastKey ? _curve.keys[_curve.length - 1].time : float.MaxValue;
		}
		public override Vector3 Update(float dt)
		{
			if (Completed) return Vector3.zero;
				
			var result = _curve.Evaluate(_time) * _direction;
			_time += dt;
			if (_time > Duration)
			{
				if (_stopOnLastKey)
				{
					Completed = true;
				}
				else
				{
					_time = Duration;
				}
			}

			return result;
		}

		public override void Stop()
		{
			Completed = true;
		}

		public override void Rotate(Quaternion value)
		{
			_direction = value * _direction;
		}

		public void Reset()
		{
			_time = 0;
		}
	}
}