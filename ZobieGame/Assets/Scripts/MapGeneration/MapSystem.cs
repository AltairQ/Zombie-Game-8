using UnityEngine;
using System.Collections.Generic;
using System;

public class MapSystem : MonoBehaviour
{
    private static MapSystem _instance = null;
    public static MapSystem Get()
    {
        return _instance;
    }

    [SerializeField]
    private float _partSize = 100;

    private GameObject _player;
    private Dictionary<string, GameObject> _map = new Dictionary<string, GameObject>();
    private void Awake()
    {
        _instance = this;
    }

    public void Init(GameObject player)
    {
        _player = player;
        UpdateMapForPlayer();
    }

    private void Update()
    {
        if(_player == null)
        {
            return;
        }

        UpdateMapForPlayer();
    }

    private int _lastX = 0;
    private int _lastY = 0;
    private void UpdateMapForPlayer()
    {
        Vector2 pos = _player.Get2dPos();
        int mappedX = MapCoordinate(pos.x);
        int mappedY = MapCoordinate(pos.y);
        if(_lastX != mappedX || _lastY != mappedY)
        {
            UpdateMapAround(_lastX, _lastY, DisableMap);
            _lastX = mappedX;
            _lastY = mappedY;
        }

        UpdateMapAround(mappedX, mappedY, EnableMap);
    }

    private int MapCoordinate(float coord)
    {
        if(coord < 0)
        {
            coord -= _partSize;
        }
        return (int)(coord / _partSize);
    }

    private bool _navMeshRebuild = false;
    private void UpdateMapAround(int x, int y, Action<GameObject, int, int> action)
    {
        if (_navMeshRebuild)
        {
            BuildNavMesh();
            _navMeshRebuild = false;
        }

        for (int dx = -1; dx <= 1; dx++)
        {
            for(int dy = -1; dy <= 1; dy++)
            {
                UpdateMapAt(x + dx, y + dy, action);
            }
        }
    }

    private void BuildNavMesh()
    {
        foreach(var go in _map)
        {
            go.Value.SetActive(true);
        }
        GameSystem.Get().BuildNavMesh();
        foreach(var go in _map)
        {
            go.Value.SetActive(false);
        }
    }

    private String GetKey(int x, int y)
    {
        return x + "|" + y;
    }

    private void UpdateMapAt(int x, int y, Action<GameObject, int, int> action)
    {
        GameObject res;
        _map.TryGetValue(GetKey(x,y), out res);
        action(res, x, y);
    }

    private void EnableMap(GameObject go, int x, int y)
    {
        if(go == null)
        {
            go = GenerateMap(x * _partSize, y * _partSize);
            go.SetParent(gameObject);
            _map.Add(GetKey(x,y), go);
            _navMeshRebuild = true;
        }
        go.SetActive(true);
    }
    private void DisableMap(GameObject go, int x, int y)
    {
        if (go != null)
        {
            go.SetActive(false);
        }
    }

    private GameObject GenerateMap(float x, float y)
    {
        //Debug.Log("gen x: " + x + " ,y: " + y);
        Rect rect = new Rect(x, y, _partSize, _partSize);
        MapObject mapObject;
        if(UnityEngine.Random.Range(0,2) == 0)
        {
            mapObject = new Forest(rect);
        }
        else
        {
            mapObject = new City(rect);
        }

        mapObject.Generate();
        GameObject go = mapObject.Make();
        Utils.CreateGround(rect, go);
        return go;
    }
}
