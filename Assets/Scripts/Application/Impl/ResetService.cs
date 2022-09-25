using System;
using Application.Init;

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