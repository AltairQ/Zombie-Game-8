using UnityEngine;
using UnityEditor;

public class Rock : MapObject
{
    private ForestSettings _settings;
    public Rock(Rect rect) : base(rect)
    {
        _settings = GeneratorAssets.Get().ForestSettings;
    }

    private float _bottomHeight;
    private float _topHeight;
    public override void Generate()
    {
        _bottomHeight = Random.Range(_settings.MinRockBottomHeight, _settings.MaxRockBottomHeight);
        _topHeight = Random.Range(_settings.MinRockTopHeight, _settings.MaxRockTopHeight);
    }

    public override GameObject Make()
    {
        GameObject go = Utils.TerrainObject("Rock");

        CreatePart(go, "BottomPart", _bottomHeight, 0, 0.4f);
        CreatePart(go, "TopPart",  _topHeight, _bottomHeight, 0.35f);

        return go;
    }

    private GameObject CreatePart(GameObject parent, string name, float height, float heightOffset, float rectPercentage)
    {
        GameObject go = Utils.TerrainObject(PrimitiveType.Cube, name);

        Vector2 size = Rect.size * rectPercentage;
        Vector2 leftTop = Rect.center - size / 2;
        Rect rect = new Rect(leftTop, size);

        go.transform.position = rect.Center(heightOffset + height / 2);
        go.transform.localScale = rect.Scale(height);
        go.SetMaterial(GeneratorAssets.Get().RockMaterial);
        go.SetParent(parent);

        return go;
    }
}