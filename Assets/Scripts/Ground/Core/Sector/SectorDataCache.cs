using System.Collections.Generic;

namespace Game.Ground
{
    internal class SectorDataCache
    {
        private readonly Dictionary<Index2, SectorData> _cache = new();

        public bool TryGetCachedData(Index2 index, int requiredDensity, out SectorData cachedData)
        {
            if (_cache.TryGetValue(index, out cachedData))
            {
                return cachedData.Density >= requiredDensity;
            }

            cachedData = null;
            return false;
        }

        public void CacheData(SectorData sectorData)
        {
            _cache[sectorData.Index] = sectorData;
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public void RemoveFromCache(Index2 index)
        {
            _cache.Remove(index);
        }

        public int Count => _cache.Count;
    }
}
