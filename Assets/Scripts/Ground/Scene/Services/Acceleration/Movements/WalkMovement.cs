using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Ground.Services.Movement
{
	public class WalkMovement : AccelerationService.Movement
		{
			private AccelerationService.Movement _movement;
			private AccelerationService.Movement _idle;
			private readonly List<AccelerationService.Movement> _added = new();
			private AccelerationService.Movement _stopMovement;

			private AccelerationService.Movement _current;

			public WalkMovement(AccelerationService.Movement movement, AccelerationService.Movement idle)
			{
				_current = _idle = idle;
				_movement = movement;
			}
			
			public override Vector3 Update(float dt)
			{
				if (Completed) return Vector3.zero;
				
				var result = _current.Update(dt);

				result += _added.Aggregate(Vector3.zero, (current, m) => current + m.Update(dt));
				_added.RemoveAll(m => m.Completed);

				return result;
			}

			public override void Stop()
			{
				Completed = true;
			}

			public void Stop(AccelerationService.Movement movement)
			{
				// TransitionTo(movement, movement.Duration);
				_stopMovement = movement;
			}

			public void Go()
			{
				_current = new TransitionMovement(_current, _movement, 0.05f);
			}

			public void Idle()
			{
				_current = new TransitionMovement(_current, _idle, 0.2f);
			}

			public void Parallel(AccelerationService.Movement movement)
			{
				_added.Add(movement);
			}

			public override void Rotate(Quaternion value)
			{
				_added.ForEach(m => m.Rotate(value));
				_idle.Rotate(value);
				_movement.Rotate(value);
			}
		}
}