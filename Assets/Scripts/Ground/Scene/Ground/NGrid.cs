using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace Game.Ground
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class NGrid : MonoBehaviour
	{
		private ISource _source;
		private MeshFilter _meshFilter;
		private Mesh _mesh;

		public Mesh Mesh => _mesh;

		public void SetSource(ISource source)
		{
			_source = source;
		}

		private void Start()
		{
			_meshFilter = GetComponent<MeshFilter>();
		}

		public void Generate(int subsX, int subsY)
		{
			Profiler.BeginSample( $"NGrid::Generate - Generate" );
			_mesh = new Mesh();

			var vertCount = (subsY + 1) * (subsX + 1);
			var triCount = subsY * subsX * 6;
			var tileSizeX = 1f / subsX;
			var tileSizeY = 1f / subsY;
			var verts = new Vector3[vertCount];
			var tris = new int[triCount];
			var uvs = new Vector2[vertCount];
			var index = 0;
			var triIndex = 0;
			for (var y = 0; y <= subsY; y++)
			{
				for ( var x = 0; x <= subsX; x++ )
				{
					ref var vert = ref verts[index];
					vert.x = x * tileSizeX - 0.5f;
					vert.z = y * tileSizeY - 0.5f;
					vert.y = GetHeight( x, y );
					ref var uv = ref uvs[index];
					uv.x = x / subsX;
					uv.y = y / subsY;
					if ( x != subsX && y != subsY )
					{
						tris[triIndex * 6 + 0] = index;
						tris[triIndex * 6 + 1] = index + subsX + 1;
						tris[triIndex * 6 + 2] = index + 1;
						tris[triIndex * 6 + 3] = index + subsX + 1;
						tris[triIndex * 6 + 4] = index + subsX + 2;
						tris[triIndex * 6 + 5] = index + 1;
						triIndex++;
					}

					index++;
				}
			}
			Profiler.EndSample(  );
			Profiler.BeginSample( $"NGrid::Generate - Apply" );
			{
				_mesh.vertices = verts;
				_mesh.triangles = tris;
				_mesh.uv = uvs;
				_mesh.RecalculateNormals( );
			}
			Profiler.EndSample(  );

			_meshFilter.sharedMesh = _mesh;
		}
		
		protected virtual float GetHeight(int x, int y) => _source?.Get(x, y) ?? 0;

		public interface ISource
		{
			float Get(int x, int y);
		}
	}
}