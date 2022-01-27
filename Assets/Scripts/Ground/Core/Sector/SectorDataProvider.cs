using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.AsyncTask;
using Core.Infrastructure.AsyncTask;
using Game.Ground;
using Game.Infrastructure;
using UnityEngine;

namespace Game.Sector
{
	public class SectorDataProvider
	{
		private IHeightSource _heightSource;

		public SectorDataProvider(IHeightSource heightSource)
		{
			_heightSource = heightSource;
		}
		
		public async Task<IEnumerable<SectorData>> RequestSectorData(List<SectorRequest> requests)
		{
			int index = 0;
			List<IAsyncTask<SectorData>> tasks = new List<IAsyncTask<SectorData>>(requests.Count);
			foreach (SectorRequest sectorRequest in requests)
			{
				var task = new ThreadAsyncTask<SectorData>(() => GenerateSectorData(sectorRequest), CancellationToken.None);
				tasks.Add(task);
			}

			return await TaskUtils.WaitAll(tasks, CancellationToken.None);
		}

		private SectorData GenerateSectorData(SectorRequest request)
		{
			int subDivs = 1 << request.Density;
			var data = new float[(subDivs + 1) * (subDivs + 1)];
			int index = 0;
			for (int i = 0; i <= subDivs; i++)
			{
				for (int j = 0; j <= subDivs; j++, index++)
				{
					data[index] = GetHeightInternal(i, j, request.Offset, request.Scale, subDivs);
				}
			}
			return new SectorData(request.Index, request.Density, data);
		}

		private float GetHeightInternal(int i, int j, Vector2 Position, Vector2 Size, int SubDivs)
		{
			// return 0;
			// Vector2 relPosition = new Vector2((float) i / (SubDivs + 1), (float) j / (SubDivs + 1));
			// Vector2 localPosition = Size * relPosition;
			// Vector2 globalPosition = Position + localPosition - Size / 2;
			float gx = (float) i / (SubDivs) * Size.x + Position.x; 
			float gy = (float) j / (SubDivs) * Size.y + Position.y;
			float result = _heightSource.GetHeight(gx, gy);
			return result; 
		}

		public readonly struct SectorRequest
		{
			public readonly Index2 Index;
			public readonly int Density;
			public readonly Vector2 Offset;
			public readonly Vector2 Scale;

			public SectorRequest(Index2 index, int density, Vector2 offset, Vector2 scale)
			{
				Index = index;
				Density = density;
				Offset = offset;
				Scale = scale;
			}
		}
	}
}