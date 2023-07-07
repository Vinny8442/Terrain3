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

		[Inject] private IUpdater _updater;

		private readonly List<TargetData> _targets = new List<TargetData>();
		private readonly Vector3 _direction = Vector3.down;


		public void Init()
		{
			_updater.AddUpdate(this);
		}

		public void UnInit()
		{
			_updater.RemoveUpdate(this);
		}

		public void AddTarget(Target target)
		{
			if (!_targets.Any(data => data.Contains(target)))
			{
				_targets.Add(new TargetData(target, _g, _direction));
			}
		}
		
		public interface Target
		{
			void ApplyGravity(Vector3 gravity);
			bool IsGrounded { get; }
		}

		public void HandleUpdate(float dt)
		{
			for (int i = 0; i < _targets.Count; i++)
			{
				var target = _targets[i];
				target.Update(dt);
				_targets[i] = target;
			}
		}

		private struct TargetData
		{
			private readonly Target _target;
			private Vector3 _velocity;
			private readonly float _g;
			private readonly Vector3 _direction;

			public TargetData(Target target, float g, Vector3 direction)
			{
				_direction = direction;
				_g = g;
				_target = target;
				_velocity = Vector3.zero;
			}

			public bool Contains(Target target) => _target == target;

			public void Update(float dt)
			{
				if (_target.IsGrounded)
				{
					_velocity = _direction * _g;
				}
				else
				{
					_velocity += _direction * _g * dt;
				}
				
				_target.ApplyGravity(_velocity * dt);
			}
			
		}
	}
}