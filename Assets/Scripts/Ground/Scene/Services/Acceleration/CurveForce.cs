using System.Linq;
using UnityEngine;

namespace Game.Ground.Services
{
	public class CurveForce : AccelerationService.IForceHandle, AccelerationService.IForce
	{
		private readonly AnimationCurve _curve;
		private readonly float _duration;
		private readonly Keyframe _lastKey;

		private Vector3 _force;
		private float _time;

		public CurveForce(Vector3 force, AnimationCurve curve, float duration = 0)
		{
			_curve = curve;
			_force = force;
			_time = 0;
			_lastKey = curve.keys.Last();
			_duration = duration > 0 ? duration : _lastKey.time;
		}

		public void Rotate(Quaternion rotation)
		{
			_force = rotation * _force; 
		}

		public bool Completed { get; private set; }
		public Vector3 Update(float dt)
		{
			if (Completed) return Vector3.zero;
				
			_time += dt;
			if (_time > _duration)
			{
				Completed = true;
			}

			if (_time > _lastKey.time)
			{
				return _force * _lastKey.value;
			}
				
			Debug.Log($"Curve: {_time} {_curve.Evaluate(_time)} {_force * _curve.Evaluate(_time)}");

			return _force * _curve.Evaluate(_time);
		}
	}
}