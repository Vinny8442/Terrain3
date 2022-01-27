using System;
using UnityEngine;

namespace Game.Ground
{
	[Serializable]
	public class PerlinNoiseSettings
	{
		public float Scale = 1;
		public Vector2 Offset = Vector2.zero;
		public float Amplitude = 1;
	}
}