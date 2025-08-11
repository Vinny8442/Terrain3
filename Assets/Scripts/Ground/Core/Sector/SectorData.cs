using System;

namespace Game.Ground
{
	public class SectorData
	{
		public Index2 Index;

        private readonly int SubDivs;
		public int Density { get; private set; } = 0;

        private readonly float[] Data;

		public SectorData(Index2 index, int density, float[] data)
		{
			Index = index;
			SubDivs = 1 << density;
			Density = density;
			Data = data;
		}

		public float GetHeight(float relX, float relY)
		{
			if (Data == null)
			{
				throw new Exception($"SectorData I:{Index} D:{Density}: Data not generated yet!");
			}
			int i = (int) (relX * SubDivs);
			int j = (int) (relY * SubDivs);
			return Data[i * (SubDivs + 1) + j];
		}

	}
}
