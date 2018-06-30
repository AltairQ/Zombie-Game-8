using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Street : MapObject
{
    private Rect _road;
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
        }
        else
        {
            _road.y += _road.height * (1 - _settings.RoadPercSize) / 2;
            _road.height = _road.height * _settings.RoadPercSize;
        }
    }

    protected override GameObject DoMake()
    {
        GameObject go = Utils.TerrainObject("Street");

        var road =  _road.ToTerrainQuad("Road", ObjectHeight.Floor);
        road.SetMaterial(GeneratorAssets.Get().RoadMaterial);
        road.GetComponent<Collider>().enabled = false;
        road.SetParent(go);

        return go;
    }
}