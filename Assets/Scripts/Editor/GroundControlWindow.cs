using Application;
using Game.Ground;
using Terrain.Application.Init;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	public class GroundControlWindow : EditorWindow
	{
		private bool _dependenciesResolved;

		private SectorControlService _sectorControl => AppRoot.EditorContainer.Resolve<SectorControlService>();

		[ MenuItem( @"Terrain/Ground Settings" ) ]
		private static void ShowWindow()
		{
			var window = GetWindow< GroundControlWindow >();
			window.titleContent.text = @"Ground Control Window";
		}
		
		private void OnGUI()
		{
			if (!EditorApplication.isPlaying)
			{
				_dependenciesResolved = false;
				return;
			}

			try
			{
				if (!_dependenciesResolved)
				{
					InitDependencies();
					_dependenciesResolved = true;
				}
			}
			catch
			{
				EditorGUILayout.LabelField("Dependencies not resolved", EditorStyles.centeredGreyMiniLabel);
			}

			if (!_dependenciesResolved) return;

			EditorGUILayout.BeginVertical();
			{
				// if (GUILayout.Button("Rebuild"))
				// {
				// 	_sectorControl.EditorRebuild();
				// }

				if (GUILayout.Button("Up"))
				{
					_sectorControl.EditorMoveCenter(new Index2(0, -1));
				}

				EditorGUILayout.BeginHorizontal();
				{

					if (GUILayout.Button("Left"))
					{
						_sectorControl.EditorMoveCenter(new Index2(-1, 0));
					}

					if (GUILayout.Button("Right"))
					{
						_sectorControl.EditorMoveCenter(new Index2(1, 0));
					}
				}
				EditorGUILayout.EndHorizontal();
				if (GUILayout.Button("Down"))
				{
					_sectorControl.EditorMoveCenter(new Index2(0, 1));
				}
			}
			EditorGUILayout.EndVertical();
		}

		private void InitDependencies()
		{
			// _sectorControl = AppRoot.EditorContainer.Resolve<SectorControlService>();
		}
	}
}