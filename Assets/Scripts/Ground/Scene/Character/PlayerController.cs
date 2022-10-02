using Core;
using UnityEngine;
using Zenject;

namespace Game.Ground
{
	public class PlayerController : MonoBehaviour, IInitable, IInjectable
	{
		[Inject] private SectorControlService _sectorControl;
		
		[SerializeField] private float _gravity = 0f;
		[SerializeField] private CharacterController _charController;
		[SerializeField] private Animator animator;
		[SerializeField] private float _groundSensitivity = 0.1f;
		[SerializeField] private LayerMask _surfaceLayer;
		[SerializeField] private float _forwardSpeed = 1;
		
		[Range(0,1f)] public float StartAnimTime = 0.3f;
		[Range(0, 1f)] public float StopAnimTime = 0.15f;
		[Range(1, 10f)] public float RotationSpeed = 2f;
		
		private Vector3 _gravityVelocity;
		private bool _grounded; 
		private bool _positionUpdated;

		public void Init() {}

		public void UnInit() {}

		private void Update()
		{
			if (_positionUpdated)
			{
				_sectorControl.HandleCharacterPositionUpdated(_charController.transform.position);
				_positionUpdated = false;
			}

			_grounded = _charController.isGrounded;

			_positionUpdated = UpdateRotation() || _positionUpdated;
			_positionUpdated = UpdateMove() || _positionUpdated;
			_positionUpdated = UpdateGravity(_grounded) || _positionUpdated;
		}

		private bool UpdateGravity(bool grounded)
		{
			// return false;
			bool changed = false;
			if (!grounded)
			{
				_gravityVelocity += Vector3.down * _gravity * Time.deltaTime;
				changed = true;
			}
			else
			{
				_gravityVelocity = Vector3.down * 0.5f;
			}
			_charController.Move(_gravityVelocity);

			return changed;
		}

		private bool UpdateMove()
		{
			var InputX = Input.GetAxis ("Horizontal");
			var InputZ = Input.GetAxis ("Vertical");
			
			// var Speed = new Vector2(InputX, InputZ).sqrMagnitude;
			var Speed = InputZ;
		
			bool changed = false;
			Vector3 forwardSpeed = transform.forward * (_forwardSpeed * Speed * Time.deltaTime);
			
			if (Speed > 0.1f)
			{
				animator.SetFloat ("Blend", Speed, StartAnimTime, Time.deltaTime);
				_charController.Move(forwardSpeed);
			}
			else
			{
				animator.SetFloat ("Blend", Speed, StopAnimTime, Time.deltaTime);
			}
			changed = Speed > 0;

			return changed;
		}

		private bool UpdateRotation()
		{
			var InputX = Input.GetAxis ("Horizontal");
			if (InputX != 0)
			{
				Vector3 localRotationEuler = Quaternion.LookRotation(Vector3.right).eulerAngles * InputX * RotationSpeed * Time.deltaTime;
				transform.rotation *= Quaternion.Euler(localRotationEuler);
				return true;
			}

			return false;
		}
	}
}