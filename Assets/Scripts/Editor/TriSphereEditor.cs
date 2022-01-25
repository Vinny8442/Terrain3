using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(TriangleSphere))]
	public class TriSphereEditor : UnityEditor.Editor
	{
		private int _subDivs = 2;
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var sphere = (TriangleSphere) target;
			
			var subDivs = EditorGUILayout.IntSlider("SubDivs", _subDivs, 0, 10); 
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