using System;
using System.Linq;
using Core;
using UnityEngine;
using Zenject;

namespace Application
{
	public class AppRoot : MonoBehaviour
	{
		public static DiContainer EditorContainer;
		
		[SerializeField] private Transform _root;
		[SerializeField] private RectTransform _uiRoot;
		private DiContainer _container;

		private void Start()
		{
			Install();
			StartLoading();

			EditorContainer = _container;
		}

		private void Install()
		{
			_container = new DiContainer();
			var installer = new RootInstaller();
			installer.Install(_container);
			
			_container.ResolveRoots();
			
			_container.Resolve<IResetService>().OnResetRequested += ClearAndReinstall;
		}

		private void StartLoading()
		{
			var startCommand = _container.Instantiate<SequentCommand>();
			startCommand.Add(_container.Instantiate<CreateWorldCommand>(new []{_root}));
			startCommand.Add(_container.Instantiate<CreateUICommand>(new []{_uiRoot}));
			startCommand.Run().ThrowException();
		}

		private void ClearAndReinstall()
		{
			Clear();
			Install();
			StartLoading();
		}

		private void Clear()
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
		}
	}
}