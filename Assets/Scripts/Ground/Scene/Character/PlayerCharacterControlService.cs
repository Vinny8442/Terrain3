using System;
using Core;
using Core.Infrastructure;

namespace Game.Ground
{
	public class PlayerCharacterControlService : IUpdateTarget, IInitable
	{
		
		private readonly PlayerInputReaderService _input;
		private readonly IUpdater _updater;
		private readonly SectorControlService _sectorControl;

		private const float WalkingThreshold = 0.1f;

		private CharAnimationController _charController;

		public PlayerCharacterControlService(
			PlayerInputReaderService input, 
			IUpdater updater, 
			SectorControlService sectorControl)
		{
			_updater = updater;

			_input = input;
			_input.OnInput += HandleInput;

			_sectorControl = sectorControl;
		}

		public void Init()
		{
			_updater.AddUpdate(this);
		}

		public void UnInit()
		{
			_updater.RemoveUpdate(this);
		}

		public void RegisterController(CharAnimationController controller)
		{
			_charController = controller;
		}

		private void HandleInput(PlayerInputReaderService.InputData data)
		{
			if (data.Forward > 0)
			{
				_charController.Walk(data.Forward > WalkingThreshold);
			}

			if (data.Jump)
			{
				_charController.Jump();
			}

			if (Math.Abs(data.Side) > 0.1f)
			{
				_charController.Turn(data.Side * data.DT);
			}
		}

		public void HandleUpdate(float dt)
		{
			_sectorControl.HandleCharacterPositionUpdated(_charController.WorldPosition);
		}
	}
}