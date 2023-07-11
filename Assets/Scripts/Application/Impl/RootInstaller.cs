using Application.Init;
using Core;
using UnityEngine;
using Zenject;

namespace Terrain.Application.Init
{
	public class RootInstaller
	{
		private DiContainer _container;
		private GameObject _updaterGO;

		public void Install(DiContainer container, GameObject updaterGO)
		{
			_updaterGO = updaterGO;
			_container = container;
			GameObject.DontDestroyOnLoad(updaterGO);

			container.Bind<SettingsStorage>().AsSingle().Lazy();
			container.Bind<PrefabStorage>().AsSingle().Lazy();
			container.BindInterfacesTo<ResetService>().AsSingle().Lazy();
			
			container.BindInterfacesTo<UpdateService>().FromInstance(updaterGO.AddComponent<UpdateService>());
		}

		public void UnInstall()
		{
			_container.Unbind<SettingsStorage>();
			_container.Unbind<PrefabStorage>();
			_container.UnbindInterfacesTo<ResetService>();
			_container.UnbindInterfacesTo<UpdateService>();
			
			GameObject.Destroy(_updaterGO.GetComponent<UpdateService>());
		}
		
	}
}