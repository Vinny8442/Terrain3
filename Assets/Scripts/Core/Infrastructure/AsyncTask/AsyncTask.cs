using System;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace Core.AsyncTask
{
	public class AsyncTask : CustomYieldInstruction, IAsyncTask
	{
		private Exception _exception;
		protected Action _onCompleted = delegate { };
		private Action<Exception> _onFailed = delegate { };
		private readonly bool _completeOnFail;

		public AsyncTask(bool completeOnFail = false)
		{
			_completeOnFail = completeOnFail;
		}

		public virtual IAsyncTask Then(Func<IAsyncTask> nextAction)
		{
			return new ContinuationFromFunc(nextAction, this);
		}

		public IAsyncTask WhenCompleted(Action onCompleted)
		{
			if (IsCompleted)
				onCompleted?.Invoke();
			else
				_onCompleted += onCompleted;

			return this;
		}

		public IAsyncTask WhenFailed(Action<Exception> onFailed)
		{
			if (_exception != null)
				onFailed?.Invoke(_exception);
			else
				_onFailed += onFailed;

			return this;
		}
		
		public virtual void Complete()
		{
			if (!IsCompleted)
			{
				IsCompleted = true;
				_onCompleted.Invoke();
			}
		}

		public bool IsCompleted { get; protected set; }
		public bool IsFailed => _exception != null;
		public IAsyncTask ThrowException()
		{
			if (_exception != null)
			{
				// throw _exception;
				ExceptionDispatchInfo.Capture(_exception).Throw();
			}

			return this;
		}

		public virtual void Fail(Exception exception)
		{
			if (_exception == null)
			{
				_exception = exception;
				_onFailed.Invoke(exception);
				if (_completeOnFail)
				{
					_onCompleted.Invoke();
				}
			}
		}

		public override bool keepWaiting => !IsCompleted && _exception == null;
		public void Dispose()
		{
			if (!IsFailed && !IsCompleted)
			{
				Fail(new ObjectDisposedException( @"Reject Task with dispose" ) );
			}
		}
	}
}