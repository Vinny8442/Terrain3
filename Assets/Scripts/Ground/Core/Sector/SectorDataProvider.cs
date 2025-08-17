using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.AsyncTask;
using Core.Infrastructure.AsyncTask;
using UnityEngine;

namespace Game.Ground
{
	public class SectorDataProvider
	{
		private readonly IHeightSource _heightSource;
		private readonly SectorDataCache _cache = new();

		public SectorDataProvider(IHeightSource heightSource)
		{
			_heightSource = heightSource;
        }

		public async Task<IEnumerable<SectorData>> RequestSectorData(List<SectorRequest> requests)
		{
			List<SectorData> result = new(requests.Count);
			List<IAsyncTask<SectorData>> tasks = new List<IAsyncTask<SectorData>>();

			foreach (SectorRequest sectorRequest in requests)
			{
				if (_cache.TryGetCachedData(sectorRequest.Index, sectorRequest.Density, out SectorData cachedData))
				{
					result.Add(cachedData);
				}
				else
				{
					var task = new ThreadAsyncTask<SectorData>(() => GenerateSectorData(sectorRequest), CancellationToken.None);
					tasks.Add(task);
				}
			}

			if (tasks.Count > 0)
			{
				IEnumerable<SectorData> generatedData = await TaskUtils.WaitAll(tasks, CancellationToken.None);

				foreach (SectorData sectorData in generatedData)
				{
					_cache.CacheData(sectorData);
					result.Add(sectorData);
				}
			}

			return result;
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
			float gx = (float) i / (SubDivs) * Size.x + Position.x;
			float gy = (float) j / (SubDivs) * Size.y + Position.y;
			float result = _heightSource.GetHeight(gx, gy);
			return result;
		}

		public void ClearCache()
		{
			_cache.Clear();
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
