using UnityEngine;

namespace Game.Ground
{
	public class PerlinNoise : INoiseSource
	{
		private readonly Vector2 _offset;
		private readonly Vector2 _scale;
		private readonly float _amplitude;

		public PerlinNoise(PerlinNoiseSettings settings)
		{
			_offset = settings.Offset;
			_scale = new Vector2(settings.Scale, settings.Scale);
			_amplitude = settings.Amplitude;
		}
		
		public float GetValue(float x, float y)
		{
			return Mathf.PerlinNoise((x + _offset.x) / _scale.x, (y + _offset.y) / _scale.y) * _amplitude;
		}
	}
}