using System;
using System.Linq;
using Core;
using UnityEngine;
using Zenject;
using Application.Init;

namespace Application
{
	public class AppRoot : MonoBehaviour
	{
		public static DiContainer EditorContainer;
		private DiContainer _container;
		private SceneLoader _sceneLoader;

		private void Start()
		{
			Install();

			EditorContainer = _container;
		}

		private void Install()
		{
			_container = new DiContainer();
			_sceneLoader = new SceneLoader(_container);
			
			var installer = new RootInstaller();
			installer.Install(_container);
			
			_container.ResolveRoots();
			_container.Resolve<IResetService>().OnResetRequested += ClearAndReinstall;

			_sceneLoader.Load(SceneLoader.Scenes.Terrain);
		}

		private void ClearAndReinstall()
		{
			Clear();
			Install();
		}

		private void Clear()
		{
			
		}
	}
}