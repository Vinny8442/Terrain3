using System;

namespace Core.AsyncTask
{
	public class AsyncTaskControl
	{
		private AsyncTask _task;
		public IAsyncTask Task => _task ?? (_task = new AsyncTask()); 
		
		public void Complete() => _task?.Complete();

		public void Fail(Exception exception) => _task?.Fail(exception);

	}
}