﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MapObject {
    private List<Tree> _trees;
    public Forest(Rect rect) : base(rect)
    {
    }

    public override void Generate()
    {
        _trees = new List<Tree>();

        float treeRectSize = 4;
        float perlinOff = Random.Range(0f, 666f);
        for(float y = Rect.yMin; y + treeRectSize < Rect.yMax; y += treeRectSize)
        {
            for (float x = Rect.xMin; x + treeRectSize < Rect.xMax; x += treeRectSize)
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

    public override GameObject Make()
    {
        GameObject go = Utils.TerrainObject("Forest");
        foreach (var tree in _trees)
        {
            var treeGO = tree.Make();
            treeGO.SetParent(go);
        }

        return go;
    }
}
