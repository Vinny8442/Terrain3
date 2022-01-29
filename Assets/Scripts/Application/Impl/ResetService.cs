using System;
using Application;

namespace Core
{
	public class ResetService : IResetService
	{
		public ResetService()
		{
		}
		
		public void Reset()
		{
			OnResetRequested.Invoke();
		}
		
		public event Action OnResetRequested = delegate {}; 
	}
}