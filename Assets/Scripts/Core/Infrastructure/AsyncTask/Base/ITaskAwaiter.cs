using System;
using System.Runtime.CompilerServices;

namespace Core.AsyncTask
{
	public interface ITaskAwaiter : INotifyCompletion
	{
		bool IsCompleted { get; }
		void GetResult();
	}
	
	public interface ITaskAwaiter<TResult> : INotifyCompletion
	{
		bool IsCompleted { get; }
		TResult GetResult();
	}
}