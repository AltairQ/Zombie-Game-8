using System.Collections.Generic;
using UnityEngine;

public class Street : MapObject
{
    private class PosInfo
    {
        public Rect rect;
        public float rotation = 0f;
    }
    private City _city;

    private Rect _road;
    private bool _roadFlipped;

    private List<PosInfo> _lampInfos = new List<PosInfo>();
    private List<PosInfo> _carInfos = new List<PosInfo>();
    private CitySettings _settings;
    public Street(Rect rect) : base(rect)
    {
        _city = null;
        _settings = GeneratorAssets.Get().CitySettings;
    }
    public Street(City city, Rect rect) : base(rect)
    {
        _city = city;
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

    private PosInfo GetLampInfo(Vector2 roadPos, Vector2 perp, float roadSize, float lampSize, bool otherSide)
    {
        int sign = otherSide ? -1 : 1;
        var rect = new Rect()
        {
            width = lampSize,
            height = lampSize,
            center = roadPos + sign * perp * (roadSize / 2 + lampSize / 2)
        };

        float rotation = otherSide ? 0 : 180;
        if(_roadFlipped)
        {
            rotation -= 90;
        }

        return new PosInfo()
        {
            rect = rect,
            rotation = rotation
        };
    }

    private PosInfo GetCarInfo(Vector2 roadPos, Vector2 perp, float roadSize, float carWidth, float carHeight, bool otherSide)
    {
        int sign = otherSide ? -1 : 1;
        var rect = new Rect()
        {
            width = carWidth * _settings.CarOnRoadPercSize,
            height = carHeight * _settings.CarOnRoadPercSize,
            center = roadPos + sign * perp * roadSize / 4
        };

        float rotation = otherSide ? 180 : 0;
        if (_roadFlipped)
        {
            rotation -= 90;
        }

        return new PosInfo()
        {
            rect = rect,
            rotation = rotation
        };
    }

    private void TryAddLamp(PosInfo lamp)
    {
        if(_city == null || !_city.CheckOnStreetCollision(this, lamp.rect))
        {
            _lampInfos.Add(lamp);
        }
    }

    private void TryAddCar(PosInfo car)
    {
        if (_city == null || !_city.CheckOnStreetCollision(this, car.rect))
        {
            _carInfos.Add(car);
        }
    }

    private void GenerateLamps()
    {
        _lampInfos.Clear();

        float roadSize = _roadFlipped ? _road.height : _road.width;
        float lampSize = _roadFlipped ? (Rect.height - _road.height) / 2 : (Rect.width - _road.width) / 2;
        Vector2 perp = _roadFlipped ? new Vector2(0, 1) : new Vector2(1, 0); // perpendicular
        Vector2 paral = _roadFlipped ? new Vector2(1, 0) : new Vector2(0, 1); // parallel

        Vector2 curPos = _road.LeftTop() + perp * roadSize / 2 + paral * Random.Range(0, _settings.SpaceBetweenLamps);
        while(_road.ContainsE(curPos))
        {
            TryAddLamp(GetLampInfo(curPos, perp, roadSize, lampSize, false));
            TryAddLamp(GetLampInfo(curPos, perp, roadSize, lampSize, true));
            curPos += _settings.SpaceBetweenLamps * paral;
        }
    }

    private void GenerateCars()
    {
        _carInfos.Clear();

        float roadSize = _roadFlipped ? _road.height : _road.width;
        float carWidth = roadSize / 2;
        float carHeight = carWidth * _settings.CarDWProportion;
        if(_roadFlipped)
        {
            Utils.Swap(ref carWidth, ref carHeight);
        }

        Vector2 perp = _roadFlipped ? new Vector2(0, 1) : new Vector2(1, 0); // perpendicular
        Vector2 paral = _roadFlipped ? new Vector2(1, 0) : new Vector2(0, 1); // parallel

        Vector2 curPos = _road.LeftTop() + perp * roadSize / 2 + paral * Random.Range(0, _settings.MaxSpaceBetweenCars);
        while (_road.ContainsE(curPos))
        {
            TryAddCar(GetCarInfo(curPos, perp, roadSize, carWidth, carHeight, Random.Range(0, 2) == 0));
            curPos += Random.Range(_settings.MinSpaceBetweenCars, _settings.MaxSpaceBetweenCars) * paral;
        }
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
        _lampInfos.ForEach(info =>
        {
            var pos = info.rect;
            GameObject lamp = GameObject.Instantiate(_settings.LampPrefab, go.transform);
            lamp.transform.position = pos.Center(ObjectHeight.Floor);
            lamp.transform.localScale = Vector3.one * pos.width; // lamps shoud be squared
            lamp.transform.localEulerAngles = new Vector3(0, info.rotation, 0);
        });

        return go;
    }

    private GameObject MakeCars()
    {
        GameObject go = Utils.TerrainObject("Cars");

        _carInfos.ForEach(info =>
        {
            var pos = info.rect;

            GameObject car = GameObject.Instantiate(_settings.CarPrefab, go.transform);
            car.transform.position = pos.Center(ObjectHeight.Floor);

            float scale = pos.width;
            if(pos.width > pos.height)
            {
                scale = pos.height;
            }
            car.transform.localScale = Vector3.one * scale; // cars shoud have proper proportions
            car.transform.localEulerAngles = new Vector3(0, info.rotation, 0);
        });

        return go;
    }
}