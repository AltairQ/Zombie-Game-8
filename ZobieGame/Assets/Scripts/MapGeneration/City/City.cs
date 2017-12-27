using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{   
    private Rect _rect;
    private List<Street> _streets = new List<Street>();
    private List<Estate> _estates = new List<Estate>();

    public float Width { get; set; }
    public float Depth { get; set; }

    public float MinEstateEdge { get; set; }
    public float StreetSize { get; set; }

    public void Generate()
    {
        _rect = new Rect(-Width / 2, -Depth / 2, Width, Depth);
        _streets.Clear();
        _estates.Clear();

        GenerateEstates(_rect);
    }

    private void AddStreet(Vector2 p1, Vector2 p2)
    {
        _streets.Add(new Street(p1, p2, StreetSize));
    }

    private void AddEstate(Rect rect)
    {
        _estates.Add(new Estate(rect));
    }

    private bool CanBeSplitVertically(Rect rect)
    {
        return rect.width > MinEstateEdge * 2;
    }
    private bool CanBeSplitHorizontally(Rect rect)
    {
        return rect.height > MinEstateEdge * 2;
    }

    private void GenerateEstates(Rect rect)
    {
        if (!CanBeSplitVertically(rect) && !CanBeSplitHorizontally(rect)) // we cannot split given rect
        {
            AddEstate(rect);
            return;
        }
        if (RandomEndEstate(rect))
        {
            AddEstate(rect);
            return;
        }

        bool splitVertical = CanBeSplitVertically(rect);
        if (CanBeSplitHorizontally(rect) && CanBeSplitVertically(rect))
        {
            splitVertical = Random.Range(0, 2) == 0;
        }

        Vector2 p1, p2;
        if (splitVertical)
        {
            float deltaX = rect.width - 2 * MinEstateEdge;
            float randX = Random.Range(0, deltaX);
            p1 = new Vector2(rect.xMin + MinEstateEdge + randX, rect.yMin);
            p2 = p1 + new Vector2(0, rect.height);
        }
        else
        {
            float deltaY = rect.height - 2 * MinEstateEdge;
            float randY = Random.Range(0, deltaY);
            p1 = new Vector2(rect.xMin, rect.yMin + MinEstateEdge + randY);
            p2 = p1 + new Vector2(rect.width, 0);
        }

        AddStreet(p1, p2);
        var newRects = rect.Split(p1, p2);
        GenerateEstates(newRects[0]);
        GenerateEstates(newRects[1]);
    }

    private bool RandomEndEstate(Rect rect)
    {
        float area = rect.Area();
        //if (area > MaxEstateArea)
        //{
        //    return false;
        //}

        float createEstate = Random.Range(0, area / (MinEstateEdge*MinEstateEdge)); // the bigger area the lower chance
        return createEstate < 1.0f;
    }

    public GameObject Make()
    {
        GameObject go = new GameObject();
        go.name = "City";

        var terrain = _rect.ToQuad("Terrain", ObjectHeight.Ground);
        terrain.transform.parent = go.transform;

        foreach (var street in _streets)
        {
            var streetGO = street.Make();
            streetGO.SetParent(go);
        }

        foreach (var estate in _estates)
        {
            var estateGO = estate.Make(GetComponent<House>());
            estateGO.SetParent(go);
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        go.SetParent(gameObject);

        return go;
    }
}