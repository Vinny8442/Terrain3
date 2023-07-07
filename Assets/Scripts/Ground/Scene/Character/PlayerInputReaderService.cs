using System;
using Core;
using Core.Infrastructure;
using UnityEngine;

namespace Game.Ground
{
	public class PlayerInputReaderService : IUpdateTarget, IInitable
	{
		private readonly IUpdater _updater;
		public event Action<InputData> OnInput;
		
		public PlayerInputReaderService(IUpdater updater)
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
				OnInput?.Invoke(new InputData(inputZ, inputX, jump, dt));
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
			public readonly float DT;

			public InputData(float forward, float side, bool jump, float dt)
			{
				DT = dt;
				Jump = jump;
				Side = side;
				Forward = forward;
			}
		}
	}
}