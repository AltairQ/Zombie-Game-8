﻿using UnityEngine;

public class Tree : MapObject
{
    private ForestSettings _settings;
    public Tree(Rect rect) : base(rect)
    {
        _settings = GeneratorAssets.Get().ForestSettings;
    }

    private float _bottomHeight;
    private float _topHeight;
    public override void Generate()
    {
        _bottomHeight = Random.Range(_settings.MinTreeBottomHeight, _settings.MaxTreeBottomHeight);
        _topHeight = Random.Range(_settings.MinTreeTopHeight, _settings.MaxTreeTopHeight);
    }

    public override GameObject Make()
    {
        GameObject go = Utils.TerrainObject("Tree");

        CreatePart(go, "BottomPart", GeneratorAssets.Get().TreeBottomMaterial, _bottomHeight, 0, 0.25f);
        CreatePart(go, "TopPart", GeneratorAssets.Get().TreeTopMaterial, _topHeight, _bottomHeight, 0.5f);

        return go;
    }

    private GameObject CreatePart(GameObject parent, string name, Material material, float height, float heightOffset, float rectPercentage)
    {
        GameObject go = Utils.TerrainObject(PrimitiveType.Cube, name);

        Vector2 size = Rect.size * rectPercentage;
        Vector2 leftTop = Rect.center - size / 2;
        Rect rect = new Rect(leftTop, size);

        go.transform.position = rect.Center(heightOffset + height / 2);
        go.transform.localScale = rect.Scale(height);
        go.SetMaterial(material);
        go.SetParent(parent);

        return go;
    }
}

