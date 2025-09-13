using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.AsyncTask;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Game.Ground
{
	public class MeshBaker
	{

		private IAsyncTask _currentTask;

		public async IAsyncTask Bake(IEnumerable<SectorView> meshes)
		{
			if (_currentTask != null)
			{
				await _currentTask;
			}

			try
			{
				_currentTask = BakeInternal(meshes);
				await _currentTask;
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
			finally
			{
				_currentTask = null;
			}
		}

		private static IAsyncTask BakeInternal( IEnumerable<SectorView> sectors )
		{
			List<IAsyncTask> tasks = new List<IAsyncTask>(sectors.Count());
			int i = 0;
			foreach ( SectorView sector in sectors )
			{
				int instanceID = sector.Ground.Mesh.GetInstanceID( );
				IAsyncTask thread = new ThreadAsyncTask( ( ) =>
				{
					try
					{
						// Debug.Log( $"--- Baking index {index}({instanceID}) on thread {Thread.CurrentThread.ManagedThreadId}" );
						Physics.BakeMesh( instanceID, false );
					}
					catch ( Exception e )
					{
						Debug.LogError( $"--- Baking error \n{e}" );
					}
				}, CancellationToken.None );
				tasks.Add( thread );
				i++;
			}

			return TaskUtils.WaitAll( tasks, CancellationToken.None );
		}

		// private async IAsyncTask BakeInternal(IEnumerable<SectorView> sectors)
		// {
		// 	try
		// 	{
		// 		NativeArray<int> meshIds = new NativeArray<int>(sectors.Count(), Allocator.TempJob);
		//
		// 		int i = 0;
		// 		foreach (var sector in sectors)
		// 		{
		// 			meshIds[i++] = sector.Mesh.GetInstanceID();
		// 		}
		//
		// 		var job = new BakeJob(meshIds);
		// 		job.Schedule(meshIds.Length, 100).Complete();
		//
		// 		meshIds.Dispose();
		// 	}
		// 	catch (Exception e)
		// 	{
		// 		Debug.LogError(e);
		// 	}
		// }

		// private struct BakeJob : IJobParallelFor
		// {
		// 	private NativeArray<int> meshIds;
		//
		// 	public BakeJob(NativeArray<int> meshIds)
		// 	{
		// 		this.meshIds = meshIds;
		// 	}
		//
		// 	public void Execute(int index)
		// 	{
		// 		Debug.Log($"--- Baking index {index} on thread {Thread.CurrentThread.Name}");
		// 		Physics.BakeMesh(meshIds[index], false);
		// 	}
		// }
	}
}
