using System;
using Application;
using Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Game.UI
{
	public class ResetPanelPresenter : MonoBehaviour, IInitable, IInjectable
	{
		[SerializeField]
		private Button _resetButton;

		[Inject]
		private IResetService _resetService;

		private void Start()
		{
			_resetButton.onClick.AddListener( OnResetClick );
		}

		public void Init()
		{
			
		}

		public void UnInit()
		{
		}

		private void OnResetClick()
		{
			_resetService.Reset();
		}
	}
}