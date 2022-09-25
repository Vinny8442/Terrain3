using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Core.Infrastructure.AsyncTask;

namespace Core.AsyncTask
{
	[AsyncMethodBuilder(typeof(MyAsyncTaskMethodBuilder))]
	public interface IAsyncTask : IDisposable
	{
		IAsyncTask Then(Func<IAsyncTask> nextAction);
		IAsyncTask WhenCompleted(Action onCompleted);
		IAsyncTask WhenFailed(Action<Exception> onFailed);
		bool IsCompleted { get; }
		bool IsFailed { get; }
		IAsyncTask ThrowException();
	}
	
	[AsyncMethodBuilder(typeof(MyAsyncTaskMethodBuilder<>))]
	public interface IAsyncTask<TResult> : IAsyncTask
	{
		IAsyncTask WhenCompleted(Action<TResult> onCompleted);
		IAsyncTask Then(Func<TResult, IAsyncTask> nextAction);
		
		TResult Result { get; }

		// IAsyncTask GetTask();
	}
	
	public static class AsyncTaskAwaiterExtensions
	{
		public static AsyncTaskAwaiter GetAwaiter( this IAsyncTask task )
		{
			return new AsyncTaskAwaiter( task );
		}

		public static AsyncTaskAwaiter< T > GetAwaiter< T >( this IAsyncTask< T > task )
		{
			return new AsyncTaskAwaiter< T >( task );
		}
	}
	
}