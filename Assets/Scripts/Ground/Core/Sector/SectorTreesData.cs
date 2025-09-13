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

        private const int TreeCount = 450; // Захардкоженное количество деревьев

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
                // Генерируем случайную позицию точно на узлах сетки
                int gridX = random.Next(0, _subdivs + 1);
                int gridY = random.Next(0, _subdivs + 1);

                // Преобразуем в относительные координаты
                float relX = (float)gridX / _subdivs;
                float relY = (float)gridY / _subdivs;

                // Случайный поворот
                float rotation = (float)(random.NextDouble() * 360.0);

                result.Add(new TreePosition(relX, relY, rotation));
            }

            return result;
        }

        public readonly struct TreePosition
        {
            public readonly float RelativeX;
            public readonly float RelativeY;
            public readonly float Rotation;

            public TreePosition(float relativeX, float relativeY, float rotation)
            {
                RelativeX = relativeX;
                RelativeY = relativeY;
                Rotation = rotation;
            }
        }
    }
}
