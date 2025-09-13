using System.Collections.Generic;
using Core;
using UnityEngine;
using Zenject;

namespace Game.Ground
{
    public class SectorGrassView : MonoBehaviour, IInjectable
    {
        [SerializeField] private List<GameObject> _grassPrefabs;
        [SerializeField] private int _grassCount = 1000;

        [Inject] private PrefabStorage _prefabStorage;

        private readonly List<GameObject> _instantiatedGrass = new();

        public void SetData(SectorData data)
        {
            // Очищаем только если уже есть трава
            if (_instantiatedGrass.Count > 0)
            {
                ClearGrass();
            }

            // Создаем траву только если есть данные о траве
            if (data?.GrassData?.GrassPositions == null || _grassPrefabs == null || _grassPrefabs.Count == 0)
                return;

            // Создаем траву только если в данных есть позиции для травы
            if (data.GrassData.GrassPositions.Count > 0)
            {
                // Используем настраиваемое количество травы, но не больше чем доступно в данных
                int grassToCreate = Mathf.Min(_grassCount, data.GrassData.GrassPositions.Count);
                
                for (int i = 0; i < grassToCreate; i++)
                {
                    InstantiateGrassAt(data.GrassData.GrassPositions[i], data);
                }
            }
        }

        public void ClearGrass()
        {
            foreach (var grass in _instantiatedGrass)
            {
                if (grass != null)
                    DestroyImmediate(grass);
            }
            _instantiatedGrass.Clear();
        }

        private void InstantiateGrassAt(SectorGrassData.GrassPosition grassPosition, SectorData sectorData)
        {
            // Выбираем случайный префаб из списка
            var randomPrefab = _grassPrefabs[Random.Range(0, _grassPrefabs.Count)];

            // Вычисляем высоту непосредственно при инстанцировании
            float height = sectorData.GetHeight(grassPosition.RelativeX, grassPosition.RelativeY);

            // Преобразуем относительные координаты в локальные
            var localPosition = new Vector3(
                grassPosition.RelativeX,
                height,
                grassPosition.RelativeY
            );

            // Инстанцируем префаб
            var grassInstance = _prefabStorage.Instantiate(randomPrefab, transform);
            grassInstance.transform.localPosition = localPosition;

            // Устанавливаем поворот
            grassInstance.transform.rotation = Quaternion.Euler(0, grassPosition.Rotation, 0);

            _instantiatedGrass.Add(grassInstance);
        }

        // Метод больше не нужен, так как мы работаем напрямую с относительными координатами

        private void OnDestroy()
        {
            ClearGrass();
        }
    }
}
