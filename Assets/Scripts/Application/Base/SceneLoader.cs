using System;
using System.Collections.Generic;
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
        public enum Scenes : int
        {
            BasePreloader = 1,
            Terrain = 2,
        }
        
        private bool _isBusy = false;
        private AsyncTask _loadingTask = null;
        private Scenes? _sceneBeingLoaded = null;
        private Scene? _currentScene = null;
        private readonly DiContainer _container;
        private readonly Dictionary<Scenes, Scenes> _uiScenes = null;

        public bool IsBusy => _loadingTask != null;
        public IAsyncTask Loading => _loadingTask;

        public SceneLoader(DiContainer container)
        {
            _container = container;
        }

        public IAsyncTask Load(Scenes scene)
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

            _currentScene = SceneManager.GetSceneByBuildIndex((int) _sceneBeingLoaded);
            var gameObjects = _currentScene?.GetRootGameObjects();
            gameObjects?.
                SelectMany(o => o.GetComponentsInChildren<ISceneInstaller>()).
                ForEach(installer => installer.Install(_container));
                
            currentLoading.Complete();
        }

        private void UnloadCurrentScene()
        {
            if (_currentScene.HasValue)
            {
                var gameObjects = _currentScene.Value.GetRootGameObjects();
                gameObjects?.
                    SelectMany(o => o.GetComponentsInChildren<ISceneInstaller>()).
                    ForEach(installer => installer.Install(_container));
            }
        }

        private void LoadNewScene(Scenes scene)
        {
            var operation = SceneManager.LoadSceneAsync((int) scene, LoadSceneMode.Additive);
            _sceneBeingLoaded = scene;
            operation.completed += _handleSceneLoadingCompleted;
            _loadingTask = new AsyncTask();
        }
        
    }
}