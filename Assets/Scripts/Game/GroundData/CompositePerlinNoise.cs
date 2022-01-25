using System.Collections.Generic;

namespace Game.Ground
{
	public class CompositePerlinNoise : INoiseSource
	{
		private readonly List<INoiseSource> _components = new List<INoiseSource>();

		public CompositePerlinNoise(IEnumerable<PerlinNoiseSettings> noises)
		{
			foreach (var noiseSettings in noises)
			{
				Add(new PerlinNoise(noiseSettings));
			}
		}
		public float GetValue(float x, float y)
		{
			float result = 0;
			foreach (INoiseSource component in _components)
			{
				result += component.GetValue(x, y);
			}

			return result;
		}

		public void Add(INoiseSource component)
		{
			_components.Add(component);
		}

		public void Clear()
		{
			_components.Clear();
		}
	}
}