using System.Collections.Generic;
using UnityEngine;

namespace Game.Ground
{
    public class SectorTreesData
    {
        private readonly float[] _heightData;
        private readonly int _subdivs;
        private readonly int _density;
        private readonly int _sectorHash;
        private List<TreePosition> _data;

        private const int TreeCount = 15; // Захардкоженное количество деревьев

        public IReadOnlyList<TreePosition> TreePositions => _data ??= CreateTreesData();

        public SectorTreesData(float[] heightData, int density, int sectorHash = 0)
        {
            _heightData = heightData;
            _density = density;
            _subdivs = 1 << density;
            _sectorHash = sectorHash;
        }

        private List<TreePosition> CreateTreesData()
        {
            var result = new List<TreePosition>();
            
            // Создаем деревья только для секторов с максимальной плотностью
            if (_density != SectorData.MaxDensity)
                return result;

            // Используем фиксированный seed на основе позиции сектора для воспроизводимости
            var random = new System.Random(_sectorHash);
            
            for (int i = 0; i < TreeCount; i++)
            {
                // Генерируем случайную позицию в пределах сектора
                float relX = (float)random.NextDouble();
                float relY = (float)random.NextDouble();
                
                // Получаем высоту в этой точке
                float height = GetHeightAtPosition(relX, relY);
                
                // Случайный поворот
                float rotation = (float)(random.NextDouble() * 360.0);
                
                result.Add(new TreePosition(relX, relY, height, rotation));
            }

            return result;
        }

        private float GetHeightAtPosition(float relX, float relY)
        {
            // Интерполируем высоту из сетки высот
            float x = relX * _subdivs;
            float y = relY * _subdivs;
            
            int x0 = Mathf.FloorToInt(x);
            int y0 = Mathf.FloorToInt(y);
            int x1 = Mathf.Min(x0 + 1, _subdivs);
            int y1 = Mathf.Min(y0 + 1, _subdivs);
            
            float fx = x - x0;
            float fy = y - y0;
            
            float h00 = GetHeight(x0, y0);
            float h10 = GetHeight(x1, y0);
            float h01 = GetHeight(x0, y1);
            float h11 = GetHeight(x1, y1);
            
            // Билинейная интерполяция
            float h0 = Mathf.Lerp(h00, h10, fx);
            float h1 = Mathf.Lerp(h01, h11, fx);
            
            return Mathf.Lerp(h0, h1, fy);
        }

        private float GetHeight(int x, int y) => _heightData[y * (_subdivs + 1) + x];

        public readonly struct TreePosition
        {
            public readonly float RelativeX;
            public readonly float RelativeY;
            public readonly float Height;
            public readonly float Rotation;

            public TreePosition(float relativeX, float relativeY, float height, float rotation)
            {
                RelativeX = relativeX;
                RelativeY = relativeY;
                Height = height;
                Rotation = rotation;
            }
        }
    }
}
