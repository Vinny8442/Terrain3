using System;
using System.Collections.Generic;
using Core;
using Core.AsyncTask;
using UnityEngine;

namespace Game.Ground
{
	public class SectorControlService : IInitable
	{
		private static readonly Vector2 SectorSize = Vector2.one;
		private const int Radius = 3;

		private readonly SettingsStorage _settings;
		private readonly SectorDataProvider _dataProvider;

		private float _sectorSize = 1;

		private Index2 _centerSectorIndex = new( 10000, 0 );
		private readonly Dictionary<Index2, SectorData> _sectors = new( );

		public event Action<Index2> OnDataLoaded;

		public SectorControlService( SettingsStorage settings, SectorDataProvider dataProvider )
		{
			_dataProvider = dataProvider;
			_settings = settings;
		}

		public SectorData GetSectorData( Index2 index )
		{
			if ( _sectors.TryGetValue( index, out var sectorData ) )
			{
				return sectorData;
			}

			throw new KeyNotFoundException( $"Sector with index {index} not loaded" );
		}

		public void HandleCharacterPositionUpdated( Vector3 position )
		{
			Index2 newCenterSector = GetSectorIndex( position.x, position.z );
			if ( newCenterSector != _centerSectorIndex )
			{
				UpdateCenterIndex( newCenterSector ).ThrowException( ); 
			}
		}

		private async IAsyncTask UpdateCenterIndex( Index2 newCenterIndex )
		{
			_centerSectorIndex = newCenterIndex;
			List<SectorDataProvider.SectorRequest> requests = new List<SectorDataProvider.SectorRequest>( );
			for ( int i = -Radius; i <= Radius; i++ )
			{
				for ( int j = -Radius; j <= Radius; j++ )
				{
					if ( i * i + j * j < ( Radius + 1 ) * ( Radius + 1 ) )
					{
						Index2 index = new Index2( i, j );
						Index2 dataIndex = index + _centerSectorIndex;
						int requiredDensity = GetDensityForIndex( index );
						if ( !_sectors.TryGetValue( dataIndex, out var sectorData ) || sectorData.Density < requiredDensity )
						{
							requests.Add( new SectorDataProvider.SectorRequest( dataIndex, requiredDensity, 100 * new Vector2( dataIndex.x, dataIndex.y ), 100 * Vector2.one ) );
						}
					}
				}
			}

			IEnumerable<SectorData> result = await _dataProvider.RequestSectorData( requests );
			foreach ( SectorData sectorData in result )
			{
				_sectors[sectorData.Index] = sectorData;
			}

			try
			{
				OnDataLoaded?.Invoke( _centerSectorIndex );
			}
			catch ( Exception e )
			{
				Debug.LogError( e );
			}
		}

			private static int GetDensityForIndex( Index2 index )
	{
		return Mathf.Clamp( 8 - index.R, 0, SectorData.MaxDensity );
	}

		private Index2 GetSectorIndex( float x, float y )
		{
			return new Index2( (int)( x / _sectorSize + Math.Sign( x ) * 0.5f ), (int)( y / _sectorSize + Math.Sign( y ) * 0.5f ) );
		}

		public void EditorMoveCenter( Index2 delta )
		{
			UpdateCenterIndex( _centerSectorIndex + delta ); 
		}

		public void Init()
		{
			var groundSettings = LoadGroundSettings();

			_sectorSize = groundSettings.SectorSize;

			HandleCharacterPositionUpdated( Vector3.zero );
		}
		
		public void UnInit()
		{
		}

		private GroundSettings LoadGroundSettings()
		{
			_settings.Clear( "GroundSettings" );
			var groundSettings = _settings.Load<GroundSettings>( "GroundSettings" );
			return groundSettings;
		}
	}
}