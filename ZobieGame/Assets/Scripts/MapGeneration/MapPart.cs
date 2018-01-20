using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapPart
{
    public enum Type
    {
        City,
        Forest
    }

    private MapSystem _map;
    private GameObject _go;
    private MapObject _mapObject;

    public int X { get; private set; }
    public int Y { get; private set; }
    public Type PartType { get; private set; }

    public MapPart(MapSystem map, int x, int y)
    {
        _map = map;
        X = x;
        Y = y;

        ChooseType();
        Create();
    }

    public void SetVisible(bool val)
    {
        _go.SetActive(val);
    }
    
    private void ChooseType()
    {
        var around = _map.ValidPartsAround(X, Y);
        PartType = Type.Forest;
        if (X%2==0 && Y%2==0)
        {
            PartType = Type.City;
        }
    }

    private void Create()
    {
        Rect rect = new Rect(_map.MapIntCoord(X), _map.MapIntCoord(Y), _map.PartSize, _map.PartSize);

        if (PartType == Type.City)
        {
            _mapObject = CreateCity(rect);
        }
        else
        {
            _mapObject = CreateForest(rect);
        }

        _mapObject.Generate();
        _go = _mapObject.Make();
        _go.SetParent(_map.gameObject);
        Utils.CreateGround(rect, _go);
    }

    private List<Vector2> EdgePoints()
    {
        if (PartType == Type.City)
        {
            return (_mapObject as City).InitStreetPoints;
        }
        if (PartType == Type.Forest)
        {
            return (_mapObject as Forest).InitStreetPoints;
        }

        Debug.LogError("MapPart.EdgePoints() unknown PartType");
        return null;
    }

    private List<Vector2> FindValidEdgePoints(Rect rect)
    {
        List<Vector2> validPoints = new List<Vector2>();
        foreach (var part in _map.ValidPartsAround(X, Y))
        {
            validPoints.AddRange(part.EdgePoints());
        }

        validPoints.RemoveAll(v => !rect.ContainsOnEdge(v));
        return validPoints;
    }

    private MapObject CreateCity(Rect rect)
    {
        City city = new City(rect);

        List<Vector2> validPoints = FindValidEdgePoints(rect);
        foreach(var p in validPoints)
        {
            city.SetInitStreet(p);
        }

        return city;
    }

    private MapObject CreateForest(Rect rect)
    {
        Forest forest = new Forest(rect);

        List<Vector2> validPoints = FindValidEdgePoints(rect);
        foreach (var p in validPoints)
        {
            forest.SetStreet(p);
        }

        return forest;
    }
}
