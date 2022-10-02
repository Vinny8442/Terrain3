using Application;
using Application.Init;
using UnityEngine;
using Zenject;

namespace Terrain.Application.Init
{
	public class AppRoot : MonoBehaviour
	{
		private const int BaseLoaderSceneIndex = 1;
		private const int TerrainSceneIndex = 2;

		public static DiContainer EditorContainer;

		[SerializeField] private GameObject _monobehUpdater;

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
			_sceneLoader = new SceneLoader(_container, BaseLoaderSceneIndex);

			var installer = new RootInstaller();
			installer.Install(_container, _monobehUpdater);

			_container.ResolveRoots();
			_container.Resolve<IResetService>().OnResetRequested += ClearAndReinstall;

			_sceneLoader.Load(TerrainSceneIndex);
		}

		private void ClearAndReinstall()
		{
			Clear();
			Install();
		}

		private void Clear()
		{
			_sceneLoader.Clear();
		}
	}
}