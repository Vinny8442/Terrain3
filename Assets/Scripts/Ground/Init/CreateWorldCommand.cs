using Application.Init;
using Core;
using Core.AsyncTask;
using Game.Ground;
using Game.Ground.Services;
using UnityEngine;
using Zenject;

namespace Terrain.Scene.Ground
{
	public class CreateWorldCommand : ICommand
	{
		private readonly PrefabStorage _prefabStorage;
		private readonly SectorControlService _sectorControl;

		private Transform _root;
		private PlayerCharacterControlService _charControlService;
		
		[Inject] private TerrainGravity _gravity;

		public CreateWorldCommand(
			PrefabStorage prefabStorage, 
			SectorControlService sectorControl,
			PlayerCharacterControlService charControlService,
			Transform root)
		{
			_charControlService = charControlService;
			_sectorControl = sectorControl;
			_prefabStorage = prefabStorage;
			_root = root;
		}
		
		public async IAsyncTask Run()
		{
			_sectorControl.Init();
			_gravity.Init();
			
			NGridGroup _gridGroup = _prefabStorage.InstantiateAs<NGridGroup>("SectorViewGroup", _root);
			CharAnimationController avatar = _prefabStorage.InstantiateAs<CharAnimationController>("Avatar", _root);

			_charControlService.RegisterController(avatar);
			
			Completed = true;
		}

		public bool Completed { get; private set; } = false; 
	}
}