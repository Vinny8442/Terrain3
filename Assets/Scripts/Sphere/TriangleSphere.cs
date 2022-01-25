using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TriangleSphere : MonoBehaviour
{
    private int _subDivs = 1;

    // Start is called before the first frame update
    public float R { get; private set; } = 0.5f;

    public void SetSubDivs(int subDivs)
    {
        _subDivs = subDivs;
        Generate();
    }

    public void Test()
    {
        R = 0.5f;
        Generate();
    }

    private Vector3 FromSpherical(float o, float f)
    {
        o *= Mathf.Deg2Rad;
        f *= Mathf.Deg2Rad;
        return new Vector3(
            R * Mathf.Sin(o) * Mathf.Cos(f),
            R * Mathf.Sin(o) * Mathf.Sin(f),
            R * Mathf.Cos(o)
            );
    }

    private void Generate()
    {
        int triCount = 8;
        int vertCount = 6;
        for (int i = 1; i <= _subDivs; i++)
        {
            vertCount += 3 * triCount;
            triCount *= 4;
        }
        
        Vector3 A = FromSpherical(90, 0);
        Vector3 B = FromSpherical(90, 90);
        Vector3 C = FromSpherical(90, 180);
        Vector3 D = FromSpherical(90, 270);
        Vector3 E = FromSpherical(0, 0);
        Vector3 F = FromSpherical(180, 0);
        
        TriData[] tris = new TriData[8];
        Vector3[] verts = new Vector3[vertCount];

        verts[0] = A;
        verts[1] = B;
        verts[2] = C;
        verts[3] = D;
        verts[4] = E;
        verts[5] = F;
        
        tris[0] = new TriData(4, 0, 1);
        tris[1] = new TriData(4, 3, 0);
        tris[2] = new TriData(4, 2, 3);
        tris[3] = new TriData(4, 1, 2);
        tris[4] = new TriData(5, 1, 0);
        tris[5] = new TriData(5, 2, 1);
        tris[6] = new TriData(5, 3, 2);
        tris[7] = new TriData(5, 0, 3);

        int currentVertIndex = 6;
        for (int i = 0; i < _subDivs; i++)
        {
            for (int j = 0; j < tris.Length; j++)
            {
                currentVertIndex = SubDivide(tris[j], verts, currentVertIndex);
            }
        }
        
        int[] indexes = new int[triCount * 3];
        var trisIndex = 0;
        for (int i = 0; i < tris.Length; i++)
        {
            trisIndex = tris[i].Fill(indexes, trisIndex);
        }
        
        // Vector2[] uvs = new []{new Vector2(A.x, A.y), new Vector2(B.x, B.y), new Vector2(C.x, C.y), new Vector2(D.x, D.y), };
        Vector2[] uvs = new Vector2[vertCount];
        for (int i = 0; i < vertCount; i++)
        {
            uvs[i] = Vector2.one;
        }

        var mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = indexes;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private int SubDivide(in TriData tri, in Vector3[] verts, int vertIndex)
    {
        int undivTrisCount = tri.GetCount();
        Vector3[] newVerts = new Vector3[undivTrisCount];
        TriData[] undivTris = new TriData[undivTrisCount];
        tri.Fill(undivTris, 0);
        for (int i = 0; i < undivTrisCount; i++)
        {
            vertIndex = undivTris[i].SubDivide(vertIndex, verts);
        }

        return vertIndex;
    }

    public class TriData
    {
        public int A;
        public int B;
        public int C;

        public bool HasSubs = false;
        private TriData[] _subs;

        public TriData(int a, int b, int c)
        {
            A = a;
            B = b;
            C = c;
        }

        public int SubDivide(int D, in Vector3[] verts)
        {
            if (HasSubs)
            {
                throw  new InvalidOperationException("Already subdivided");
            }
            
            HasSubs = true;
            
            var AB = D + 0; 
            verts[AB] = Spherize((verts[A] + verts[B]) / 2);
            var AC = D + 1; 
            verts[AC] = Spherize((verts[A] + verts[C]) / 2);
            var BC = D + 2; 
            verts[BC] = Spherize((verts[B] + verts[C]) / 2);

            _subs = new TriData[4];
            int index = 0;
            _subs[0] = new TriData(A, AB, AC);
            _subs[1] = new TriData(B, BC, AB);
            _subs[2] = new TriData(C, AC, BC);
            _subs[3] = new TriData(AB, BC, AC);
            
            return D + 3;
        }

        private static Vector3 Spherize(in Vector3 vert)
        {
            // return vert;
            return vert * (0.5f / vert.magnitude);
        }

        public int GetCount()
        {
            if (!HasSubs)
            {
                return 1;
            }

            return _subs.Sum(sub => sub.GetCount());
        }

        public int Fill(TriData[] target, int startIndex)
        {
            if (!HasSubs)
            {
                target[startIndex] = this;
                return startIndex + 1;
            }

            foreach (TriData sub in _subs)
            {
                startIndex = sub.Fill(target, startIndex);
            }
            return startIndex;
        }

        public int Fill(in int[] indexes, int index)
        {
            if (!HasSubs)
            {
                indexes[index++] = A;
                indexes[index++] = B;
                indexes[index++] = C;
                return index;
            }

            foreach (TriData sub in _subs)
            {
                index = sub.Fill(indexes, index);
            }
            return index;
        }
    }
}
