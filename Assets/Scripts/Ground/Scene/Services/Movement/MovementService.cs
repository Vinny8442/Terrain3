using Core.Infrastructure;

namespace Game.Ground.Services.Movement
{
	public class MovementService : IUpdateTarget
	{
		private IUpdater _updateService;

		public MovementService(IUpdater updateService)
		{
			_updateService = updateService;
		}
		
		public void HandleUpdate(float dt)
		{
			throw new System.NotImplementedException();
		}
	}
}