using System;
using Core;
using Core.Infrastructure;
using Game.Ground.Services;
using UnityEngine;

namespace Game.Ground
{
	public class PlayerCharacterControlService : TerrainGravity.Target, IUpdateTarget, IFixedUpdateTarget, IInitable
	{
		public event Action OnJump = delegate { };
		public event Action<float> OnRun = delegate { };
		
		private readonly PlayerInputService _input;
		private readonly TerrainGravity _gravityService;
		private readonly IUpdater _updater;
		private readonly SectorControlService _sectorControl;
		
		private CharAnimationController _charController;

		private bool _physicsUpdated = false;
		private bool _positionUpdated = false;

		public PlayerCharacterControlService(
			PlayerInputService input, 
			TerrainGravity gravityService, 
			IUpdater updater, 
			SectorControlService sectorControl)
		{
			_updater = updater;
			
			_gravityService = gravityService;
			_gravityService.AddTarget(this);
			
			_input = input;
			_input.OnInput += HandleInput;

			_sectorControl = sectorControl;
		}

		public void Init()
		{
			_updater.AddUpdate(this);
			_updater.AddFixedUpdate(this);
		}

		public void UnInit()
		{
			_updater.RemoveUpdate(this);
			_updater.RemoveFixedUpdate(this);
		}

		public void RegisterController(CharAnimationController controller)
		{
			_charController = controller;
		}
		

		private void HandleInput(PlayerInputService.InputData data)
		{
			if (data.Forward > 0)
			{
				HandleForward();
			}

			if (data.Jump)
			{
				HandleJump();
			}
		}

		private void HandleJump()
		{
			_charController.animator.SetTrigger("JumpAtPlace");
		}

		private void HandleForward()
		{
			Debug.Log("Handle Forward");
		}

		public void ApplyGravity(Vector3 gravity)
		{
			_charController.Move(gravity);
		}

		public bool IsGrounded => _charController.IsGrounded;

		public void HandleUpdate(float dt)
		{
			_sectorControl.HandleCharacterPositionUpdated(_charController.WorldPosition);
		}

		public void HandleFixedUpdate(float dt)
		{
			_physicsUpdated = true;
		}
	}
}