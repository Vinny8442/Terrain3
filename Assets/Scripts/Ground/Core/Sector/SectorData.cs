using System;
using UnityEditor.UI;

namespace Game.Ground
{
	public class SectorData
	{
		public const int MaxDensity = 7;

		public Index2 Index;

        private readonly int _subDivs;
		public int Density { get; private set; } = 0;
        public readonly SectorGrassData GrassData;
        public readonly SectorTreesData TreesData;

        private readonly float[] _data;

		public SectorData(Index2 index, int density, float[] data)
		{
			Index = index;
			Density = density;
            GrassData = new(data, Density);
            TreesData = new(data, Density, Index.GetHashCode());
			_subDivs = 1 << density;
			_data = data;

		}


        public float GetHeight(float relX, float relY)
		{
			if (_data == null)
			{
				throw new Exception($"SectorData I:{Index} D:{Density}: Data not generated yet!");
			}
			int i = (int) (relX * _subDivs);
			int j = (int) (relY * _subDivs);
			return _data[i * (_subDivs + 1) + j];
		}

	}
}
