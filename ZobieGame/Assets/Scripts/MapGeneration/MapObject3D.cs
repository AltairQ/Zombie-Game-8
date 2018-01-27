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
            var partGO = MakePart(i);
            partGO.SetParent(go);
            go.Combine(partGO);
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
        for (int i=0; i<=maxN; i++)
        {
            float sizeScale = setting.Shape.Evaluate((float)i / maxN);
            var segment = MakeSegment(sizeScale, segmentHeight, segmentHeight * i + partYOff);
            segment.SetMaterial(setting.Material);
            segment.SetParent(go);
        }

        return go;
    }

    private GameObject MakeSegment(float sizeScale, float height, float yOff)
    {
        var segment = Utils.TerrainObject(PrimitiveType.Cube, "Segment");
        Vector2 rectSize = Rect.size * sizeScale;
        segment.transform.localScale = new Vector3(rectSize.x, height, rectSize.y);
        segment.transform.position = Rect.Center(yOff + height/2);

        return segment;
    }
}
