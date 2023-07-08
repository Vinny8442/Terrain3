using System;
using Core;
using Core.Infrastructure;
using Game.Ground.Services;
using UnityEngine;

namespace Game.Ground
{
	public class PlayerCharacterControlService : TerrainGravity.ITarget, IUpdateTarget, IFixedUpdateTarget, IInitable, AccelerationService.ITarget
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
		
		private AccelerationService.IForceHandle _jumpAcc;
		private AccelerationService.IForce _jumpForceHandle;

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
			}
		}

		private void HandleJump()
		{
			if (_jumpForceHandle?.Completed ?? true)
			{
				_charController.animator.SetTrigger("JumpAtPlace");
				_jumpForceHandle = _accelerationService.AddForce(this, new AccelerationService.CurveForce(_charController.transform.up * 30f, _charController.JumpAtPlace));
				// _jumpForceHandle = _accelerationService.AddForce(this, new AccelerationService.LinearForce(_charController.transform.up * 100, 0.5f));
			}
		}

		private void HandleForward(float forward)
		{
			var isWalking = forward > _walkingThreshold;
			if (_isWalking != isWalking)
			{
				_isWalking = isWalking;
				_charController.animator.SetBool("IsWalking", _isWalking);
				// _accelerationService.AddForce(this, new AccelerationService.CurveForce(_charController.transform.up * 5, _charController.JumpAtPlace));
				// var state = _charController.animator.GetNextAnimatorStateInfo(0);
				// Debug.Log(state);
			}
			
			if (_isWalking)
			{
				// _forwardAcc.Proceed(0.2f);
			}
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