using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Core;
using Core.AsyncTask;
using Core.Infrastructure;
using UnityEngine;
using Zenject;

namespace Game.Ground
{
    public class NGridGroup : MonoBehaviour, IInjectable, IInitable, ICoroutineRunner
    {
        [SerializeField] private SectorView _sectorPrefab;

        private readonly List<SectorView> _sectors = new( );
        private Index2 _centerSector;
        private Vector2 _offset;

        [Inject]
        private PrefabStorage _prefabStorage;

        [Inject]
        private SectorControlService _sectorControl;

        [Inject]
        private SettingsStorage _settingsStorage;

        private const int Radius = 3;

        public void Init( )
        {
            _sectorControl.OnDataLoaded += HandleCenterChanged;

            CreateSubComponents( );
            SetSubComponentsPosition( );
        }

        public void UnInit()
        {
            _sectorControl.OnDataLoaded -= HandleCenterChanged;
        }

        private void HandleCenterChanged( Index2 center )
        {
            HandleCenterChangedInternal( center ).ThrowException( );
        }

        private async IAsyncTask HandleCenterChangedInternal( Index2 center )
        {
            var delta = center - _centerSector;
            _centerSector = center;
            var cache = new Queue<SectorView>( );
            var views = _sectors.ToDictionary( s => s.Index, s => s );
            foreach ( var sectorIndex in EnumerateIndexes( ) )
            {
                var newIndex = sectorIndex - delta;
                var sectorView = views[sectorIndex];
                if ( IsInside( newIndex ) )
                    sectorView.SetIndex( newIndex );
                else
                    cache.Enqueue( sectorView );
            }

            foreach ( var sectorIndex in EnumerateIndexes( ) )
            {
                var replacementIndex = sectorIndex + delta;
                if ( !IsInside( replacementIndex ) )
                {
                    var sectorView = cache.Dequeue( );
                    sectorView.SetIndex( sectorIndex );
                }
            }

            var viewsToRebuild = new List<SectorView>( );
            foreach ( var sectorView in _sectors )
            {
                var density = GetDensityForIndex( sectorView.Index );
                var dataIndex = sectorView.Index + center;
                if ( sectorView.Density != density || sectorView.DataIndex != dataIndex )
                {
                    var sectorData = _sectorControl.GetSectorData( dataIndex );
                    sectorView.SetData( density, sectorData );
                    sectorView.Ground.SetInteractable( density == SectorData.MaxDensity );
                    viewsToRebuild.Add( sectorView );
                }
            }

            transform.localPosition = new Vector3( _centerSector.x * _sectorPrefab.Size.x, 0, _centerSector.y * _sectorPrefab.Size.z );
            SetSubComponentsPosition( );

            // var timer = Stopwatch.StartNew( );
            // Debug.Log($"Start rebuilding {viewsToRebuild.Count} sectors");
            await new CoroutineTask( this, RebuildSectors( viewsToRebuild ), CancellationToken.None );
            // await new CoroutineTask( this, CoroutineTest( ), CancellationToken.None );
            // Debug.Log($"Rebuild {timer.ElapsedMilliseconds}");
            var meshBaker = new MeshBaker( );
            await meshBaker.Bake( viewsToRebuild.Where( s => s.Interactable ) );
            // Debug.Log($"Bake {timer.ElapsedMilliseconds}");
            RebuildColliders( viewsToRebuild );
            // Debug.Log($"Rebuild collider {timer.ElapsedMilliseconds}");
        }


        private static IEnumerator RebuildSectors( List<SectorView> sectors )
        {
            sectors.Sort( ( a, b ) => b.Density.CompareTo( a.Density ) );
            var timer = Stopwatch.StartNew( );
            int i = 0;
            foreach ( var sectorView in sectors )
            {
                sectorView.Ground.Rebuild( );
                i++;
                if ( timer.ElapsedMilliseconds >= 6 )
                {
                    // Debug.Log($"Bake: {i} sectors rebuilt in {timer.ElapsedMilliseconds} ms");
                    yield return null;
                    timer.Restart( );
                }
            }

            yield return null;
        }

        private void RebuildColliders( List<SectorView> sectors )
        {
            sectors.Sort( ( a, b ) => b.Density.CompareTo( a.Density ) );
            sectors.ForEach( x => x.Ground.RebuildCollider( ) );
        }

        private IEnumerable<Index2> EnumerateIndexes( )
        {
            for ( var i = -Radius; i <= Radius; i++ )
            for ( var j = -Radius; j <= Radius; j++ )
                if ( i * i + j * j < ( Radius + 1 ) * ( Radius + 1 ) )
                    yield return new Index2( i, j );
        }

        private bool IsInside( Index2 index )
        {
            return index.R2 < ( Radius + 1 ) * ( Radius + 1 );
        }

        private void CreateSubComponents( )
        {
            foreach ( var index in EnumerateIndexes( ) )
            {
                var grid = _prefabStorage.InstantiateAs<SectorView>(_sectorPrefab.gameObject, transform);
                grid.name = $"SectorView({index.x}, {index.y})";
                grid.SetIndex( index );
                _sectors.Add( grid );
            }
        }

        private void SetSubComponentsPosition( )
        {
            foreach ( var grid in _sectors )
            {
                grid.transform.localPosition = Vector3.Scale(new Vector3( grid.Index.x, 0, grid.Index.y ), grid.Ground.transform.localScale);
            }
        }

        private static int GetDensityForIndex( Index2 index )
        {
            return Mathf.Clamp( 8 - index.R, 0, SectorData.MaxDensity );
        }
    }
}
