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
    [SerializeField] private bool ramp;
    [SerializeField] private int rampLength;
    [SerializeField] private int rampStartIndex;
    [SerializeField] private float rampWidth;
    [SerializeField] private AnimationCurve rampCurve;
    [SerializeField] private AnimationCurve widthCurve;

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


        CalculateVerticies(out verts, out colors);
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
    private void CalculateVerticies(out Vector3[] _verts, out Color[] _col)
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
        if (ramp)
        {
            int startIndex = rampStartIndex % resolution;
            int currIndex;
            int nextIndex;

            rampLength = Mathf.Clamp(rampLength, 0, resolution);
            Vector3 p1 = Vector3.zero;
            Vector3 p2 = Vector3.zero;
            Vector3 p3 = Vector3.zero;
            Vector3 p4 = Vector3.zero;

            Vector3 bp1 = Vector3.zero;
            Vector3 bp2 = Vector3.zero;

            for (int i = 0; i < rampLength; i++)
            {
                currIndex = (startIndex + i) % resolution;
                nextIndex = (currIndex + 1) % resolution;
                float p1Height = Mathf.Clamp(rampCurve.Evaluate(i / (float)rampLength), 0, 1) * height;
                float p2Height = Mathf.Clamp(rampCurve.Evaluate((i + 1) / (float)rampLength), 0, 1) * height;

                Vector3 correctedP1 = Vector3.Lerp(fPoints[currIndex], new Vector3(tPoints[currIndex].x, fPoints[currIndex].y, tPoints[currIndex].z), p1Height/height);
                Vector3 correctedP2 = Vector3.Lerp(fPoints[nextIndex], new Vector3(tPoints[nextIndex].x, fPoints[nextIndex].y, tPoints[nextIndex].z), p2Height / height);

         


                //on wall
                p1 = correctedP1 + new Vector3(0, p1Height, 0);
                p2 = correctedP2 + new Vector3(0, p2Height, 0);



                //off wall
                if (i == 0)
                {
                    p3 = p1 + fPoints[currIndex].normalized * widthCurve.Evaluate((i + 1) / (float)rampLength) * rampWidth;
                    bp1 = fPoints[currIndex] + fPoints[currIndex].normalized * widthCurve.Evaluate((i + 1) / (float)rampLength) * rampWidth;
                }
               
                p4 = p2 + fPoints[nextIndex].normalized * widthCurve.Evaluate((i + 1) / (float)rampLength) * rampWidth;
                bp2 = fPoints[nextIndex] + fPoints[nextIndex].normalized * widthCurve.Evaluate((i + 1) / (float)rampLength) * rampWidth;




                if (i == 0)
                {
                    //draw start cap
                    res.Add(p1);
                    res.Add(bp1);
                    res.Add(fPoints[currIndex]);

                    res.Add(p1);
                    res.Add(p3);
                    res.Add(bp1);
                    colorRes.AddRange(Enumerable.Repeat(stone, 6).ToArray());

                }
                 else if (i == rampLength - 1)
                {
                    //draw start cap
                    res.Add(p2);
                    res.Add(fPoints[nextIndex]);
                    res.Add(bp2);

                    res.Add(p4);
                    res.Add(p2);
                    res.Add(bp2);
                    colorRes.AddRange(Enumerable.Repeat(stone, 6).ToArray());
                }


                //top surface
                res.Add(p1);
                res.Add(p2);
                res.Add(p3);

                res.Add(p2);
                res.Add(p4);
                res.Add(p3);
                //==================
                colorRes.AddRange(Enumerable.Repeat(grass, 6).ToArray());


                //recalculate bottom verts
                //outer wall
                res.Add(bp1);
                res.Add(p3);
                res.Add(bp2);


                res.Add(p4);
                res.Add(bp2);
                res.Add(p3);

                //==================
                colorRes.AddRange(Enumerable.Repeat(stone, 6).ToArray());

                p3 = p4;
                bp1 = bp2;
            }


            //res.Add(fPoints[(startIndex + 1) % resolution] + new Vector3(0, rampCurve.Evaluate(i + 1 / (float)rampLength) * height, 0));
            //res.Add(fPoints[(startIndex + 1) % resolution] + new Vector3(0, rampCurve.Evaluate(i + 1 / (float)rampLength) * height, 0) +fPoints[(startIndex + 1) % resolution].normalized * rampWidth);
            //res.Add(fPoints[(startIndex)] + fPoints[(startIndex)].normalized * rampWidth);


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



}
