using System;
using System.Collections.Generic;
using Core;
using Core.AsyncTask;

namespace Application
{
	public interface ICommand
	{
		IAsyncTask Run();
		bool Completed { get; }
	}

	public interface ICompositeCommand : ICommand
	{
		void Add(ICommand command);
	}
	
	public class SequentCommand : ICompositeCommand
	{
		private Queue<ICommand> _items = new Queue<ICommand>();
		private bool _running = false;
		private AsyncTask _task;

		public void Add(ICommand command)
		{
			_items.Enqueue(command);
		}

		public IAsyncTask Run()
		{
			if (!_running)
			{
				_running = true;
				_task = new AsyncTask(true);
				RunNext();
			}

			return _task;
		}

		public bool Completed => _task?.IsCompleted ?? false;

		private async IAsyncTask RunNext()
		{
			try
			{
				while (_items.Count > 0)
				{
					var command = _items.Dequeue();
					if (!command.Completed)
					{
						await command.Run();
					}
				}
			}
			catch (Exception e)
			{
				_running = false;
				_task.Fail(e);
			}
			finally
			{
				CompleteTask();
			}
		}

		private void CompleteTask()
		{
			_running = false;
			_task.Complete();
		}
	}
}