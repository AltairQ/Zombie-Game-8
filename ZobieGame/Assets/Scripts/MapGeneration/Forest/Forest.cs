using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MapObject {
    private List<MapObject> _naturalObjects = new List<MapObject>();
    private Street street = null;
    private List<Vector2> _initStreetPoints = new List<Vector2>();
    public List<Vector2> InitStreetPoints { get { return _initStreetPoints; } }

    public Forest(Rect rect) : base(rect)
    {
    }

    public override void Generate()
    {
        _naturalObjects.Clear();
        //SetStreet(new Vector2(Rect.xMin, Rect.yMax - Rect.height / 2));

        float treeRectSize = 4;
        float perlinOff = Random.Range(0f, 666f);
        for (float y = Rect.yMin; y + treeRectSize < Rect.yMax; y += treeRectSize)
        {
            for (float x = Rect.xMin; x + treeRectSize < Rect.xMax; x += treeRectSize)
            {
                float val = Mathf.PerlinNoise(x + perlinOff, y + perlinOff);
                float randVal = Random.Range(0f, 1f);
                if (randVal > val)
                {
                    AddNaturalObject(x, y, treeRectSize);
                }
            }
        }
    }

    public void SetStreet(Vector2 edgePoint)
    {
        if(!Rect.ContainsOnEdge(edgePoint))
        {
            Debug.LogError("Forest.SetStreet() point not on edge!");
            return;
        }

        var points = Rect.AllPoints();
        Vector2[] p1Shift = new Vector2[]
        {
            Vector2.zero, new Vector2(-Rect.width, 0),
            new Vector2(0, -Rect.height), Vector2.zero,

        };
        Vector2[] p2Shift = new Vector2[]
        {
            new Vector2(0, Rect.height), Vector2.zero,
            Vector2.zero, new Vector2(Rect.width, 0),
        };

        Vector2 p1 = Vector2.zero, p2 = Vector2.zero;
        for (int i = 0; i < 4; i++)
        {
            if(Utils.Contains(points[i], points[i+1], edgePoint))
            {
                p1 = edgePoint + p1Shift[i];
                p2 = edgePoint + p2Shift[i];
                break;
            }
        }

        _initStreetPoints.Clear();
        _initStreetPoints.Add(p1);
        _initStreetPoints.Add(p2);

        Rect streetRect = Utils.SegmentToRect(p1, p2, GeneratorAssets.Get().CitySettings.StreetSize);
        street = new Street(streetRect);
    }

    private void AddNaturalObject(float x, float y, float treeRectSize)
    {
        Rect rect = new Rect(x, y, treeRectSize, treeRectSize);
        if(street != null && street.Rect.Overlaps(rect))
        {
            return;
        }

        MapObject naturalObject = RandomNaturalObject(rect);
        naturalObject.Generate();

        _naturalObjects.Add(naturalObject);
    }

    private MapObject RandomNaturalObject(Rect rect)
    {
        int randVal = Random.Range(0, 4);
        if (randVal == 0)
        {
            return new MapObject3D("Bush", GeneratorAssets.Get().BushSetting, rect);
        }
        else if (randVal == 1)
        {
            return new MapObject3D("Rock", GeneratorAssets.Get().RockSetting, rect);
        }
        return new MapObject3D("Tree", new[] { GeneratorAssets.Get().TreeBottomSetting, GeneratorAssets.Get().TreeTopSetting }, rect);
    }

    public override GameObject Make()
    {
        GameObject go = Utils.TerrainObject("Forest");
        foreach (var naturalObject in _naturalObjects)
        {
            var naturalGO = naturalObject.Make();
            naturalGO.SetParent(go);
        }

        if(street != null)
        {
            street.Make().SetParent(go);
        }

        return go;
    }
}
