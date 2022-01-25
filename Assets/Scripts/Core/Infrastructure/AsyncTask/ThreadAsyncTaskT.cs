using System;
using System.Threading;
using System.Threading.Tasks;
using Core.AsyncTask;

namespace Core.Infrastructure.AsyncTask
{
	public class ThreadAsyncTask<TResult> : AsyncTask<TResult>
	{
		private Task<TResult> _innerTask;
		
		public ThreadAsyncTask(Func<TResult> action, CancellationToken token)
		{
			Wait(action, token);
		}

		private async void Wait(Func<TResult> action, CancellationToken token)
		{
			try
			{
				_innerTask = Task.Run(action, token);
				await _innerTask;
				CompleteInternal(_innerTask.Result);
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

		private void CompleteInternal(TResult result)
		{
			base.Complete(result);
		}

		public override void Complete(TResult result)
		{
			throw new Exception("Can't complete ThreadAsyncTask manually");
		}

		public override void Fail(Exception exception)
		{
			throw new Exception("Can't fail ThreadAsyncTask manually");
		}
	}
}