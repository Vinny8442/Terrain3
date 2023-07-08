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
		private readonly Dictionary<ITarget, TargetForces> _items = new();

		public AccelerationService(IUpdater updater)
		{
			_updater = updater;
		}

		public Vector3 GetVelocity(ITarget target)
		{
			return _items.TryGetValue(target, out var forces) ? forces.Velocity : Vector3.zero;
		}
		
		public void HandleUpdate(float dt)
		{
			foreach (var forces in _items.Values)
			{
				forces.Update(dt);
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

		public IForce AddForce(ITarget target, IForce force)
		{
			if (!_items.TryGetValue(target, out var forces))
			{
				_items[target] = forces = new TargetForces(target);
			}
			// var forces = _items[target] ??= new TargetForces(target);
			forces.AddForce(force);
			return force;
		}


		public interface ITarget
		{
			void ApplyAcc(Vector3 movement);
		}

		public interface IForceHandle
		{
			void Rotate(Quaternion rotation);
		}

		public interface IForce
		{
			bool Completed { get; }
			Vector3 Update(float dt);
		}


		private class TargetForces
		{
			private readonly ITarget _target;
			private readonly List<IForce> _forces = new();
			public Vector3 Velocity { get; private set; } = Vector3.zero;

			public TargetForces(ITarget target)
			{
				_target = target;
			}

			public void AddForce(IForce force)
			{
				_forces.Add(force);
			}

			public void Update(float dt)
			{
				Vector3 resultAcc = _forces.Aggregate(Vector3.zero, (current, force) =>
				{
					var acc = force.Update(dt);
					if (acc.magnitude > 0)
					{
						Debug.Log($"FORCE: {force} {acc}");
					}
					return current + acc;
				});
				
				Velocity += resultAcc * dt;
				if (resultAcc.magnitude > 0)
				{
					Debug.Log($"-------------------- UPDATE {Velocity}");
				}


				_forces.RemoveAll(f => f.Completed);
				_target.ApplyAcc(Velocity * dt);
			}
			
		}
		public class CurveForce : IForceHandle, IForce
		{
			private Vector3 _force;
			private AnimationCurve _curve;
			private float _time;
			private Keyframe _lastKey;
			private float _duration;

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

		public class LinearForce : IForceHandle, IForce
		{
			private  Vector3 _acceleration = Vector3.zero;
			private float _timeLeft = 0;
			private bool _deceleration = false;
			private readonly float _duration;
			private readonly float _decelerationRatio;

			public LinearForce(Vector3 force, float duration, float decelerationRatio = 0)
			{
				_decelerationRatio = decelerationRatio;
				_duration = duration;
				_timeLeft = duration;
				_acceleration = force;
			}

			public void Rotate(Quaternion rotation)
			{
				_acceleration = rotation * _acceleration; 
			}

			public bool Completed { get; private set; }
			
			
			public Vector3 Update(float dt)
			{
				if (Completed)
				{
					return Vector3.zero;
				}
				
				_timeLeft -= dt;
				if (_timeLeft <= 0)
				{
					if (_deceleration)
					{
						Completed = true;
						return Vector3.zero;
					}
					else
					{
						if (_decelerationRatio > 0)
						{
							_deceleration = true;
							_timeLeft = _duration / _decelerationRatio;
							_acceleration *= -_decelerationRatio;
						}
						else
						{
							Completed = true;
							return Vector3.zero;
						}
					}
				}
				return _acceleration;
			}
		}
	}
}