using Game.Ground.Services.Movement;
using UnityEngine;

namespace Game.Ground.Services
{
	public class ResistanceForce : IDirection, AccelerationService.IForce
	{
		private readonly AccelerationService.IVelocityDataProvider _velocityProvider;
		private readonly AccelerationService.IForce _source;
		private readonly IMovable _target;
		private Vector3 _axis;
		private readonly float _factor;

		public ResistanceForce(AccelerationService.IVelocityDataProvider velocityProvider, IMovable target, float factor, AccelerationService.IForce source)
		{
			_target = target;
			_source = source;
			_factor = factor;
			_velocityProvider = velocityProvider;
		}
			
		public void Rotate(Quaternion rotation)
		{
			_axis = rotation * _axis;
		}

		public bool Completed => _source?.Completed ?? false;
			
		public Vector3 Update(float dt)
		{
			var velocity = Vector3.Project(_velocityProvider.GetVelocity(_target), _axis).magnitude;
			return velocity * _factor * _axis;
		}
	}
}