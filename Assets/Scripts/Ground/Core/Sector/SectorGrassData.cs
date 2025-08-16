using System.Collections.Generic;
using System.Linq;

namespace Game.Ground
{
    public class SectorGrassData
    {
        private readonly float[] _heightData;
        private readonly int _subdivs;
        private readonly int _density;
        private List<Index2> _data;

        public IReadOnlyList<Index2> GrassPositions => _data ??= CreateGrassData();

        public SectorGrassData(float[] heightData, int density)
        {
            _heightData = heightData;
            _density = density;
            _subdivs = 1 << density;
        }

        private List<Index2> CreateGrassData()
        {
            var result = new List<Index2>();
            foreach (var index in EnumerateIndexes())
            {
                if (index.x - 1 < 0 || index.x + 1 >= _subdivs || index.y - 1 < 0 || index.y + 1 >= _subdivs) continue;
                var h = GetHeight(index.x, index.y);
                if ((h < GetHeight(index.x - 1, index.y) && h < GetHeight(index.x + 1, index.y)) ||
                    (h < GetHeight(index.x, index.y - 1) && h < GetHeight(index.x, index.y + 1))) {
                    result.Add(index);
                }
            }

            return result;
        }

        private IEnumerable<Index2> EnumerateIndexes() =>
            from i in Enumerable.Range(0, _subdivs)
            from j in Enumerable.Range(0, _subdivs)
            select new Index2(i, j);


        float GetHeight(int x, int y) => _heightData[y * _subdivs + x];
    }
}
