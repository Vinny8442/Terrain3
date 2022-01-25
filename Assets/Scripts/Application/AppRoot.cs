using UnityEngine;
using Zenject;

namespace Application
{
	public class AppRoot : MonoBehaviour
	{
		public static DiContainer EditorContainer;
		
		[SerializeField] private Transform _root;
		private void Start()
		{
			var container = new DiContainer();
			var installer = new RootInstaller();
			installer.Install(container);
			
			container.ResolveRoots();

			var startCommand = container.Instantiate<SequentCommand>();
			startCommand.Add(container.Instantiate<CreateWorldCommand>(new []{_root}));
			startCommand.Add(container.Instantiate<CreateUICommand>(new []{_root}));
			startCommand.Run().ThrowException();

			EditorContainer = container;
		}
	}
}