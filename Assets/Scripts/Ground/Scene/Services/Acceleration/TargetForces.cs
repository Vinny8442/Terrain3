using System.Collections.Generic;
using System.Linq;
using Core.Infrastructure;
using Game.Ground.Services.Movement;
using UnityEngine;

namespace Game.Ground.Services
{
	internal class TargetForces : IDirection
	{
		private readonly IMovable _target;
		private readonly List<AccelerationService.IForce> _forces = new();
		private readonly List<AccelerationService.Movement> _movements = new();
		private Vector3 _accumulatedVelocity = Vector3.zero;
		public Vector3 Velocity { get; private set; } = Vector3.zero;

		public TargetForces(IMovable target)
		{
			_target = target;
		}

		public void AddForce(AccelerationService.IForce force)
		{
			_forces.Add(force);
		}

		public void AddMovement(AccelerationService.Movement movement)
		{
			_movements.Add(movement);
		}

		public void Update(float dt)
		{
			Velocity = UpdateForces(dt) + UpdateMovements(dt);
			_target.ApplyMove(Velocity * dt);
		}

		public void Rotate(Quaternion rotation)
		{
			Velocity = rotation * Velocity;
			_forces.OfType<IDirection>().ForEach(f => f.Rotate(rotation));
			_movements.OfType<IDirection>().ForEach(f => f.Rotate(rotation));
		}

		private Vector3 UpdateForces(float dt)
		{
			var resultAcc = _forces.Aggregate(Vector3.zero, (current, force) => current + force.Update(dt));
				
			_accumulatedVelocity += resultAcc * dt;
				
			_forces.RemoveAll(f => f.Completed);
			return _accumulatedVelocity;
		}

		private Vector3 UpdateMovements(float dt)
		{
			var speed = _movements.Aggregate(Vector3.zero, (current, movement) => current + movement.Update(dt));
			_movements.RemoveAll(m => m.Completed);
			return speed;
		}
	}
}