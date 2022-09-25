using System;
using System.Linq;
using Application.Init;
using Core;
using Core.AsyncTask;
using Game.Ground;
using Game.Sector;
using UnityEngine;
using Zenject;

namespace Application
{
    public class TerrainInstaller : MonoBehaviour, ISceneInstaller
    {
        private DiContainer _container;
        
        [SerializeField] private Transform _root;
        [SerializeField] private RectTransform _uiRoot;
        [SerializeField] private RectTransform _mainCamera;
        
        public IAsyncTask Install(DiContainer container)
        {
            _container = container;
            
            container.BindInterfacesTo<HeightDataSource>().AsSingle().Lazy();
            container.Bind<SectorDataProvider>().AsSingle().Lazy();
            container.Bind<SectorControlService>().AsSingle().Lazy();
            
            return StartLoading();
        }

        public void Uninstall()
        {
            foreach ( IInitable initable in _root.GetComponentsInChildren<IInitable>().Concat( _uiRoot.GetComponentsInChildren<IInitable>() ) )
            {
                try
                {
                    initable.UnInit();
                }
                catch ( Exception e )
                {
                    Debug.LogError( $"Clear and reinstall:\n{e}" );
                }
            }

            foreach ( Transform child in _root.transform )
            {
                Destroy( child.gameObject );
            }

            _container.UnbindInterfacesTo<HeightDataSource>();
            _container.Unbind<SectorDataProvider>();
            _container.Unbind<SectorControlService>();
        }

        public Camera GetCamera()
        {
            return null;
        }

        private IAsyncTask StartLoading()
        {
            var startCommand = _container.Instantiate<SequentCommand>();
            startCommand.Add(_container.Instantiate<CreateWorldCommand>(new []{_root}));
            startCommand.Add(_container.Instantiate<CreateUICommand>(new []{_uiRoot}));
            return startCommand.Run().ThrowException();
        }
    }
}