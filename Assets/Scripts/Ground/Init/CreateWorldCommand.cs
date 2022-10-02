using Application.Init;
using Core;
using Core.AsyncTask;
using Game.Ground;
using UnityEngine;

namespace Terrain.Scene.Ground
{
	public class CreateWorldCommand : ICommand
	{
		private readonly PrefabStorage _prefabStorage;
		private readonly SectorControlService _sectorControl;

		private Transform _root; 

		public CreateWorldCommand(SettingsStorage settings, PrefabStorage prefabStorage, SectorControlService sectorControl, Transform root)
		{
			_sectorControl = sectorControl;
			_prefabStorage = prefabStorage;
			_root = root;
		}
		
		public async IAsyncTask Run()
		{
			_sectorControl.Init();
			
			NGridGroup _gridGroup = _prefabStorage.InstantiateAs<NGridGroup>("SectorViewGroup", _root);
			PlayerController avatar = _prefabStorage.InstantiateAs<PlayerController>("Avatar", _root);
			
			Completed = true;
		}

		public bool Completed { get; private set; } = false; 
	}
}