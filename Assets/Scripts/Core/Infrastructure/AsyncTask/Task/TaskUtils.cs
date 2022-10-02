using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.AsyncTask
{
	public class TaskUtils
	{
		private static AsyncTask _completedTask;

		public static IAsyncTask CompletedTask
		{
			get
			{
				if (_completedTask == null)
				{
					_completedTask = new AsyncTask();
					_completedTask.Complete();
				}

				return _completedTask;
			}
		}

		public static IAsyncTask WaitAll(IEnumerable<IAsyncTask> tasks, CancellationToken token)
		{
			// System.Threading.Tasks.Task.Run(async () => await WaitAllInternal(tasks));
			return new ThreadAsyncTask(() =>
			{
				while (!tasks.All(task => task == null || task.IsCompleted || task.IsFailed))
				{
					if (token.IsCancellationRequested)
					{
						throw new TaskCanceledException("WaitAll has been cancelled");
					}
				}
			}, token);
		}

		public static async IAsyncTask<IEnumerable<TResult>> WaitAll<TResult>(IEnumerable<IAsyncTask<TResult>> tasks,
			CancellationToken token)
		{
			await WaitAll(tasks.Select(t => t as IAsyncTask), token);
			return tasks.Where(t => t.IsCompleted).Select(t => t.Result);
		}
	}
}