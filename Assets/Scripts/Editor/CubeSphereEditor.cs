using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(CubeSphere))]
	public class CubeSphereEditor : UnityEditor.Editor
	{
		private int _subDivs = 10;
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var sphere = (CubeSphere) target;
			
			var subDivs = EditorGUILayout.IntSlider("SubDivs", _subDivs, 2, 255);
			if (subDivs != _subDivs)
			{
				_subDivs = subDivs;
				sphere.SetSubDivs(_subDivs);
			}

			if (GUILayout.Button("Test"))
			{
				sphere.Test();
			}
		}
	}
}