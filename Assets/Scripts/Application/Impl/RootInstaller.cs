using Core;
using Game.Ground;
using Game.Sector;
using Zenject;

namespace Application
{
	public class RootInstaller
	{
		public void Install(DiContainer container)
		{
			container.Bind<SettingsStorage>().AsSingle().Lazy();
			container.Bind<PrefabStorage>().AsSingle().Lazy();

			container.BindInterfacesTo<ResetService>().AsSingle().Lazy();
			container.BindInterfacesTo<HeightDataSource>().AsSingle().Lazy();
			container.Bind<SectorDataProvider>().AsSingle().Lazy();
			container.Bind<SectorControlService>().AsSingle().Lazy();
			
			
		}
	}
}