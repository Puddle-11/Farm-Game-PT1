using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class HillBuilder : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private float height;
    [SerializeField] private int resolution;
    [SerializeField] private float noiseScale;
    [SerializeField] private float noiseWeight;


    [SerializeField] private Color stone;
    [SerializeField] private Color grass;
    private Vector3[] fPoints;
    private Vector3[] tPoints;
    [SerializeField] private float topAngleStrength;
    private Vector3[] verts;
    private Color[] colors;
    private int[] tris;
    private Mesh m;


    private MeshFilter _mf;
    public MeshFilter mf
    {
        get
        {
            if (_mf == null) TryGetComponent(out _mf);
            return _mf;
        }
        set { _mf = value; }
    }
    private MeshRenderer _mr;
    public MeshRenderer mr
    {
        get
        {
            if (_mr == null) TryGetComponent(out _mr);
            return _mr;
        }
        set
        {
            _mr = value;
        }
    }


    public void GeneratePoints()
    {
        fPoints = new Vector3[resolution];
        tPoints = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {

            float x = radius * Mathf.Cos(2 * Mathf.PI * i / resolution);
            float y = radius * Mathf.Sin(2 * Mathf.PI * i / resolution);

            fPoints[i] = new Vector3(x, 0, y);
            tPoints[i] = new Vector3(x, height, y);

            float offset = SampleNoise(new Vector2(fPoints[i].x, fPoints[i].z));
            float toffset = SampleNoise(new Vector2(fPoints[i].x + 100, fPoints[i].z + 100));
            fPoints[i] += fPoints[i].normalized * noiseWeight * radius * offset;
            tPoints[i] += new Vector3(tPoints[i].x, 0, tPoints[i].z).normalized * noiseWeight * radius * offset;
            tPoints[i] += new Vector3(tPoints[i].x, 0, tPoints[i].z).normalized * topAngleStrength * toffset;

        }


        GenerateTrianglePoints(out verts, out colors);
        tris = GenerateTriangles();
        UpdateMesh();
    }
    private void UpdateMesh()
    {


        if (verts.Length <= 0)
        {
            return;
        }

        if (m == null)
        {
            m = new Mesh();
        }
        mf.sharedMesh = new Mesh();

        mf.sharedMesh.vertices = verts;
        mf.sharedMesh.triangles = tris;
        mf.sharedMesh.colors = colors;
        //UV's here

        mf.sharedMesh.RecalculateNormals();
        mf.sharedMesh.RecalculateBounds();


    }
    private void GenerateTrianglePoints(out Vector3[] _verts, out Color[] _col)
    {

        List<Vector3> res = new List<Vector3>();
        List<Color> colorRes = new List<Color>();


        for (int i = 0; i < resolution; i++)
        {

            int startIndex = i * 6;
            res.Add(fPoints[i]);
            res.Add(tPoints[i]);
            res.Add(fPoints[(i + 1) % resolution]);
            res.Add(tPoints[i]);
            res.Add(tPoints[(i + 1) % resolution]);
            res.Add(fPoints[(i + 1) % resolution]);
            colorRes.AddRange(Enumerable.Repeat(stone, 6).ToArray());
        }

        int p = Mathf.CeilToInt((float)resolution / 2);


        for (int i = 0; i < resolution; i++)
        {



            res.Add(tPoints[(i + 1) % resolution]);
            res.Add(tPoints[i]);
            res.Add(new Vector3(0, height, 0));
            colorRes.AddRange(Enumerable.Repeat(grass, 3).ToArray());

            //==========================================================

        }
        _col = colorRes.ToArray();
        _verts = res.ToArray();
       
    }
    private int[] GenerateTriangles()
    {
        int[] res = new int[verts.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            res[i] = i;
        }
        return res;
    }
    public float SampleNoise(Vector2 _point/*local position*/)
    {
        _point += new Vector2(transform.position.x, transform.position.z);

        float offsetx = Mathf.PerlinNoise(_point.x / noiseScale, _point.y / noiseScale) * 2;
        offsetx -= 1;
        return offsetx;
    }
    public void Clean()
    {


    }

    private void OnDrawGizmosSelected()
    {

        DrawPointGizmos();

    }
    private void DrawPointGizmos()
    {
        if (tPoints == null || tPoints.Length <= 0)
        {
            return;
        }
        for (int i = 0; i < tPoints.Length; i++)
        {
            Gizmos.DrawSphere(tPoints[i] + transform.position, 0.1f);
        }


    }
}
