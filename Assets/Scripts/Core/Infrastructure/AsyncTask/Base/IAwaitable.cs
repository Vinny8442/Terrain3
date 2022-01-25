namespace Core.AsyncTask
{
	public interface IAwaitable/* where TAwaiter : ITaskAwaiter*/
	{
		AsyncTaskAwaiter GetAwaiter();
	}

	public interface IAwaitable<TResult> /*where TAwaiter : ITaskAwaiter<TResult>*/
	{
		AsyncTaskAwaiter<TResult> GetAwaiter();
	}
}