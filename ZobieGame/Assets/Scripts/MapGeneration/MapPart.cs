using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class MapPart
{
    public enum Type
    {
        City,
        Forest
    }

    private MapSystem _map;
    private GameObject _go;
    private GameObject _ground;
    private Rect _rect;
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
        InitRect();

        float width = _rect.width;
        float height = _rect.height;
        Rect biggerRect = _rect;
        float scale = 0.02f;
        biggerRect.yMin -= height * scale;
        biggerRect.yMax += height * scale;
        biggerRect.xMin -= width * scale;
        biggerRect.xMax += width * scale;

        _ground = Utils.CreateGround(biggerRect, _map.gameObject);
    }

    private void ChooseType()
    {
        var around = _map.ValidPartsAround(X, Y);
        PartType = Type.Forest;
        if (X % 2 == 0 && Y % 2 == 0)
        {
            PartType = Type.City;
        }
    }

    private void InitRect()
    {
        _rect = new Rect(_map.MapIntCoord(X), _map.MapIntCoord(Y), _map.PartSize, _map.PartSize);

        if (PartType == Type.City)
        {
            _mapObject = CreateCity(_rect);
        }
        else
        {
            _mapObject = CreateForest(_rect);
        }

        _mapObject.Generate();
    }

    public void SetVisible(bool val)
    {
        if(_go != null)
        {
            _go.SetActive(val);
        }
    } 

    public void TryCreate()
    {
        if(IsCreated() || !_mapObject.IsMade())
        {
            return;
        }

        _go = _mapObject.Make();
        _go.SetParent(_map.gameObject);
        _ground.SetParent(_go);

        if(!_map.GlobalNavMeshgeneration)
        {
            GenerateLocalNavMesh();
        }     
    }

    private static int[] dx = new int[] { -1, 1, 0, 0 };
    private static int[] dy = new int[] { 0, 0, -1, 1 };
    private void GenerateLocalNavMesh()
    {
        var navMesh = _go.AddComponent<NavMeshSurface>();
        navMesh.collectObjects = CollectObjects.Children;
        navMesh.layerMask = LayerMask.GetMask("Terrain");
        navMesh.BuildNavMesh();

        for (int k = 0; k < 4; k++)
        {
            var part = _map.PartAt(X + dx[k], Y + dy[k]);
            if (part != null && part.IsCreated())
            {
                ConnectWith(part);
            }
        }
    }

    private void ConnectWith(MapPart other)
    {
        var myPos = _ground.transform.position;
        var otherPos = other._ground.transform.position;
        var middle = (myPos + otherPos) / 2;

        GameObject connector = new GameObject("NavMeshConnector");
        connector.transform.position = middle;
        connector.SetParent(_map.gameObject);

        var link = connector.AddComponent<NavMeshLink>();
        link.startPoint = (myPos - middle).normalized/3;
        link.endPoint = (otherPos - middle).normalized/3;
        link.width = _map.PartSize;
    }

    public bool IsCreated()
    {
        return _go != null;
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
