using System;
using Core;
using Core.Infrastructure;
using Game.Ground.Services;
using UnityEngine;

namespace Game.Ground
{
	public class PlayerCharacterControlService : TerrainGravity.Target, IUpdateTarget, IFixedUpdateTarget, IInitable, AccelerationService.Target
	{
		public event Action OnJump = delegate { };
		public event Action<float> OnRun = delegate { };
		
		private readonly PlayerInputReaderService _input;
		private readonly TerrainGravity _gravityService;
		private readonly IUpdater _updater;
		private readonly AccelerationService _accelerationService;
		private readonly SectorControlService _sectorControl;

		private CharAnimationController _charController;

		private bool _isWalking = false;
		private float _walkingThreshold = 0.1f;
		
		private AccelerationService.Handler _forwardAcc;
		private AccelerationService.Handler _jumpAcc;

		public PlayerCharacterControlService(
			PlayerInputReaderService input, 
			TerrainGravity gravityService, 
			IUpdater updater, 
			SectorControlService sectorControl,
			AccelerationService accelerationService)
		{
			_updater = updater;
			_accelerationService = accelerationService;


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
			
			_forwardAcc = _accelerationService.CreateAcceleration(this, _charController.transform.forward * 5f, 10f, _charController.MaxSpeed);
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
		

		private void HandleInput(PlayerInputReaderService.InputData data)
		{
			if (data.Forward > 0)
			{
				HandleForward(data.Forward);
			}

			if (data.Jump)
			{
				HandleJump();
			}

			if (Math.Abs(data.Side) > 0.1f)
			{
				Quaternion rotation = Quaternion.AngleAxis(Math.Sign(data.Side) * data.DT * _charController.RotationSpeed, _charController.transform.up);
				_charController.transform.rotation *= rotation; 
				_forwardAcc.Rotate(rotation);
			}
		}

		private void HandleJump()
		{
			_charController.animator.SetTrigger("JumpAtPlace");
			_jumpAcc = _accelerationService.CreateAcceleration(this, _charController.transform.up * 20f, _charController.JumpAtPlace);
			_jumpAcc.Proceed(0.0f);
			_jumpAcc.ReleaseWhenCompleted();
		}

		private void HandleForward(float forward)
		{
			var isWalking = forward > _walkingThreshold;
			if (_isWalking != isWalking)
			{
				_isWalking = isWalking;
				_charController.animator.SetBool("IsWalking", _isWalking);
				// var state = _charController.animator.GetNextAnimatorStateInfo(0);
				// Debug.Log(state);
			}
			
			if (_isWalking)
			{
				_forwardAcc.Proceed(0.2f);
			}
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
		}

		public void ApplyAcc(Vector3 movement)
		{
			_charController.Move(movement);
		}
	}
}