using UnityEngine;

namespace Game.Ground.Services.Movement
{
	public interface IMovable
	{
		void ApplyMove(Vector3 movement);
	}

	public interface IDirection
	{
		void Rotate(Quaternion value);
	}
}