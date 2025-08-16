using System;
using System.Diagnostics;
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
            // var totalStopwatch = Stopwatch.StartNew();
            // UnityEngine.Debug.Log($"[NGrid] Starting Generate - subsX: {subsX}, subsY: {subsY}");

            Profiler.BeginSample($"NGrid::Generate - Generate");
            // var generateStopwatch = Stopwatch.StartNew();
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
                for (var x = 0; x <= subsX; x++)
                {
                    ref var vert = ref verts[index];
                    vert.x = x * tileSizeX - 0.5f;
                    vert.z = y * tileSizeY - 0.5f;
                    vert.y = GetHeight(x, y);
                    ref var uv = ref uvs[index];
                    uv.x = (float)x / subsX;
                    uv.y = (float)y / subsY;
                    if (x != subsX && y != subsY)
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

            // generateStopwatch.Stop();
            Profiler.EndSample();
            // UnityEngine.Debug.Log($"[NGrid] Generate phase completed in {generateStopwatch.ElapsedMilliseconds}ms");

            Profiler.BeginSample($"NGrid::Generate - Apply");
            // var applyStopwatch = Stopwatch.StartNew();
            {
                _mesh.vertices = verts;
                _mesh.triangles = tris;
                _mesh.uv = uvs;
                _mesh.RecalculateNormals();
            }
            // applyStopwatch.Stop();
            Profiler.EndSample();
            // UnityEngine.Debug.Log($"[NGrid] Apply phase completed in {applyStopwatch.ElapsedMilliseconds}ms");

            _meshFilter.sharedMesh = _mesh;

            // totalStopwatch.Stop();
            // UnityEngine.Debug.Log(
            //     $"[NGrid] Total Generate completed in {totalStopwatch.ElapsedMilliseconds}ms (verts: {(subsY + 1) * (subsX + 1)}, tris: {subsY * subsX * 2})");
        }

        protected virtual float GetHeight(int x, int y) => _source?.Get(x, y) ?? 0;

        public interface ISource
        {
            float Get(int x, int y);
        }
    }
}
