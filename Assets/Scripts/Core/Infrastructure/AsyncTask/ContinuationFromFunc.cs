using System;

namespace Core.AsyncTask
{
	public class ContinuationFromFunc : AsyncTask
	{
		private IAsyncTask _controlTask;
		private readonly Func<IAsyncTask> _nextAction;

		public ContinuationFromFunc(Func<IAsyncTask> nextAction, IAsyncTask controlTask)
		{
			_nextAction = nextAction;
			_controlTask = controlTask;
			
			_controlTask.WhenCompleted(OnControlTaskCompleted);
			_controlTask.WhenFailed(Fail);
		}

		public override IAsyncTask Then(Func<IAsyncTask> nextAction)
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
				else
				{
					Complete();
				}
			}
			catch (Exception e)
			{
				Fail(e);
			}
		}
	}
}