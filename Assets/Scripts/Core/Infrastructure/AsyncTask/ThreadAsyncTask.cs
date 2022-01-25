using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.AsyncTask
{
	public class ThreadAsyncTask : AsyncTask
	{
		private Task _innerTask;

		public ThreadAsyncTask(Action action, CancellationToken token)
		{
			Wait(action, token);
		}

		private async void Wait(Action action, CancellationToken token)
		{
			try
			{
				_innerTask = Task.Run(action, token);
				await _innerTask;
				CompleteInternal();
			}
			catch (Exception e)
			{
				FailInternal(e);
			}
		}

		private void FailInternal(Exception exception)
		{
			base.Fail(exception);
		}

		private void CompleteInternal()
		{
			base.Complete();
		}

		public override void Complete()
		{
			throw new Exception("Can't complete ThreadAsyncTask manually");
		}

		public override void Fail(Exception exception)
		{
			throw new Exception("Can't fail ThreadAsyncTask manually");
		}
	}
}