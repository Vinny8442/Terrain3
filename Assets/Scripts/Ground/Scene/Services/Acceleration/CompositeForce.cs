using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Ground.Services
{
	public class CompositeForce : AccelerationService.IForceHandle, AccelerationService.IForce
	{
		protected readonly IEnumerable<AccelerationService.IForce> _forces;

		public CompositeForce(IEnumerable<AccelerationService.IForce> forces)
		{
			_forces = forces;
		}
		
		public void Rotate(Quaternion rotation)
		{
			foreach (var force in _forces.OfType<AccelerationService.IForceHandle>())
			{
				force.Rotate(rotation);
			}
		}

		public bool Completed => _forces.All(f => f.Completed);
		
		public Vector3 Update(float dt)
		{
			return _forces.Aggregate(Vector3.zero, (current, f) => current + f.Update(dt));
		}
	}
}