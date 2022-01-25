using UnityEngine;

namespace Game.Ground
{
	public interface IHeightSource
	{
		float GetHeight(float x, float y);
	}
}