using UnityEngine;
using System.Collections;

public class MapComponent<T> : MonoBehaviour where T : MapObject
{
    public void Create(T mapObject)
    {
        mapObject.Generate();
        GameObject go = mapObject.Make();

        CreateTerrain(mapObject.Rect, go);

        gameObject.Clear(DestroyImmediate);
        go.SetParent(gameObject);
    }

    private void CreateTerrain(Rect rect, GameObject parent)
    {
        var terrain = rect.ToTerrainQuad("Terrain", ObjectHeight.Ground);
        terrain.SetParent(parent);
        terrain.SetMaterial(GeneratorAssets.Get().TerrainMaterial);
    }
}
