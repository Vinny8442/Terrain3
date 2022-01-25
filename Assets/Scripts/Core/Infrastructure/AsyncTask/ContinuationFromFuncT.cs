using System;
using Core.AsyncTask;

namespace Core.Infrastructure.AsyncTask
{
	public class ContinuationFromFuncT<TResult> : Core.AsyncTask.AsyncTask
	{
		private IAsyncTask<TResult> _prevTask;
		private Func<TResult, IAsyncTask> _nextAction;
		private Action _onComplete;

		public ContinuationFromFuncT(Func<TResult, IAsyncTask> nextAction, IAsyncTask<TResult> prevTask)
		{
			_nextAction = nextAction;
			_prevTask = prevTask;
			
			_prevTask.WhenCompleted(OnControlTaskCompleted);
			_prevTask.WhenFailed(Fail);
		}

		public IAsyncTask Then(Func<IAsyncTask> nextAction)
		{
			return new ContinuationFromFunc(nextAction, this);
		}


		private void OnControlTaskCompleted(TResult result)
		{
			try
			{
				IAsyncTask nextTask = _nextAction?.Invoke(result);
				if (nextTask != null)
				{
					nextTask.WhenCompleted(Complete);
					nextTask.WhenFailed(Fail);
				}
			}
			catch (Exception e)
			{
				Fail(e);
			}
		}
	}
}