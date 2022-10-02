using System;
using System.Linq;
using Core.AsyncTask;
using Core.Infrastructure;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Application.Init
{
	public class SceneLoader
	{
		private int _loaderSceneIndex;
		private AsyncTask _loadingTask = null;
		private int? _sceneBeingLoaded = null;
		private Scene? _currentScene = null;

		private readonly DiContainer _container;
		// private readonly Dictionary<Scenes, Scenes> _uiScenes = null;

		public bool IsBusy => _loadingTask != null;
		public IAsyncTask Loading => _loadingTask;

		public SceneLoader(DiContainer container, int loaderSceneIndex)
		{
			_loaderSceneIndex = loaderSceneIndex;
			_container = container;
		}

		public IAsyncTask Load(int scene)
		{
			if (_loadingTask != null)
			{
				throw new Exception("SceneLoader is busy");
			}

			UnloadCurrentScene();

			LoadNewScene(scene);

			return _loadingTask;
		}

		private void _handleSceneLoadingCompleted(AsyncOperation operation)
		{
			var currentLoading = _loadingTask;
			_loadingTask = null;

			_currentScene = SceneManager.GetSceneByBuildIndex(_sceneBeingLoaded.Value);
			var gameObjects = _currentScene?.GetRootGameObjects();
			gameObjects?.SelectMany(o => o.GetComponentsInChildren<ISceneInstaller>())
				.ForEach(installer => installer.Install(_container));

			currentLoading.Complete();
		}

		private void UnloadCurrentScene()
		{
			if (_currentScene.HasValue)
			{
				var gameObjects = _currentScene.Value.GetRootGameObjects();
				gameObjects?.SelectMany(o => o.GetComponentsInChildren<ISceneInstaller>())
					.ForEach(installer => installer.Uninstall());

				SceneManager.UnloadSceneAsync(_currentScene.Value);
				_currentScene = null;
			}
		}

		private void LoadNewScene(int scene)
		{
			var operation = SceneManager.LoadSceneAsync((int) scene, LoadSceneMode.Additive);
			_sceneBeingLoaded = scene;
			operation.completed += _handleSceneLoadingCompleted;
			_loadingTask = new AsyncTask();
		}

		public void Clear()
		{
			UnloadCurrentScene();
		}
	}
}