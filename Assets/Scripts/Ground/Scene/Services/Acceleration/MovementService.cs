using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Infrastructure;
using Game.Ground.Services.Movement;
using UnityEngine;

namespace Game.Ground.Services
{
	public class AccelerationService : IUpdateTarget, IInitable, AccelerationService.IVelocityDataProvider
	{
		private readonly IUpdater _updater;
		private readonly Dictionary<IMovable, TargetForces> _items = new();

		public AccelerationService(IUpdater updater)
		{
			_updater = updater;
		}

		public Vector3 GetVelocity(IMovable target)
		{
			return _items.TryGetValue(target, out var forces) ? forces.Velocity : Vector3.zero;
		}
		
		public void HandleUpdate(float dt)
		{
			_items.Values.ForEach(f => f.Update(dt));
		}

		public void Init()
		{
			_updater.AddUpdate(this);
		}
		
		public void UnInit()
		{
			_updater.RemoveUpdate(this);
		}

		public IForce AddForce(IMovable target, IForce force)
		{
			if (!_items.TryGetValue(target, out var forces))
			{
				_items[target] = forces = new TargetForces(target);
			}
			forces.AddForce(force);
			return force;
		}

		public IDirection AddMovement(IMovable target, Movement movement)
		{
			if (!_items.TryGetValue(target, out var forces))
			{
				_items[target] = forces = new TargetForces(target);
			}
			forces.AddMovement(movement);
			return forces;
		}


		public IDirection GetDirection(IMovable target)
		{
			return _items.TryGetValue(target, out var result) ? result : null;
		}
		
		public interface IVelocityDataProvider
		{
			Vector3 GetVelocity(IMovable target);
		}

		public class VelocityProvider
		{
			private readonly AccelerationService _service;
			private readonly IMovable _target;

			public VelocityProvider(AccelerationService service, IMovable target)
			{
				_target = target;
				_service = service;
			}

			public Vector3 GetVelocity() => _service.GetVelocity(_target);
		}

		public interface IForce : IDirection
		{
			bool Completed { get; }
			Vector3 Update(float dt);
		}
		
		public abstract class Movement : IDirection
		{
			public bool Completed { get; protected set; }
			public float Duration { get; protected set; }
			public abstract Vector3 Update(float dt);
			public abstract void Stop();
			public abstract void Rotate(Quaternion value);
		}


	}
}