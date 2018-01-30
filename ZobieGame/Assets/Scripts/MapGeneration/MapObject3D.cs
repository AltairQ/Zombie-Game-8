using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapObject3D : MapObject
{
    private Object3DSetting[] _partSettings;
    private string _name;
    public MapObject3D(string name, Object3DSetting partSetting, Rect rect) : 
        this(name, new Object3DSetting[] { partSetting }, rect)
    {
    }

    public MapObject3D(string name, Object3DSetting[] partSettings, Rect rect) : base(rect)
    {
        _name = name;
        _partSettings = partSettings;
    }

    private List<float> _partHeights = new List<float>();
    public override void Generate()
    {
        _partHeights.Clear();
        foreach (var setting in _partSettings)
        {
            float height = Random.Range(setting.MinHeight, setting.MaxHeight);
            _partHeights.Add(height);
        }
    }

    public override GameObject Make()
    {
        GameObject go = Utils.TerrainObject(_name);
        for(int i=0; i<_partSettings.Length; i++)
        {
            var partGO = MakePart2(i);
            partGO.SetParent(go);
            //go.Combine(partGO);
            //partGO.Clear(GameObject.DestroyImmediate);
        }
        return go;
    }

    private GameObject MakePart(int k)
    {
        float partYOff = 0;
        for(int i = 0; i<k; i++)
        {
            partYOff += _partHeights[i];
        }

        var setting = _partSettings[k];
        float height = _partHeights[k];
        var go = Utils.TerrainObject(setting.Name);
        int maxN = setting.SegmentsNumber - 1;
        float segmentHeight = height / maxN;
        float maxScale = 0f;
        for (int i=0; i<=maxN; i++)
        {
            float sizeScale = setting.Shape.Evaluate((float)i / maxN);
            maxScale = Mathf.Max(sizeScale, maxScale);
            var segment = MakeSegment(sizeScale, segmentHeight, segmentHeight * i + partYOff);
            segment.SetMaterial(setting.Material);
            segment.SetParent(go);
        }

        go.transform.position = Rect.Center(0f);
        var box = go.AddComponent<BoxCollider>();
        box.size = new Vector3(Rect.width * maxScale, height, Rect.height * maxScale);
        box.center = new Vector3(0f, partYOff + height / 2, 0f);

        return go;
    }

    private GameObject MakeSegment(float sizeScale, float height, float yOff)
    {
        var segment = Utils.TerrainObject(PrimitiveType.Cube, "Segment");
        segment.GetComponent<BoxCollider>().enabled = false;
        Vector2 rectSize = Rect.size * sizeScale;
        segment.transform.localScale = new Vector3(rectSize.x, height, rectSize.y);
        segment.transform.position = new Vector3(0f, yOff + height/2, 0f);

        return segment;
    }

    private GameObject MakePart2(int k)
    {
        float partYOff = 0;
        for (int i = 0; i < k; i++)
        {
            partYOff += _partHeights[i];
        }

        var setting = _partSettings[k];
        float height = _partHeights[k];
        var go = Utils.TerrainObject(setting.Name);
        int maxN = setting.SegmentsNumber - 1;
        float segmentHeight = height / maxN;
        float maxScale = 0f;
        var pattern = PatternGenerator.GeneratePattern(1);
        SegmentMeshGenerator meshGen = new SegmentMeshGenerator(pattern);
        for (int i = 0; i <= maxN; i++)
        {
            float sizeScale = setting.Shape.Evaluate((float)i / maxN);
            maxScale = Mathf.Max(sizeScale, maxScale);
            meshGen.AddSegment(Rect.width * sizeScale, segmentHeight * i + partYOff);
        }

        go.transform.position = Rect.Center(0f);
        var box = go.AddComponent<BoxCollider>();
        box.size = new Vector3(Rect.width * maxScale, height, Rect.height * maxScale);
        box.center = new Vector3(0f, partYOff + height / 2, 0f);

        go.AddComponent<MeshRenderer>().material = setting.Material;
        go.AddComponent<MeshFilter>().mesh = meshGen.GenerateMesh();

        return go;
    }
}
