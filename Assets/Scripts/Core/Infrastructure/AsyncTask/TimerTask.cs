using System.Collections;
using System.Threading;
using Core.AsyncTask;

namespace Core.Infrastructure.AsyncTask
{
	public class TimerTask : CoroutineTask
	{
		public TimerTask(ICoroutineRunner runner, CancellationToken token) : this(runner, null, token)
		{
		}

		private TimerTask(ICoroutineRunner runner, IEnumerator enumerator, CancellationToken token) : base(runner,
			enumerator, token)
		{
		}
	}
}