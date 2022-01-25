using Core;

namespace Game.Ground
{
	public class HeightDataSource : IHeightSource
	{
		private INoiseSource _noiseSource;

		public HeightDataSource(SettingsStorage settingsService)
		{
			_noiseSource = new CompositePerlinNoise(settingsService.Load<GroundSettings>("GroundSettings").Noises);
		}
		
		public float GetHeight(float x, float y)
		{
			float result = _noiseSource.GetValue(x, y);
			return result;
		}

		public void Reset(INoiseSource noiseSource)
		{
			_noiseSource = noiseSource;
		}
	}
}