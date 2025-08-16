using System.Collections.Generic;
using Core;
using UnityEngine;
using Zenject;

namespace Game.Ground
{
    public class SectorGrassView : MonoBehaviour, IInjectable
    {
        [SerializeField] private List<GameObject> _grassPrefabs;

        [Inject] private PrefabStorage _prefabStorage;

        private readonly List<GameObject> _instantiatedGrass = new();

        public void SetData(SectorData data)
        {
            ClearGrass();

            if (data?.GrassData?.GrassPositions == null || _grassPrefabs == null || _grassPrefabs.Count == 0)
                return;

            foreach (var grassIndex in data.GrassData.GrassPositions)
            {
                InstantiateGrassAt(grassIndex, data);
            }
        }

        private void ClearGrass()
        {
            foreach (var grass in _instantiatedGrass)
            {
                if (grass != null)
                    DestroyImmediate(grass);
            }
            _instantiatedGrass.Clear();
        }

        private void InstantiateGrassAt(Index2 grassIndex, SectorData data)
        {
            // Выбираем случайный префаб из списка
            var randomPrefab = _grassPrefabs[Random.Range(0, _grassPrefabs.Count)];

            // Преобразуем индекс сетки в мировые координаты
            var worldPosition = GridIndexToPosition(grassIndex, data);

            // Инстанцируем префаб
            var grassInstance = _prefabStorage.Instantiate(randomPrefab, transform);
            grassInstance.transform.localPosition = worldPosition;

            // Добавляем небольшую случайную ротацию для разнообразия
            grassInstance.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            // grassInstance.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            _instantiatedGrass.Add(grassInstance);
        }

        private Vector3 GridIndexToPosition(Index2 grassIndex, SectorData data)
        {
            // Размер сектора (предполагаем, что это единица)
            float sectorSize = 1f;
            int subdivs = 1 << data.Density;

            // Преобразуем индекс в относительные координаты (0-1)
            float relX = (float)grassIndex.x / subdivs;
            float relY = (float)grassIndex.y / subdivs;

            // Преобразуем в локальные координаты сектора
            float localX = relX * sectorSize;
            float localZ = relY * sectorSize;

            // Получаем высоту в этой точке
            float height = data.GetHeight(relX, relY);

            return new Vector3(localX, height, localZ);
        }

        private void OnDestroy()
        {
            ClearGrass();
        }
    }
}
