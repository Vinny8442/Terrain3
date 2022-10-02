using Application.Init;
using Core;
using UnityEngine;
using Zenject;

namespace Terrain.Application.Init
{
	public class RootInstaller
	{
		public void Install(DiContainer container, GameObject monobehUpdater)
		{
			GameObject.DontDestroyOnLoad(monobehUpdater);

			container.Bind<SettingsStorage>().AsSingle().Lazy();
			container.Bind<PrefabStorage>().AsSingle().Lazy();
			container.BindInterfacesTo<ResetService>().AsSingle().Lazy();
			container.BindInterfacesTo<UpdateService>().FromInstance(monobehUpdater.AddComponent<UpdateService>());
		}
	}
}