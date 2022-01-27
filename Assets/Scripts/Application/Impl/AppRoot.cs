using UnityEngine;
using Zenject;

namespace Application
{
	public class AppRoot : MonoBehaviour
	{
		public static DiContainer EditorContainer;
		
		[SerializeField] private Transform _root;
		[SerializeField] private RectTransform _uiRoot;
		private void Start()
		{
			var container = new DiContainer();
			var installer = new RootInstaller();
			installer.Install(container);
			
			container.ResolveRoots();

			var startCommand = container.Instantiate<SequentCommand>();
			startCommand.Add(container.Instantiate<CreateWorldCommand>(new []{_root}));
			startCommand.Add(container.Instantiate<CreateUICommand>(new []{_uiRoot}));
			startCommand.Run().ThrowException();

			EditorContainer = container;
		}
	}
}