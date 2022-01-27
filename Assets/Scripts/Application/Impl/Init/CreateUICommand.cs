using Core;
using Core.AsyncTask;
using UnityEngine;

namespace Application
{
	public class CreateUICommand : ICommand
	{
		private AsyncTask _task;
		private readonly PrefabStorage _prefabStorage;
		private readonly RectTransform _uiRoot;

		public CreateUICommand(RectTransform uiRoot, PrefabStorage prefabStorage)
		{
			_uiRoot = uiRoot;
			_prefabStorage = prefabStorage;
		}

		public async IAsyncTask Run()
		{
			try
			{
				_prefabStorage.InjectAndInit( _uiRoot.gameObject );
			}
			finally
			{
				Completed = true;
			}
		}

		public bool Completed {get; private set;}
	}
}