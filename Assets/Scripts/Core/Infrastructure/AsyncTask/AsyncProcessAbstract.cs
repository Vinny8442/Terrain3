namespace Core.AsyncTask
{
	public abstract class AsyncProcessAbstract : AsyncTask, IAsyncProcess
	{
		public abstract IAsyncTask Start();

		public virtual int Priority { get; } = 0;
	}
}