using Core;
using UnityEngine;

namespace Game.Ground
{
	public class SectorView : NGrid, IInitable
	{
		[SerializeField] private MeshCollider _collider;
		[SerializeField] private SectorGrassView _grassView;
		[SerializeField] private SectorTreesView _treesView;
		public Index2 Index { get; private set; }

		public Index2 DataIndex { get; private set; }
		public int Density { get; private set; }
		public int SubDivs { get; private set; }
		public SectorData Data { get; private set; }

		public bool Interactable { get; private set; }
		private bool _isCentral = false;

		public void Init()
		{
			Index = new Index2();
			DataIndex = new Index2();
			// Density = 0;
		}

		public void UnInit()
		{
		}

		public void SetInteractable(bool value)
		{
			Interactable = value;
		}

		public void Rebuild()
		{
			Generate(SubDivs, SubDivs);
		}

		public void RebuildCollider()
		{
			_collider.sharedMesh = Interactable ? Mesh : null;
		}

		protected override float GetHeight(int x, int y)
		{
			var height = Data.GetHeight((float) x / SubDivs, (float) y / SubDivs);
			return height;
		}


		public void SetIndex(Index2 index)
		{
			Index = index;
			// name = $"Grid({index.x}:{index.y})";
		}

		public void SetData(int density, SectorData data)
		{
			Density = density;
			Data = data;
			SubDivs = 1 << density;
			DataIndex = data.Index;

			// Определяем, является ли сектор центральным (Index == (0,0))
			bool newIsCentral = Index is { x: 0, y: 0 };

			// Если сектор перестал быть центральным, очищаем траву
			if (_isCentral && !newIsCentral)
			{
				_grassView.ClearGrass();
			}

			_isCentral = newIsCentral;

			// Устанавливаем данные в grass view только если это центральный сектор
			if (_isCentral)
			{
				_grassView.SetData(data);
			}

			// Деревья отображаются только для секторов с максимальной плотностью
			if (density == SectorData.MaxDensity)
			{
				_treesView.SetData(data);
			}
			else
			{
				_treesView.ClearTrees();
			}
        }

	}
}
