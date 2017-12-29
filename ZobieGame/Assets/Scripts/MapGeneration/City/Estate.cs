using UnityEngine;
using System.Collections.Generic;

public class Estate
{
    private Rect _rect;
    private List<Rect> _houses = new List<Rect>(); // placeholders for houses
    private HouseSettings _houseSettings;
    private CitySettings _citySettings;

    public Estate(Rect rect)
    {
        _rect = rect;
        _houseSettings = GeneratorAssets.Get().HouseSettings;
        _citySettings = GeneratorAssets.Get().CitySettings;
    }

    public void Generate()
    {
        _houses.Clear();

        float horizontalEdge = RandomHouseEdge(_rect.width);
        int horizontalNum = CalcNumber(horizontalEdge, _rect.width);

        float verticalEdge = RandomHouseEdge(_rect.height);
        int verticalNum = CalcNumber(verticalEdge, _rect.height);

        float horOffset = CalcFinalOffset(horizontalNum, _rect.width, horizontalEdge);
        float verOffset = CalcFinalOffset(verticalNum, _rect.height, verticalEdge);

        float space = _citySettings.SpaceBetweenHouses;
        for(int i=0; i<verticalNum; i++)
        {
            for(int j=0; j<horizontalNum; j++)
            {
                float x = _rect.xMin + horOffset + j * (space + horizontalEdge);
                float y = _rect.yMin + verOffset + i * (space + verticalEdge);

                _houses.Add(new Rect(x, y, horizontalEdge, verticalEdge));
            }
        }
    }

    private float RandomHouseEdge(float size)
    {
        float value = Random.Range(_houseSettings.MinHouseEdge, _houseSettings.MaxHouseEdge);
        return Mathf.Min(value, size - 2 * Offset() - float.Epsilon);
    }

    private int CalcNumber(float edge, float size)
    {
        size -= Offset() * 2;
        int num = 1;
        while(size >= edge) 
        {
            size -= edge + _citySettings.SpaceBetweenHouses;
            num++;
        }

        return num - 1;
    }

    private float CalcFinalOffset(int num, float size, float edge)
    {
        float space = size - num * edge - (num-1) * _citySettings.SpaceBetweenHouses;
        return space / 2; 
    }

    private float Offset()
    {
        return _citySettings.EstateStreetOffset + _citySettings.StreetSize / 2;
    }

    public GameObject Make(House h)
    {
        GameObject go = Utils.TerrainObject("Estate");
        foreach (var houseRect in _houses)
        {
            h.Generate(houseRect);
            var houseGO = h.Make();
            houseGO.SetParent(go);
        }

        return go;
    }
}