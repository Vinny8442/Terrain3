using System;
using Core.AsyncTask;

namespace Core.AsyncTask
{
	public struct AsyncTaskAwaiter : ITaskAwaiter
	{
		private readonly IAsyncTask _task;

		public AsyncTaskAwaiter(IAsyncTask task)
		{
			_task = task;
		}
		public void OnCompleted(Action continuation)
		{
			_task.WhenCompleted(continuation).WhenFailed(_ => continuation());
		}

		public bool IsCompleted => _task.IsCompleted;
		
		public void GetResult()
		{
			_task.ThrowException();
		}
	}
	
	public struct AsyncTaskAwaiter<TResult> : ITaskAwaiter<TResult>
	{
		private readonly IAsyncTask<TResult> _task;

		public AsyncTaskAwaiter(IAsyncTask<TResult> task)
		{
			_task = task;
		}
		public void OnCompleted(Action continuation)
		{
			_task.WhenCompleted(continuation).WhenFailed(_ => continuation());
		}

		public bool IsCompleted => _task.IsCompleted;
		
		public TResult GetResult()
		{
			return _task.Result;
		}
	}
}