using Core.AsyncTask;
using UnityEngine;

namespace Application
{
	public class CreateUICommand : ICommand
	{
		private AsyncTask _task;

		public CreateUICommand(Transform root)
		{
			
		}

		public IAsyncTask Run()
		{
			_task = _task ?? new AsyncTask();
			_task.Complete();
			return _task;
		}

		public bool Completed => true;
	}
}