using System;

namespace Application
{
	public interface IResetService
	{
		void Reset();
		event Action OnResetRequested;
	}
}