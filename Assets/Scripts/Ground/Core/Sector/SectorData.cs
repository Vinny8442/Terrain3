using System;

namespace Game.Ground
{
	// public class GroundDataCache
	// {
	// 	private readonly IHeightSource _heightDataSource;
	//
	// 	private Dictionary<Index2, SectorData> _sectors = new Dictionary<Index2, SectorData>();
	// 	public int R = 4;
	//
	// 	public bool TryGetSectorData(Index2 index, int density, out SectorData sectorData)
	// 	{
	// 		if (_sectors.TryGetValue(index, out sectorData))
	// 		{
	// 			if (sectorData.Density < density)
	// 			{
	// 				sectorData.SetDensity(density);
	// 			}
	//
	// 			return true;
	// 		}
	// 		return false;
	// 	}
	//
	// 	public void Add(Index2 index, SectorData data)
	// 	{
	// 		_sectors[index] = data;
	// 	}
	// }

	public class SectorData
	{
		public Index2 Index;
		// public readonly Vector2 Position;

		public readonly int SubDivs;
		public int Density { get; private set; } = 0;

		public readonly float[] Data;

		public bool IsDirty = false;

		// private readonly IHeightSource _heightSource;

		public SectorData(Index2 index, int density, float[] data)
		{
			// _heightSource = heightSource;
			Index = index;
			// Position = position;
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
			// try
			// {
				return Data[i * (SubDivs + 1) + j];
			// }
			// catch (Exception e)
			// {
				// return 0;
			// }
		}
		
	}
}