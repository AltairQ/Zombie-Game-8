using UnityEngine;

public class Tree
{
    private Rect _rect;
    private TreeSettings _settings;
    public Tree(Rect rect)
    {
        _rect = rect;
        _settings = GeneratorAssets.Get().TreeSettings;
    }

    private float _lowerHeight;
    private float _upperHeight;
    public void Generate()
    {
        _lowerHeight = Random.Range(_settings.MinLowerHeight, _settings.MaxLowerHeight);
        _upperHeight = Random.Range(_settings.MinUpperHeight, _settings.MaxUpperHeight);
    }

    public GameObject Make()
    {
        GameObject go = Utils.TerrainObject("Tree");

        CreatePart(go, "LowerTreePart", GeneratorAssets.Get().LowerTreeMaterial, _lowerHeight, 0, 0.25f);
        CreatePart(go, "UpperTreePart", GeneratorAssets.Get().UpperTreeMaterial, _upperHeight, _lowerHeight, 0.5f);

        return go;
    }

    private GameObject CreatePart(GameObject parent, string name, Material material, float height, float heightOffset, float rectPercentage)
    {
        GameObject go = Utils.TerrainObject(PrimitiveType.Cube, name);

        Vector2 size = _rect.size * rectPercentage;
        Vector2 leftTop = _rect.center - size / 2;
        Rect rect = new Rect(leftTop, size);

        go.transform.position = rect.Center(heightOffset + height / 2);
        go.transform.localScale = rect.Scale(height);
        go.SetMaterial(material);
        go.SetParent(parent);

        return go;
    }
}

