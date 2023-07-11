using System;
using System.Collections.Generic;
using System.Linq;
using Application.Init;
using Core;
using Core.AsyncTask;
using Core.Infrastructure;
using Game.Ground;
using Game.Ground.Services;
using UnityEngine;
using Zenject;

namespace Terrain.Scene.Ground
{
	public class TerrainGroundSceneInstaller : MonoBehaviour, ISceneInstaller
	{
		private DiContainer _container;
		private List<Type> _initableDeps = new();

		[SerializeField] private Transform _root;
		[SerializeField] private RectTransform _uiRoot;
		[SerializeField] private RectTransform _mainCamera;
		[SerializeField] private TerrainGravity _gravity;

		public IAsyncTask Install(DiContainer container)
		{
			_container = container;

			CreateDependencies(container);
			InjectGameObjects(container);
			InitGameObjects();

			return StartLoading().WhenCompleted(InitDependencies);  
		}

		public void Uninstall()
		{
			foreach (IInitable initable in GetAllComponents<IInitable>())
			{
				try
				{
					initable.UnInit();
				}
				catch (Exception e)
				{
					Debug.LogError($"Clear and reinstall:\n{e}");
				}
			}

			
			_root.transform.OfType<Transform>().Select(c => c.gameObject).ForEach(Destroy);
			_initableDeps.Select(t => _container.Resolve(t)).Cast<IInitable>().ForEach(d => d.UnInit());
			_initableDeps.Clear();
			
			_container.UnbindInterfacesTo<HeightDataSource>();
			_container.Unbind<SectorDataProvider>();
			_container.Unbind<SectorControlService>();
			_container.Unbind<PlayerInputReaderService>();
			_container.Unbind<PlayerCharacterControlService>();
			_container.Unbind<TerrainGravity>();
			_container.Unbind<AccelerationService>();
			// _container.Unbind<MovementService>();
		}

		private void CreateDependencies(DiContainer container)
		{
			container.BindInterfacesTo<HeightDataSource>().AsSingle().Lazy();
			container.Bind<SectorDataProvider>().AsSingle().Lazy();
			container.Bind<SectorControlService>().AsSingle().Lazy();
			container.Bind<PlayerInputReaderService>().AsSingle().Lazy();
			container.Bind<PlayerCharacterControlService>().AsSingle().Lazy();
			container.Bind<AccelerationService>().AsSingle().Lazy();
			container.Bind<TerrainGravity>().FromInstance( _gravity ).AsSingle();
			// container.Bind<MovementService>().AsSingle().Lazy();
		}

		private void InjectGameObjects(DiContainer container)
		{
			GetAllComponents<IInjectable>().ForEach(container.Inject);
		}

		private void InitGameObjects()
		{
			GetAllComponents<IInitable>().ForEach(c => c.Init());
		}

		private void InitDependencies()
		{
			InitDependency<SectorControlService>();
			InitDependency<PlayerInputReaderService>();
			InitDependency<PlayerCharacterControlService>();
			InitDependency<TerrainGravity>();
			InitDependency<AccelerationService>();
		}

		private void InitDependency<T>() where T : IInitable
		{
			_container.Resolve<T>().Init();
			_initableDeps.Add(typeof(T));
		} 

		private void UnInitDependency<T>() where T : IInitable
		{
			_container.Resolve<T>().UnInit();
		} 

		private IEnumerable<T> GetAllComponents<T>()
		{
			return _root.GetComponentsInChildren<T>().Concat(_uiRoot.GetComponentsInChildren<T>());
		}

		public Camera GetCamera()
		{
			return null;
		}

		private IAsyncTask StartLoading()
		{
			var startCommand = _container.Instantiate<SequentCommand>();
			startCommand.Add(_container.Instantiate<CreateWorldCommand>(new[] {_root}));
			startCommand.Add(_container.Instantiate<CreateUICommand>(new[] {_uiRoot}));
			return startCommand.Run().ThrowException();
		}
	}
}