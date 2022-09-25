using System;

namespace Application.Init
{
	public interface IResetService
	{
		void Reset();
		event Action OnResetRequested;
	}
}