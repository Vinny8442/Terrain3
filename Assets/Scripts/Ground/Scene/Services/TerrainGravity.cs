using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Infrastructure;
using UnityEngine;
using Zenject;

namespace Game.Ground.Services
{
	public class TerrainGravity : MonoBehaviour, IUpdateTarget, IInjectable, IInitable
	{
		[SerializeField] private float _g = 2f;
		[SerializeField] private float _airFriction = 0.01f;

		[Inject] private IUpdater _updater;
		[Inject] private AccelerationService _acceleration;

		private readonly Vector3 _direction = Vector3.down;


		public void Init()
		{
			_updater.AddUpdate(this);
		}

		public void UnInit()
		{
			_updater.RemoveUpdate(this);
		}

		public void AddTarget(ITarget target)
		{
			_acceleration.AddForce(target, new Gravity(_acceleration, target, _g, _direction, _airFriction));
		}
		
		public interface ITarget : AccelerationService.ITarget
		{
			bool IsGrounded { get; }
		}

		public void HandleUpdate(float dt)
		{
		}

		private class Gravity : AccelerationService.IForceHandle, AccelerationService.IForce
		{
			private readonly Vector3 _value;
			private readonly AccelerationService _velocityDataProvider;
			private readonly float _airFrictionFactor;
			private readonly ITarget _target;
			private float _freeFallTime = 0;
			private bool _grounded = false;

			public Gravity(AccelerationService velocityDataProvider, ITarget target, float value, Vector3 direction, float airFrictionFactor)
			{
				_target = target;
				_airFrictionFactor = airFrictionFactor;
				_velocityDataProvider = velocityDataProvider;
				_value = value * direction;
			}

			public void Complete()
			{
				Completed = true;
			}

			public void Rotate(Quaternion rotation)
			{
				
			}

			public bool Completed { get; private set; }

			public Vector3 Update(float dt)
			{
				if (_target.IsGrounded)
				{
					if (!_grounded)
					{
						_grounded = true;
						var vp = Vector3.Project(_velocityDataProvider.GetVelocity(_target), _value);
						Debug.Log($"Grounded after {_freeFallTime}");
						return -vp / dt * 0.95f;
					}
					_freeFallTime = 0;
					return Vector3.zero;
				}

				_grounded = false;
				_freeFallTime += dt;
				var velocityProjection = Vector3.Project(_velocityDataProvider.GetVelocity(_target), _value).magnitude;
				var airFriction = velocityProjection * _airFrictionFactor * _value;
				return _value - airFriction;
			}
		}
	}
}