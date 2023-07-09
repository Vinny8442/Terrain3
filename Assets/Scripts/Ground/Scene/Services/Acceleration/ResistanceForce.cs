using UnityEngine;

namespace Game.Ground.Services
{
	public class ResistanceForce : AccelerationService.IForceHandle, AccelerationService.IForce
	{
		private AccelerationService.IVelocityDataProvider _velocityProvider;
		private Vector3 _axis;
		private float _factor;
		private AccelerationService.IForce _source;
		private AccelerationService.ITarget _target;

		public ResistanceForce(AccelerationService.IVelocityDataProvider velocityProvider, AccelerationService.ITarget target, float factor, AccelerationService.IForce source)
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