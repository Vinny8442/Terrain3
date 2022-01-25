namespace Core.AsyncTask
{
	public interface IAsyncProcess : IAsyncTask
	{
		IAsyncTask Start();
	}
}