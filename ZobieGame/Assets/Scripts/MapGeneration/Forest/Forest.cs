using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MapObject {
    private List<Tree> _trees = new List<Tree>();
    private Street street = null;
    public Forest(Rect rect) : base(rect)
    {
    }

    public override void Generate()
    {
        _trees.Clear();
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
                    AddTree(x, y, treeRectSize);
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

        Rect streetRect = Utils.SegmentToRect(p1, p2, GeneratorAssets.Get().CitySettings.StreetSize);
        street = new Street(streetRect);
    }

    private void AddTree(float x, float y, float treeRectSize)
    {
        Rect treeRect = new Rect(x, y, treeRectSize, treeRectSize);
        if(street != null && street.Rect.Overlaps(treeRect))
        {
            return;
        }

        Tree tree = new Tree(treeRect);
        tree.Generate();

        _trees.Add(tree);
    }

    public override GameObject Make()
    {
        GameObject go = Utils.TerrainObject("Forest");
        foreach (var tree in _trees)
        {
            var treeGO = tree.Make();
            treeGO.SetParent(go);
        }

        if(street != null)
        {
            street.Make().SetParent(go);
        }

        return go;
    }
}
