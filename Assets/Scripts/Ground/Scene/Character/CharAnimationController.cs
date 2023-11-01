using System.Collections.Generic;
using Core;
using Game.Ground.Services;
using Game.Ground.Services.Movement;
using UnityEngine;
using Zenject;

namespace Game.Ground
{
	public class CharAnimationController : MonoBehaviour, IInjectable, IInitable, TerrainGravity.ITarget
	{
		private enum AnimationTrigger
		{
			JumpAtPlace = 1,
			JumpWalking,
			IsWalking
		}

		[SerializeField] public CharacterController _charController;
		[SerializeField] public Animator animator;
		[SerializeField] private float _forwardSpeed = 1;
		[SerializeField] private AnimationCurve _accelerationCurve;
		[SerializeField] private float _rotationSpeed;
		[SerializeField] private AnimationCurve _jumpAtPlaceCurve;
		[SerializeField] private float _jumpAtPlaceForce;
		[SerializeField] private float _jumpWalkingForce;
		
		[Inject] private readonly AccelerationService _accelerationService;
		[Inject] private readonly TerrainGravity _gravityService;
		
		private bool _isWalking = false;
		private WalkMovement _walkMovement;
		private AccelerationService.IForce _jumpForceHandle;

		public bool IsGrounded => _charController.isGrounded;

		public Vector3 WorldPosition => transform.position;

		public void Jump()
		{
			if (_jumpForceHandle is {Completed: false}) return;
			
			if (_isWalking)
			{
				animator.SetTrigger(AnimationTrigger.JumpWalking.ToString());
				_jumpForceHandle = _accelerationService.AddForce(this, new CompositeForce(new AccelerationService.IForce[] {
					new LinearForce(_charController.transform.up * _jumpWalkingForce, 0.4f),
					new CompensationForce(new LinearForce(_charController.transform.forward * 7.5f, 0.4f), 0.4f),
				}));
			}
			else
			{
				animator.SetTrigger(AnimationTrigger.JumpAtPlace.ToString());
				_jumpForceHandle = _accelerationService.AddForce(this, new CurveForce(_charController.transform.up * _jumpAtPlaceForce, _jumpAtPlaceCurve));
			}
		}

		public void Walk(bool isWalking)
		{
			if (_isWalking != isWalking)
			{
				_isWalking = isWalking;
				animator.SetBool(AnimationTrigger.IsWalking.ToString(), _isWalking);
				if (_isWalking)
				{
					Debug.Log("start walk");
					_walkMovement.Go();
				}
				else
				{
					_walkMovement.Idle();
				}
			}
		}


		public void Turn(float value)
		{
			Quaternion rotation = Quaternion.AngleAxis(value * _rotationSpeed, transform.up);
			_charController.transform.rotation *= rotation;
			_walkMovement.Rotate(rotation);
		}

		public void Init()
		{
			_gravityService.AddTarget(this);
			
			_walkMovement = new WalkMovement(new CurveMovement(_charController.transform.forward * _forwardSpeed, _accelerationCurve), new SimpleMovement(Vector3.zero));
			_accelerationService.AddMovement(this, _walkMovement);
		}

		public void UnInit() { }
		
		public void ApplyMove(Vector3 movement)
		{
			_charController.Move(movement);
		}
	}
}