using System;

namespace Application
{
	public class ResetService : IResetService
	{
		public ResetService() {}

		public void Reset()
		{
			OnResetRequested.Invoke();
		}

		public event Action OnResetRequested = delegate {};
	}
}