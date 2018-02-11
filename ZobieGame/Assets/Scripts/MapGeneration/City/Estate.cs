using UnityEngine;
using System.Collections.Generic;

public class Estate : MapObject
{
    private List<House> _houses = new List<House>();
    private HouseSettings _houseSettings;
    private CitySettings _citySettings;

    public Estate(Rect rect) : base(rect)
    {
        _houseSettings = GeneratorAssets.Get().HouseSettings;
        _citySettings = GeneratorAssets.Get().CitySettings;
    }

    protected override void DoGenerate()
    {
        _houses.Clear();

        float horizontalEdge = RandomHouseEdge(Rect.width);
        int horizontalNum = CalcNumber(horizontalEdge, Rect.width);

        float verticalEdge = RandomHouseEdge(Rect.height);
        int verticalNum = CalcNumber(verticalEdge, Rect.height);

        float horOffset = CalcFinalOffset(horizontalNum, Rect.width, horizontalEdge);
        float verOffset = CalcFinalOffset(verticalNum, Rect.height, verticalEdge);

        float space = _citySettings.SpaceBetweenHouses;
        for(int i=0; i<verticalNum; i++)
        {
            for(int j=0; j<horizontalNum; j++)
            {
                float x = Rect.xMin + horOffset + j * (space + horizontalEdge);
                float y = Rect.yMin + verOffset + i * (space + verticalEdge);

                AddHouse(x, y, horizontalEdge, verticalEdge);
            }
        }
    }

    private void AddHouse(float x, float y, float width, float height)
    {
        Rect houseRect = new Rect(x, y, width, height);
        House house = new House(houseRect);
        house.Generate();
        _houses.Add(house);
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

    protected override GameObject DoMake()
    {
        GameObject go = Utils.TerrainObject("Estate");
        foreach (var house in _houses)
        {
            var houseGO = house.Make();
            houseGO.SetParent(go);
        }

        return go;
    }
}