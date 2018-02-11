using System.Collections.Generic;
using UnityEngine;

public class Room : MapObject
{
    private bool _hasLoot = false;
    private HouseSettings _settings;
    private List<Vector2> _doorCenters = new List<Vector2>();
    public Room(Rect rect) : base(rect)
    {
        _settings = GeneratorAssets.Get().HouseSettings;
    }

    public void AddDoorCenter(Vector2 doorCenter)
    {
        _doorCenters.Add(doorCenter);
    }

    protected override void DoGenerate()
    {
        _hasLoot = _settings.RoomSpawnChance >= Random.Range(0f, 1f);
        GenerateWallAreas();
    }

    List<Rect> _wallRects = new List<Rect>();
    Rect _centralRect = new Rect();
    private void GenerateWallAreas()
    {
        _wallRects.Clear();
        _centralRect.width = _centralRect.height = 0;

        float shift = _settings.DoorSize;
        if (shift * 2 > Mathf.Min(Rect.width, Rect.height))
        {
            return;
        }

        _wallRects.Add(new Rect(Rect.xMin, Rect.yMin, Rect.width, shift));
        _wallRects.Add(new Rect(Rect.xMin, Rect.yMax - shift, Rect.width, shift));
        _wallRects.Add(new Rect(Rect.xMin, Rect.yMin + shift, shift, Rect.height - 2 * shift));
        _wallRects.Add(new Rect(Rect.xMax - shift, Rect.yMin + shift, shift, Rect.height - 2 * shift));
        _centralRect = new Rect(Rect.xMin + shift, Rect.yMin + shift, Rect.width - 2 * shift, Rect.height - 2 * shift);

        RemoveRectAtDoor();
    }

    private const float _smallShift = 0.1f;
    private float[] dx = new float[] { _smallShift, 0, -_smallShift, 0 };
    private float[] dy = new float[] { 0, _smallShift, 0, -_smallShift };
    private void RemoveRectAtDoor()
    {
        foreach (Vector2 doorCenter in _doorCenters)
        {
            for(int k = 0; k<4; k++)
            {
                _wallRects.RemoveAll(rect => rect.Contains(doorCenter + new Vector2(dx[k], dy[k])));
            }
        }
    }

    protected override GameObject DoMake()
    {
        GameObject items = null; // will be initialised if necessary in  MakeFurniture 
        var settings = GeneratorAssets.Get().FurnitureSettings;

        if(_hasLoot)
        {
            MakeLootItem(ref items);
        }
        else if (_centralRect.Area() > (_settings.MinRoomArea + _settings.MaxRoomArea) / 2)
        {
            MakeFurniture(settings.GetRandomCentralFurniture(), ref items, _centralRect);
        }

        foreach (var wallRect in _wallRects)
        {
            MakeFurniture(settings.GetRandomWallFurniture(), ref items, wallRect);
        }

        return items;
    }

    private void MakeLootItem(ref GameObject parent)
    {
        if (parent == null)
        {
            parent = Utils.TerrainObject("Items");
        }

        var item = GeneratorAssets.Get().LootSettings.GetRandomItem();
        item.transform.position = Rect.Center(ObjectHeight.Floor) + new Vector3(0, item.transform.position.y, 0);
        item.SetParent(parent);
    }

    private void MakeFurniture(FurnitureSettings.FurnitureSetting setting, ref GameObject parent, Rect rect)
    {
        if (parent == null)
        {
            parent = Utils.TerrainObject("Items");
        }

        var item = setting.gameObject;
        float height = item.transform.lossyScale.y + setting.yShift;
        item.transform.position = rect.Center(ObjectHeight.Floor) + new Vector3(0, height, 0);
        item.SetParent(parent);

        Vector2 defaultNormal = Vector2.up;
        Vector2 currentNormal = (Rect.center - rect.center).normalized;
        float angle = Vector2.Angle(defaultNormal, currentNormal);
        if(Utils.TheSame(currentNormal.x, -1))
        {
            angle += 180f;
        }

        item.transform.Rotate(Vector3.forward, angle);
    }
}
