using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SegmentMeshGenerator
{
    /* !!! pattern has to be clockwise !!! */
    private List<Vector2> _segmentPattern;
    public SegmentMeshGenerator(List<Vector2> segmentPattern)
    {
        _segmentPattern = segmentPattern;
    }

    private List<List<Vector3>> _segments = new List<List<Vector3>>();
    public void AddSegment(float scale, float height)
    {
        List<Vector3> newSegment = new List<Vector3>(_segmentPattern.Capacity);
        foreach(Vector2 v in _segmentPattern)
        {
            newSegment.Add(new Vector3(scale * v.x, height, scale * v.y));
        }
        _segments.Add(newSegment);
    }

    private List<Vector3> _vert = new List<Vector3>();
    private List<Vector3> _norm = new List<Vector3>();
    private List<Vector2> _uv = new List<Vector2>();
    private List<int> _tri = new List<int>();
    public Mesh GenerateMesh()
    {
        _vert.Clear();
        _norm.Clear();
        _uv.Clear();
        _tri.Clear();

        GenerateSurface(_segments[0], false);
        for(int i=0; i < _segments.Count -1; i++)
        {
            GenerateSide(_segments[i], _segments[i + 1]);
        }
        GenerateSurface(_segments[_segments.Count-1], true);

        Mesh mesh = new Mesh {
            vertices = _vert.ToArray(),
            normals = _norm.ToArray(),
            uv = _uv.ToArray(),
            triangles = _tri.ToArray()
        };

        return mesh;
    }

    private int LastTriIdx()
    {
        return _tri.Count == 0 ? -1 : _tri[_tri.Count - 1];
    }


    private void GenerateSurface(List<Vector3> segment, bool atTop)
    {
        var triangles = Triangulation.TriangulateConcavePolygon(segment);
        foreach (var triangle in triangles)
        {
            _uv.Add(new Vector2(0f, 0f));
            _uv.Add(new Vector2(1f, 0f));
            _uv.Add(new Vector2(0f, 1f));

            Vector3 normal = atTop ? Vector3.up : Vector3.down;
            _norm.Add(normal);
            _norm.Add(normal);
            _norm.Add(normal);

            Vector3 v1 = triangle.v1.Position;
            Vector3 v2 = triangle.v2.Position;
            Vector3 v3 = triangle.v3.Position;

            Vector3 cross = Vector3.Cross(v2 - v1, v3 - v1).normalized;
            if (Utils.TheSame(cross.y, normal.y))
            {
                _vert.Add(v1);
                _vert.Add(v2);
                _vert.Add(v3);
            }
            else
            {
                _vert.Add(v1);
                _vert.Add(v3);
                _vert.Add(v2);
            }

            int lastIdx = LastTriIdx();
            _tri.Add(lastIdx + 1);
            _tri.Add(lastIdx + 2);
            _tri.Add(lastIdx + 3);
        }
    }

    private void GenerateSide(List<Vector3> lower, List<Vector3> upper)
    {
        int size = lower.Count;
        for(int i = 0; i < lower.Count; i++)
        {
            Vector3 lowV1 = lower[i];
            Vector3 lowV2 = lower[(i+1)%size];
            Vector3 upV1 = upper[i];
            Vector3 upV2 = upper[(i + 1) % size];

            _vert.Add(lowV1);
            _vert.Add(lowV2);
            _vert.Add(upV1);
            _vert.Add(upV2);

            _uv.Add(new Vector2(0f, 0f));
            _uv.Add(new Vector2(0f, 1f));
            _uv.Add(new Vector2(1f, 0f));
            _uv.Add(new Vector2(1f, 1f));

            var normal = Vector3.Cross(lowV2 - lowV1, upV2 - lowV1);
            _norm.Add(normal);
            _norm.Add(normal);
            _norm.Add(normal);
            _norm.Add(normal);


            int lastIdx = LastTriIdx();   
            _tri.Add(lastIdx + 1);
            _tri.Add(lastIdx + 4);
            _tri.Add(lastIdx + 3);
            _tri.Add(lastIdx + 1);
            _tri.Add(lastIdx + 2);
            _tri.Add(lastIdx + 4);
        }
    }
}
