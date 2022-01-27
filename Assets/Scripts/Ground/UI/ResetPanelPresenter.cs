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

		public void Init()
		{
			_resetButton.onClick.AddListener( OnResetClick );
		}

		private void OnResetClick()
		{
			_resetService.Reset();
		}
	}
}