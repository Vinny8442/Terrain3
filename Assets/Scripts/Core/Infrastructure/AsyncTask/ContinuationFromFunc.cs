using System;

namespace Core.AsyncTask
{
	public class ContinuationFromFunc : AsyncTask
	{
		private IAsyncTask _controlTask;
		private Func<IAsyncTask> _nextAction;
		private Action _onComplete;

		public ContinuationFromFunc(Func<IAsyncTask> nextAction, IAsyncTask controlTask)
		{
			_nextAction = nextAction;
			_controlTask = controlTask;
			
			_controlTask.WhenCompleted(OnControlTaskCompleted);
			_controlTask.WhenFailed(Fail);
		}

		public IAsyncTask Then(Func<IAsyncTask> nextAction)
		{
			return new ContinuationFromFunc(nextAction, this);
		}


		private void OnControlTaskCompleted()
		{
			try
			{
				IAsyncTask nextTask = _nextAction?.Invoke();
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