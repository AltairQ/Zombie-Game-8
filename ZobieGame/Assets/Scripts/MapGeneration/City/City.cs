using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{   
    private Rect _rect;
    private List<Street> _streets = new List<Street>();
    private List<Estate> _estates = new List<Estate>();

    private CitySettings _settings;

    public void Generate(Rect rect)
    {
        _rect = rect;
        _settings = GeneratorAssets.Get().CitySettings;

        _streets.Clear();
        _estates.Clear();

        GenerateEstates(_rect);
    }

    private void AddStreet(Vector2 p1, Vector2 p2)
    {
        _streets.Add(new Street(p1, p2, _settings.StreetSize));
    }

    private void AddEstate(Rect rect)
    {
        var estate = new Estate(rect);
        estate.Generate();
        _estates.Add(estate);
    }

    private bool CanBeSplitVertically(Rect rect)
    {
        return rect.width > _settings.MinEstateEdge * 2;
    }
    private bool CanBeSplitHorizontally(Rect rect)
    {
        return rect.height > _settings.MinEstateEdge * 2;
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
        float minEdge = _settings.MinEstateEdge;
        if (splitVertical)
        {
            float deltaX = rect.width - 2 * minEdge;
            float randX = Random.Range(0, deltaX);
            p1 = new Vector2(rect.xMin + minEdge + randX, rect.yMin);
            p2 = p1 + new Vector2(0, rect.height);
        }
        else
        {
            float deltaY = rect.height - 2 * minEdge;
            float randY = Random.Range(0, deltaY);
            p1 = new Vector2(rect.xMin, rect.yMin + minEdge + randY);
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

        float minArea = _settings.MinEstateEdge * _settings.MinEstateEdge; 
        float createEstate = Random.Range(0, area / minArea); // the bigger area the lower chance
        return createEstate < 1.0f;
    }

    public GameObject Make()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        GameObject go = Utils.TerrainObject("City");
        go.SetParent(gameObject);

        MakeTerrain(go);

        foreach (var street in _streets)
        {
            var streetGO = street.Make();
            streetGO.SetParent(go);
        }

        House house = GetComponent<House>();
        foreach (var estate in _estates)
        {
            var estateGO = estate.Make(house);
            estateGO.SetParent(go);
        }

        return go;
    }

    private GameObject MakeTerrain(GameObject parent)
    {
        var terrain = _rect.ToTerrainQuad("Terrain", ObjectHeight.Ground);
        terrain.SetParent(parent);
        terrain.SetMaterial(GeneratorAssets.Get().TerrainMaterial);

        return terrain;
    }
}