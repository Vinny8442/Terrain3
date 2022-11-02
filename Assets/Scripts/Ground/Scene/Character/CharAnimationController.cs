using Core;
using UnityEngine;

namespace Game.Ground
{
	public class CharAnimationController : MonoBehaviour, IInitable
	{
		[SerializeField] private CharacterController _charController;
		[SerializeField] private Animator animator;
		[SerializeField] private float _forwardSpeed = 1;
		
		// [Range(0,1f)] public float StartAnimTime = 0.3f;
		// [Range(0, 1f)] public float StopAnimTime = 0.15f;
		// [Range(1, 10f)] public float RotationSpeed = 2f;
		
		public bool IsGrounded => _charController.isGrounded;

		public void Move(Vector3 movement)
		{
			_charController.Move(movement);
		}

		public Vector3 WorldPosition => transform.position;


		private bool UpdateMove()
		{
			// var InputX = Input.GetAxis ("Horizontal");
			// var InputZ = Input.GetAxis ("Vertical");
			
			// var Speed = new Vector2(InputX, InputZ).sqrMagnitude;
			// var Speed = InputZ;
		
			bool changed = false;
			// Vector3 forwardSpeed = transform.forward * (_forwardSpeed * Speed * Time.deltaTime);
			//
			// if (Speed > 0.1f)
			// {
			// 	animator.SetFloat ("Blend", Speed, StartAnimTime, Time.deltaTime);
			// 	_charController.Move(forwardSpeed);
			// }
			// else
			// {
			// 	animator.SetFloat ("Blend", Speed, StopAnimTime, Time.deltaTime);
			// 	animator.GetCurrentAnimatorClipInfo()
			// }
			// changed = Speed > 0;

			return changed;
		}

		private bool UpdateRotation()
		{
			// var InputX = Input.GetAxis ("Horizontal");
			// if (InputX != 0)
			// {
			// 	Vector3 localRotationEuler = Quaternion.LookRotation(Vector3.right).eulerAngles * InputX * RotationSpeed * Time.deltaTime;
			// 	transform.rotation *= Quaternion.Euler(localRotationEuler);
			// 	return true;
			// }
			return false;
		}

		public void Init() { }

		public void UnInit() { }
	}
}