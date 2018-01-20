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
    public float PartSize { get { return _partSize; } }

    private GameObject _player;
    private Dictionary<string, MapPart> _map = new Dictionary<string, MapPart>();
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
        int mappedX = MapFloatCoord(pos.x);
        int mappedY = MapFloatCoord(pos.y);
        if(_lastX != mappedX || _lastY != mappedY)
        {
            UpdateMapAround(_lastX, _lastY, DisableMap);
            _lastX = mappedX;
            _lastY = mappedY;
        }

        UpdateMapAround(mappedX, mappedY, EnableMap);
    }

    public int MapFloatCoord(float coord)
    {
        if(coord < 0)
        {
            coord -= _partSize;
        }
        return (int)(coord / _partSize);
    }

    public float MapIntCoord(int coord)
    {
        return coord * _partSize;
    }

    private bool _navMeshRebuild = false;
    private int[] _dx = new int[] { 0, -1, 1, 0, 0, 1, -1, 1, -1 }; // order matters
    private int[] _dy = new int[] { 0, 0, 0, -1, 1, 1, 1, -1, -1 };
    private void UpdateMapAround(int x, int y, Action<MapPart, int, int> action)
    {
        if (_navMeshRebuild)
        {
            BuildNavMesh();
            _navMeshRebuild = false;
        }
    
        for (int k = 0; k < 9; k++)
        {
            UpdateMapAt(x + _dx[k], y + _dy[k], action);
        }
    }

    private void BuildNavMesh()
    {
        foreach(var go in _map)
        {
            go.Value.SetVisible(true);
        }
        GameSystem.Get().BuildNavMesh();
        foreach(var go in _map)
        {
            go.Value.SetVisible(false);
        }
    }

    private String GetKey(int x, int y)
    {
        return x + "|" + y;
    }

    public MapPart PartAt(int x, int y)
    {
        MapPart res;
        _map.TryGetValue(GetKey(x, y), out res);
        return res;
    }

    public List<MapPart> ValidPartsAround(int x, int y)
    {
        List<MapPart> validParts = new List<MapPart>();
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if(dx == 0 && dy == 0)
                {
                    continue;
                }

                var part = PartAt(x + dx, y + dy);
                if(part != null)
                {
                    validParts.Add(part);
                }
            }
        }

        return validParts;
    }

    private void UpdateMapAt(int x, int y, Action<MapPart, int, int> action)
    {
        action(PartAt(x,y), x, y);
    }

    private void EnableMap(MapPart part, int x, int y)
    {
        if(part == null)
        {
            part = new MapPart(this, x, y);
            _map.Add(GetKey(x,y), part);
            _navMeshRebuild = true;
        }
        part.SetVisible(true);
    }

    private void DisableMap(MapPart part, int x, int y)
    {
        if (part != null)
        {
            part.SetVisible(false);
        }
    }
}
