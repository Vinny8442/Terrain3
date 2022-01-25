using System;
using System.Collections.Generic;
using System.Diagnostics;
using Game.Infrastructure;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CubeSphere : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    private int _subDivsX = 50;
    private int _subDivsY = 50;

    private abstract class SubMeshData
    {
        public SubMeshData(short id)
        {
        }

        public abstract Mesh CreateMesh();
    }

    public void SetSubDivs(int value)
    {
        _subDivsX = _subDivsY = value;
        Generate();
    }

    public void Generate()
    {
        var timer = new Stopwatch();
        timer.Start();
        var subMesh1 = new RectMeshData(_subDivsY, _subDivsY, i => -0.5f * Vector3.one + new Vector3((float) i.x / _subDivsX, 0, (float)i.y / _subDivsY));
        var subMesh2 = new RectMeshData(_subDivsY, _subDivsY,  i => -0.5f * Vector3.one + new Vector3((float) (_subDivsX - i.x) / _subDivsX, 1, (float)i.y / _subDivsY));
        var subMesh3 = new RectMeshData(_subDivsY, _subDivsY,  i => -0.5f * Vector3.one + new Vector3(1, (float) (i.x) / _subDivsX, (float)i.y / _subDivsY));
        var subMesh4 = new RectMeshData(_subDivsY, _subDivsY,  i => -0.5f * Vector3.one + new Vector3(0, (float) (_subDivsX - i.x) / _subDivsX, (float)i.y / _subDivsY));
        var subMesh5 = new RectMeshData(_subDivsY, _subDivsY,  i => -0.5f * Vector3.one + new Vector3((float) i.x / _subDivsX, (float)i.y / _subDivsY, 1));
        var subMesh6 = new RectMeshData(_subDivsY, _subDivsY,  i => -0.5f * Vector3.one + new Vector3((float) (_subDivsX - i.x) / _subDivsX, (float)i.y / _subDivsY, 0));
        
        var meshData = new MeshData(new []{subMesh1, subMesh2, subMesh3, subMesh4, subMesh5, subMesh6});
        meshData.MakeSphere(0.5f);
        timer.Restart();
        GetComponent<MeshFilter>().sharedMesh = meshData.CreateMesh();
        Debug.Log($"--- {timer.Elapsed.TotalMilliseconds}");
        timer.Stop();
        
    }

    public void Test()
    {
        
        var verts = new VertexData[_subDivsX * _subDivsY];
        var tris = new TriData[2 *_subDivsX * _subDivsY];
        
        var timer = new Stopwatch();
        timer.Start();
        
        for (int j = 0; j < 1000; j++)
        {
            for (int i = 0; i < _subDivsX * _subDivsY; i++)
            {
                // verts[i] = new VertexData(0, new Vector3(1, 2, 3), new Vector2(1, 2), 1  );
                // tris[i * 2] = new TriData(0, 1, 2);
                // tris[i * 2 + 1] = new TriData(0, 1, 2);
                
                verts[i].Set(0, new Vector3(1, 2, 3), new Vector2(1, 2), 1  );
                tris[i * 2].Set(0, 1, 2);
                tris[i * 2 + 1].Set(0, 1, 2);
            }
        }
        timer.Stop();
        Debug.Log(timer.Elapsed.TotalMilliseconds);
    }

    private class MeshData
    {
        private readonly VertexData[] _verts;
        private readonly TriData[] _tris;
        // private readonly SubMeshDescriptor[] _subs;
        
        public MeshData(IEnumerable<RectMeshData> subs)
        {
            int trisCount = 0;
            int vertsCount = 0;
            foreach (RectMeshData sub in subs)
            {
                sub.Init(vertsCount, trisCount);
                vertsCount += sub.VertsCount;
                trisCount += sub.TrisCount;
            }

            _verts = new VertexData[vertsCount];
            _tris = new TriData[trisCount];
            // _subs = new SubMeshDescriptor[subs.Count()];

            // int index = 0;
            // foreach (RectMeshData sub in subs)
            // {
            //     sub.Generate(_tris, _verts);
            //     _subs[index++] = new SubMeshDescriptor(sub.VertsOffset, sub.VertsCount);
            // }

            foreach (RectMeshData sub in subs)
            {
                sub.Generate(_tris, _verts);
            }
        }
        
        public void MakeSphere(float radius)
        {
            for (int i = 0; i < _verts.Length; i++)
            {
                // float noiseAmp = 0.05f * radius * (Random.value - 0.5f);
                float magnitude = _verts[i].Position.magnitude / radius;
                _verts[i] = new VertexData(_verts[i].Position / magnitude, _verts[i].UV, _verts[i].Index);
            }
        }

        public Mesh CreateMesh()
        {
            var mesh = new Mesh();
            var verts = new Vector3[_verts.Length];
            var tris = new int[_tris.Length * 3];
            var uvs = new Vector2[_verts.Length];
            
            for (var i = 0; i < _tris.Length; i++)
            {
                ref var tri = ref _tris[i];
                tris[i * 3 + 0] = tri.v1;
                tris[i * 3 + 1] = tri.v2;
                tris[i * 3 + 2] = tri.v3;
            }

            for (int i = 0; i < _verts.Length; i++)
            {
                ref var vert = ref _verts[i];
                verts[i] = vert.Position;
                uvs[i] = vert.UV;
            }

            // for (int i = 0; i < _subs.Length; i++)
            // {
            //     mesh.SetSubMesh(i, _subs[i]);
            // }
            
            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.uv = uvs;
            mesh.RecalculateNormals();

            return mesh;
        }
        
    }

    private class RectMeshData
    {
        private int _subDivsX;
        private int _subDivsY;
        private Func<Index2, Vector3> _positionModifier;
        public int VertsOffset { get; private set; }
        private int _trisOffset;

        public int VertsCount { get; private set; }
        public int TrisCount { get; private set; }

        public RectMeshData(int subDivsX, int subDivsY, Func<Index2, Vector3> positionModifier) : base()
        {
            _positionModifier = positionModifier;
            _subDivsY = subDivsY;
            _subDivsX = subDivsX;
            VertsCount = (_subDivsX + 1) * (_subDivsY + 1);
            TrisCount = _subDivsX * _subDivsX * 2;
        }

        public void Init(int vertsOffset, int trisOffset)
        {
            _trisOffset = trisOffset;
            VertsOffset = vertsOffset;
        }

        public void Generate(TriData[] tris, VertexData[] verts)
        {
            int tIndex = _trisOffset;
            int pIndex = VertsOffset;
            for (int i = 0; i < _subDivsX + 1; i++)
            {
                for (int j = 0; j < _subDivsY + 1; j++)
                {
                    verts[pIndex] = new VertexData(_positionModifier(new Index2(i, j)), new Vector2((float) i / _subDivsX, (float)j / _subDivsY), pIndex );
                    if (i < _subDivsX && j < _subDivsY)
                    {
                        tris[tIndex++] = new TriData(pIndex, pIndex + _subDivsX + 1, pIndex + 1);
                        tris[tIndex++] = new TriData(pIndex + _subDivsX + 1, pIndex + _subDivsX + 2, pIndex + 1);
                    }
                    pIndex++;
                }
            }
        }


        
    }

    private struct TriData
    {
        public int v1;
        public int v2;
        public int v3;

        public TriData(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public void Set(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public TriData Clone(int offset)
        {
            return new TriData(v1 + offset, v2 + offset, v3 + offset);
        }
    }

    private struct VertexData
    {
        public Vector3 Position;
        public Vector2 UV;
        public int Index;

        public VertexData(Vector3 position, Vector2 uv, int index)
        {
            Index = index;
            UV = uv;
            Position = position;
        }

        public void Set(int subMesh, Vector3 position, Vector2 uv, int index)
        {
            Index = index;
            UV = uv;
            Position = position;
        }

        public VertexData Clone(int offset, int subMesh)
        {
            return new VertexData(Position, UV, Index + offset);
        }
    }
}
