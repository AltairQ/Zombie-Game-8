using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MonoBehaviour {

    private Rect _rect;
    private List<Tree> _trees;
    public void Generate(Rect rect)
    {
        _rect = rect;
        _trees = new List<Tree>();

        float treeRectSize = 4;
        float perlinOff = Random.Range(0f, 666f);
        for(float y = _rect.yMin; y + treeRectSize < _rect.yMax; y += treeRectSize)
        {
            for (float x = _rect.xMin; x + treeRectSize < _rect.xMax; x += treeRectSize)
            {
                float val = Mathf.PerlinNoise(x+perlinOff, y+perlinOff);
                float randVal = Random.Range(0f, 1f);
                if(randVal > val)
                {
                    AddTree(x, y, treeRectSize);
                }
            }
        }
    }

    private void AddTree(float x, float y, float treeRectSize)
    {
        Rect treeRect = new Rect(x, y, treeRectSize, treeRectSize);
        Tree tree = new Tree(treeRect);
        tree.Generate();

        _trees.Add(tree);
    }

    public GameObject Make(bool selfParent = false)
    {
        GameObject go = Utils.TerrainObject("Forest");
        if (selfParent)
        {
            gameObject.Clear(DestroyImmediate);
            go.SetParent(gameObject);
        }     

        MakeTerrain(go);

        foreach (var tree in _trees)
        {
            var treeGO = tree.Make();
            treeGO.SetParent(go);
        }

        return go;
    }

    private GameObject MakeTerrain(GameObject parent)
    {
        var terrain = _rect.ToTerrainQuad("Terrain", ObjectHeight.Ground);
        terrain.SetParent(parent);
        terrain.SetMaterial(GeneratorAssets.Get().TerrainMaterial);
        
        return terrain;
    }
}
