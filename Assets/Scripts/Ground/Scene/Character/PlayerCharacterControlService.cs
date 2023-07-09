using System;
using System.Collections.Generic;
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
		private WalkForce _walkForce;
		private AccelerationService.VelocityProvider _velocityProvider;
		private AccelerationService.IForceHandle _accHandle;

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

			_velocityProvider = new AccelerationService.VelocityProvider(_accelerationService, this);
			_walkForce = new WalkForce(_velocityProvider, _charController.transform.forward * 2f, 1.2f);
			_accelerationService.AddForce(this, _walkForce);
			_accHandle = _accelerationService.GetHandle(this);
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
				_accHandle.Rotate(rotation);
			}
		}

		private void HandleJump()
		{
			if (IsJump) return;
			
			if (_isWalking)
			{
				return;
			}
			
			_charController.animator.SetTrigger("JumpAtPlace");
			_jumpForceHandle = _accelerationService.AddForce(this, new CurveForce(_charController.transform.up * 30f, _charController.JumpAtPlace));
		}

		private void HandleForward(float forward)
		{
			if (IsJump) return;
			
			var isWalking = forward > _walkingThreshold;
			if (_isWalking != isWalking)
			{
				_isWalking = isWalking;
				_charController.animator.SetBool("IsWalking", _isWalking);
				
			}
			
			if (_isWalking)
			{
				_walkForce.Proceed(0.2f);
			}
		}

		public bool IsGrounded => _charController.IsGrounded;

		public bool IsJump => _jumpForceHandle is {Completed: false};

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

		private class WalkForce : AccelerationService.IForce, AccelerationService.IForceHandle
		{
			private readonly BrakingForce _brakingForce;
			private readonly LinearForce _walkForce;

			public WalkForce(AccelerationService.VelocityProvider velocityProvider, Vector3 force, float maxSpeed)
			{
				_brakingForce = new BrakingForce(velocityProvider, force * -10f);
				_walkForce = new LinearForce(velocityProvider, force, maxSpeed, false);
			}

			public bool Completed => false;

			public void Proceed(float time)
			{
				_walkForce.Proceed(time);
			}
			
			public Vector3 Update(float dt)
			{
				if (_walkForce.IsPending)
				{
					return _brakingForce.Update(dt);
				}

				return _walkForce.Update(dt);
			}

			public void Rotate(Quaternion rotation)
			{
				_walkForce.Rotate(rotation);
				_brakingForce.Rotate(rotation);
			}
		}
	}
}