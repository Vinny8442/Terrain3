using System.Collections.Generic;
using System.Linq;

namespace Game.Ground
{
    public class SectorGrassData
    {
        private readonly float[] _heightData;
        private readonly int _subdivs;
        private readonly List<Index2> _data = new();

        public IReadOnlyList<Index2> GrassPositions => _data;

        public SectorGrassData(float[] heightData, int density)
        {
            _heightData = heightData;
            _subdivs = 1 << density;
            if (density == SectorData.MaxDensity)
            {
                CreateGrassData();
            }
        }

        private void CreateGrassData()
        {
            foreach (var index in EnumerateIndexes())
            {
                if (index.x - 1 < 0 || index.x + 1 >= _subdivs || index.y - 1 < 0 || index.y + 1 >= _subdivs) continue;
                var h = GetHeight(index.x, index.y);
                if ((h < GetHeight(index.x - 1, index.y) && h < GetHeight(index.x + 1, index.y)) ||
                    (h < GetHeight(index.x, index.y - 1) && h < GetHeight(index.x, index.y + 1))) {
                    _data.Add(index);
                }
            }
        }

        private IEnumerable<Index2> EnumerateIndexes() =>
            from i in Enumerable.Range(0, _subdivs)
            from j in Enumerable.Range(0, _subdivs)
            select new Index2(i, j);


        float GetHeight(int x, int y) => _heightData[y * _subdivs + x];
    }
}
