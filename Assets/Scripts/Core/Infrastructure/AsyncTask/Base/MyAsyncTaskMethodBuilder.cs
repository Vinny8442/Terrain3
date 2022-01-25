using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Core.AsyncTask;

namespace Core.AsyncTask
{
	public struct MyAsyncTaskMethodBuilder
	{
		private AsyncTask<VoidResult> _task;
	
		public IAsyncTask Task
		{
			get
			{
				if (_task == null)
				{
					_task = new AsyncTask<VoidResult>();
				}

				return _task;
			}
		}

		public static MyAsyncTaskMethodBuilder Create()
		{
			return default;
		}
	
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}
	
		[DebuggerStepThrough]
		public void Start<TStateMachine>(ref TStateMachine stateMachine)
			where TStateMachine : IAsyncStateMachine
		{
			stateMachine.MoveNext();
		}
	
		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine machine)
			where TAwaiter : INotifyCompletion
			where TStateMachine : IAsyncStateMachine
		{
			MyAsyncTaskMethodBuilder<VoidResult>.AwaitOnCompleted(ref awaiter, ref machine, ref _task);
		}
	
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine machine)
			where TAwaiter : INotifyCompletion
			where TStateMachine : IAsyncStateMachine
		{
			MyAsyncTaskMethodBuilder<VoidResult>.AwaitOnCompleted(ref awaiter, ref machine, ref _task);
		}
	
		public void SetResult()
		{
			if (_task == null) _task = new AsyncTask<VoidResult>();
	
			_task.Complete(new VoidResult());
		}
	
		public void SetException(Exception ex)
		{
			if (_task == null) _task = new AsyncTask<VoidResult>();
	
			_task.Fail(ex);
		}
	
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct VoidResult
		{
		}
	}
}