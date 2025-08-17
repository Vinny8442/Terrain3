using System.Collections.Generic;
using Core;
using UnityEngine;
using Zenject;

namespace Game.Ground
{
    public class SectorTreesView : MonoBehaviour, IInjectable
    {
        [SerializeField] private List<GameObject> _treePrefabs;

        [Inject] private PrefabStorage _prefabStorage;

        private readonly List<GameObject> _instantiatedTrees = new();

        public void SetData(SectorData data)
        {
            // Очищаем только если уже есть деревья
            if (_instantiatedTrees.Count > 0)
            {
                ClearTrees();
            }

            // Создаем деревья только если есть данные и префабы
            if (data?.TreesData?.TreePositions == null || _treePrefabs == null || _treePrefabs.Count == 0)
                return;

            // Создаем деревья только если в данных есть позиции
            if (data.TreesData.TreePositions.Count > 0)
            {
                foreach (var treePosition in data.TreesData.TreePositions)
                {
                    InstantiateTreeAt(treePosition);
                }
            }
        }

        public void ClearTrees()
        {
            foreach (var tree in _instantiatedTrees)
            {
                if (tree != null)
                    DestroyImmediate(tree);
            }
            _instantiatedTrees.Clear();
        }

        private void InstantiateTreeAt(SectorTreesData.TreePosition treePosition)
        {
            // Выбираем случайный префаб из списка
            var randomPrefab = _treePrefabs[Random.Range(0, _treePrefabs.Count)];

            // Преобразуем относительные координаты в локальные
            var localPosition = new Vector3(
                treePosition.RelativeX,
                treePosition.Height,
                treePosition.RelativeY
            );

            // Инстанцируем префаб
            var treeInstance = _prefabStorage.Instantiate(randomPrefab, transform);
            treeInstance.transform.localPosition = localPosition;

            // Устанавливаем поворот
            treeInstance.transform.rotation = Quaternion.Euler(0, treePosition.Rotation, 0);
            treeInstance.transform.localScale = Vector3.one * 0.01f;

            _instantiatedTrees.Add(treeInstance);
        }

        private void OnDestroy()
        {
            ClearTrees();
        }
    }
}
