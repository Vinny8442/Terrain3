using System;
using Core.Infrastructure.AsyncTask;

namespace Core.AsyncTask
{
	public class AsyncTask<TResult> : AsyncTask, IAsyncTask<TResult>
	{
		private Action<TResult> _onCompletedT = delegate { };

		public virtual void Complete(TResult result)
		{
			if (!IsCompleted)
			{
				Result = result;
				IsCompleted = true;
				_onCompleted?.Invoke();
				_onCompletedT?.Invoke(result);
			}
		}

		public IAsyncTask WhenCompleted(Action<TResult> onCompleted)
		{
			if (IsCompleted)
				onCompleted?.Invoke(Result);
			else
				_onCompletedT += onCompleted;

			return ToUntypedTask(this);
		}

		private static async IAsyncTask ToUntypedTask(AsyncTask<TResult> task)
		{
			await task;
		}

		public IAsyncTask Then(Func<TResult, IAsyncTask> nextAction)
		{
			return new ContinuationFromFuncT<TResult>(nextAction, this);
		}

		public TResult Result { get; private set; }

		public AsyncTaskAwaiter<TResult> GetAwaiter()
		{
			return new AsyncTaskAwaiter<TResult>(this);
		}
	}
}