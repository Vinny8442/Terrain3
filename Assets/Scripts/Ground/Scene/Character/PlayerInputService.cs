using System;
using Core;
using Core.Infrastructure;
using UnityEngine;

namespace Game.Ground
{
	public class PlayerInputService : IUpdateTarget, IInitable
	{
		private readonly IUpdater _updater;
		public event Action<InputData> OnInput;
		
		public PlayerInputService(IUpdater updater)
		{
			_updater = updater;
		}

		public void HandleUpdate(float dt)
		{
			var inputX = Input.GetAxis ("Horizontal");
			var inputZ = Input.GetAxis ("Vertical");
			var jump = Input.GetKeyDown(KeyCode.Space);

			if (inputX != 0 || inputZ != 0 || jump)
			{
				OnInput?.Invoke(new InputData(inputZ, inputX, jump));
			} 
		}

		public void Init()
		{
			_updater.AddUpdate(this);
		}

		public void UnInit()
		{
			_updater.RemoveUpdate(this);
		}
		
		public readonly struct InputData
		{
			public readonly float Forward;
			public readonly float Side;
			public readonly bool Jump;

			public InputData(float forward, float side, bool jump)
			{
				Jump = jump;
				Side = side;
				Forward = forward;
			}
		}
	}
}