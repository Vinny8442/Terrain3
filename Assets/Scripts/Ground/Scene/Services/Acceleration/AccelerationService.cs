using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Infrastructure;
using UnityEngine;

namespace Game.Ground.Services
{
	public class AccelerationService : IUpdateTarget, IInitable, IInjectable, AccelerationService.IVelocityDataProvider
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
			forces.AddForce(force);
			return force;
		}


		public IForceHandle GetHandle(ITarget target)
		{
			return _items.TryGetValue(target, out var result) ? result : null;
		}
		
		public interface IVelocityDataProvider
		{
			Vector3 GetVelocity(ITarget target);
		}

		public class VelocityProvider
		{
			private readonly AccelerationService _service;
			private readonly ITarget _target;

			public VelocityProvider(AccelerationService service, ITarget target)
			{
				_target = target;
				_service = service;
			}

			public Vector3 GetVelocity() => _service.GetVelocity(_target);
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


		private class TargetForces : AccelerationService.IForceHandle
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

			public void Rotate(Quaternion rotation)
			{
				Velocity = rotation * Velocity;
				_forces.OfType<IForceHandle>().ForEach(f => f.Rotate(rotation));
			}
		}
	}
}