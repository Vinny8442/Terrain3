using System.Collections.Generic;
using UnityEngine;

namespace Game.Ground
{
    public class SectorGrassData
    {
        private readonly float[] _heightData;
        private readonly int _subdivs;
        private readonly int _density;
        private readonly int _sectorHash;
        private readonly int _grassCount;
        private List<GrassPosition> _data;

        public IReadOnlyList<GrassPosition> GrassPositions => _data ??= CreateGrassData();

        public SectorGrassData(float[] heightData, int density, int grassCount, int sectorHash = 0)
        {
            _heightData = heightData;
            _density = density;
            _subdivs = 1 << density;
            _sectorHash = sectorHash;
            _grassCount = grassCount;
        }

        private List<GrassPosition> CreateGrassData()
        {
            var result = new List<GrassPosition>();

            // Создаем траву только для секторов с максимальной плотностью
            if (_density != SectorData.MaxDensity)
                return result;

            // Используем фиксированный seed на основе позиции сектора для воспроизводимости
            var random = new System.Random(_sectorHash);

            for (int i = 0; i < _grassCount; i++)
            {
                // Генерируем случайную позицию точно на узлах сетки
                int gridX = random.Next(0, _subdivs + 1);
                int gridY = random.Next(0, _subdivs + 1);

                // Преобразуем в относительные координаты
                float relX = (float)gridX / _subdivs;
                float relY = (float)gridY / _subdivs;

                // Случайный поворот
                float rotation = (float)(random.NextDouble() * 360.0);

                result.Add(new GrassPosition(relX, relY, rotation));
            }

            return result;
        }

        public readonly struct GrassPosition
        {
            public readonly float RelativeX;
            public readonly float RelativeY;
            public readonly float Rotation;

            public GrassPosition(float relativeX, float relativeY, float rotation)
            {
                RelativeX = relativeX;
                RelativeY = relativeY;
                Rotation = rotation;
            }
        }
    }
}
