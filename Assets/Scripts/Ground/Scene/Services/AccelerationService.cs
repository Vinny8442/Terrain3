using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Infrastructure;
using UnityEngine;

namespace Game.Ground.Services
{
	public class AccelerationService : IUpdateTarget, IInitable, IInjectable
	{
		private readonly IUpdater _updater;
		private readonly List<IAccelerationData> _forces = new();

		public AccelerationService(IUpdater updater)
		{
			_updater = updater;
		}
		
		public void HandleUpdate(float dt)
		{
			foreach (var force in _forces.ToArray())
			{
				if (!force.Completed){
					force.Update(dt);
					if (force.Completed)
					{
						_forces.Remove(force);
					}
				}
			}
		}

		public void Init()
		{
			_updater.AddUpdate(this);
		}
		
		public void UnInit()
		{
			_updater.RemoveUpdate(this);
		}

		public Handler CreateAcceleration(Target target, Vector3 force, float decelerationRatio, float maxSpeed)
		{
			var result = new RegularAcceleration(target, force, maxSpeed, decelerationRatio);
			_forces.Add(result);
			return result;
		}

		public Handler CreateAcceleration(Target target, Vector3 force, AnimationCurve curve)
		{
			var result = new CurveAcceleration(target, force, curve);
			_forces.Add(result);
			return result;
		}

		public interface Target
		{
			void ApplyAcc(Vector3 movement);
		}

		public interface Handler
		{
			void Proceed(float dt);
			bool IsIdle { get; }
			void ReleaseWhenCompleted();
			void Rotate(Quaternion rotation);
		}

		private interface IAccelerationData
		{
			bool Completed { get; }
			void Update(float dt);
		}


		private abstract class AbstractAcceleration : Handler, IAccelerationData
		{
			protected enum State
			{
				Idle,
				Acceleration, 
				Deceleration
			}
			
			protected State _state = State.Idle;
			private bool _releaseWhenCompleted = false;
			
			public abstract void Proceed(float dt);

			public bool IsIdle => _state == State.Idle;
			
			public void ReleaseWhenCompleted()
			{
				_releaseWhenCompleted = true;
			}

			public abstract void Rotate(Quaternion rotation);

			public bool Completed { get; private set; } = false;
			
			public abstract void Update(float dt);

			protected void Complete()
			{
				if (_releaseWhenCompleted)
				{
					Completed = true;
				}
			}
			
		}

		private class CurveAcceleration : AbstractAcceleration
		{
			private Target _target;
			private Vector3 _force;
			private CurveWrapper _curve;
			private float _time;
			private float _timeLeft;
			private Vector3 _currentSpeed;

			public CurveAcceleration(Target target, Vector3 force, AnimationCurve curve)
			{
				_curve = new CurveWrapper(curve);
				_force = force;
				_target = target;
				_time = 0;
			}

			public override void Proceed(float dt)
			{
				
				if (_state == State.Idle && dt == 0)
				{
					_timeLeft = _curve.Duration;
				} 
				_state = State.Acceleration;
				_timeLeft = Math.Max(dt, _timeLeft);
			}

			public override void Rotate(Quaternion rotation)
			{
				
			}

			public override void Update(float dt)
			{
				if (_timeLeft <= 0)
				{
					_time = 0;
					_timeLeft = 0;
					if (_state == State.Acceleration)
					{
						Debug.Log($"@#NO time left {_timeLeft}");
						_state = State.Idle;
						Complete();
					}
					return;
				}
				
				_timeLeft -= dt;
				_time += dt;
				if (_time > _curve.Duration)
				{
					_time -= _curve.Duration;
				}

				_currentSpeed = _curve.Evaluate(_time) * _force;
				
				Debug.Log($"@#UPD {_timeLeft} {_time} {_currentSpeed}");
				
				_target.ApplyAcc(_currentSpeed * dt);
			}

			private readonly struct CurveWrapper
			{
				private readonly AnimationCurve _source;
				private readonly float _curveMaxValue;

				public readonly float Duration;

				public CurveWrapper(AnimationCurve source)
				{
					_source = source;
					_curveMaxValue = _source.keys.Max(key => key.value);
					Duration = _source.keys.Max(key => key.time);
				}

				public float Evaluate(float normalizedTime)
				{
					return _source.Evaluate(normalizedTime);
				}
			}
		}

		private class RegularAcceleration : AbstractAcceleration
		{
			
			private  Vector3 _acceleration = Vector3.zero;
			private  Vector3 _deceleration;
			private readonly float _maxSpeed = 1.3f;
			private readonly Target _target = null;
			private Vector3 _currentSpeed = Vector3.zero;
			private float _timeLeft = 0;

			public RegularAcceleration(Target target, Vector3 acceleration, float maxSpeed, float decelerationRatio = 1)
			{
				_maxSpeed = maxSpeed;
				_acceleration = acceleration;
				_deceleration = acceleration * -1 * decelerationRatio;
				_target = target;
			}

			public override void Rotate(Quaternion rotation)
			{
				_currentSpeed = rotation * _currentSpeed;
				_acceleration = rotation * _acceleration; 
				_deceleration = rotation * _deceleration;

				// Quaternion.
			}

			public override void Update(float dt)
			{
				var movement = Vector3.zero;
				if (_state == State.Acceleration)
				{
					movement = Accelerate(dt);
				}
				else if (_state == State.Deceleration)
				{
					movement = Decelerate(dt);
				}
				
				if (_state != State.Idle)
				{
					_target.ApplyAcc(movement);
				}
			}

			public override void Proceed(float dt)
			{
				_state = State.Acceleration;
				_timeLeft = Math.Max(dt, _timeLeft);
			}

			private Vector3 Accelerate(float dt)
			{
				_currentSpeed += _acceleration * dt;
				if (_currentSpeed.magnitude > _maxSpeed)
				{
					var ratio = _maxSpeed / _currentSpeed.magnitude;
					_currentSpeed *= ratio;
				}

				if (dt > _timeLeft)
				{
					dt = _timeLeft;
					_state = State.Deceleration;
				}

				Debug.Log($"Acc {_currentSpeed.magnitude} {_acceleration.magnitude} {_timeLeft} ");
				_timeLeft -= dt;
				return _currentSpeed * dt;
			}

			private Vector3 Decelerate(float dt)
			{
				var dv = _deceleration * dt;
				
				if (_currentSpeed.magnitude <= dv.magnitude)
				{
					_currentSpeed = Vector3.zero;
					_state = State.Idle;
					
					Complete();
				}
				else
				{
					_currentSpeed += dv;
				}
				Debug.Log($"Dcc {_currentSpeed.magnitude} {_deceleration.magnitude}");
				return _currentSpeed * dt;
			}
			
		}
	}
}