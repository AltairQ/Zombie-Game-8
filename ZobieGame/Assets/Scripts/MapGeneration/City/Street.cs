using System.Collections.Generic;
using UnityEngine;

public class Street : MapObject
{
    private Rect _road;
    private bool _roadFlipped;

    private List<Rect> _lampPos = new List<Rect>();
    private List<Rect> _carPos = new List<Rect>();
    private CitySettings _settings;
    public Street(Rect rect) : base(rect)
    {
        _settings = GeneratorAssets.Get().CitySettings;
    }

    protected override void DoGenerate()
    {
        _road = Rect;
        if(_road.width < _road.height)
        {
            _road.x += _road.width * (1 - _settings.RoadPercSize) / 2;
            _road.width = _road.width * _settings.RoadPercSize;
            _roadFlipped = false;
        }
        else
        {
            _road.y += _road.height * (1 - _settings.RoadPercSize) / 2;
            _road.height = _road.height * _settings.RoadPercSize;
            _roadFlipped = true;
        }

        GenerateLamps();

        GenerateCars();
    }

    private void GenerateLamps()
    {
        _lampPos.Clear();

        float roadSize = _roadFlipped ? _road.height : _road.width;
        float lampSize = _roadFlipped ? (Rect.height - _road.height) / 2 : (Rect.width - _road.width) / 2;
        Vector2 dPos = _roadFlipped ? new Vector2(0, 1) : new Vector2(1, 0);

        Rect test1 = new Rect()
        {
            width = lampSize,
            height = lampSize,
            center = _road.center + dPos * (roadSize / 2 + lampSize / 2)
        };
        Rect test2 = new Rect()
        {
            width = lampSize,
            height = lampSize,
            center = _road.center - dPos * (roadSize / 2 + lampSize / 2)
        };

        _lampPos.Add(test1);
        _lampPos.Add(test2);
    }

    private void GenerateCars()
    {
        _carPos.Clear();

        float roadSize = _roadFlipped ? _road.height : _road.width;
        float carWidth = roadSize / 2;
        float carHeight = carWidth * _settings.CarDWProportion;
        if(_roadFlipped)
        {
            Utils.Swap(ref carWidth, ref carHeight);
        }

        Vector2 dPos = _roadFlipped ? new Vector2(0, 1) : new Vector2(1, 0);

        Rect test = new Rect()
        {
            width = carWidth,
            height = carHeight,
            center = _road.center + dPos * roadSize / 4
        };

        _carPos.Add(test);
    }

    protected override GameObject DoMake()
    {
        GameObject go = Utils.TerrainObject("Street");

        var road =  _road.ToTerrainQuad("Road", ObjectHeight.Floor);
        road.SetMaterial(GeneratorAssets.Get().RoadMaterial);
        road.GetComponent<Collider>().enabled = false;
        road.SetParent(go);

        var lamps = MakeLamps();
        lamps.SetParent(go);

        var cars = MakeCars();
        cars.SetParent(go);

        return go;
    }

    private GameObject MakeLamps()
    {
        GameObject go = Utils.TerrainObject("Lamps");
        _lampPos.ForEach(pos =>
        {
            GameObject lamp = GameObject.Instantiate(_settings.LampPrefab, go.transform);
            lamp.transform.position = pos.Center(ObjectHeight.Floor);
            lamp.transform.localScale = Vector3.one * pos.width; // lamps shoud be squared
        });

        return go;
    }

    private GameObject MakeCars()
    {
        GameObject go = Utils.TerrainObject("Cars");

        _carPos.ForEach(pos =>
        {
            GameObject car = GameObject.Instantiate(_settings.CarPrefab, go.transform);
            car.transform.position = pos.Center(ObjectHeight.Floor);

            float scale = pos.width;
            if(pos.width > pos.height)
            {
                car.transform.localEulerAngles = new Vector3(0, 90, 0);
                scale = pos.height;
            }
            car.transform.localScale = Vector3.one * scale; // cars shoud have proper proportions
        });

        return go;
    }
}