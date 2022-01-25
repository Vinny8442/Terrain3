using Gui.Game.Ground;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(NGrid))]
	public class NGridEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Generate")) GenerateGrid(target as NGrid);
		}

		private void GenerateGrid(NGrid grid)
		{
			// grid.Generate();
		}
	}
}