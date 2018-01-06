﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Street
{
    private Rect _rect;
    public Street(Vector2 p1, Vector2 p2, float streetSize)
    {
        _rect = Utils.SegmentToRect(p1, p2, streetSize);
    }

    public GameObject Make()
    {
        var street =  _rect.ToTerrainQuad("Street", ObjectHeight.Floor);
        street.SetMaterial(GeneratorAssets.Get().StreetMaterial);
        street.GetComponent<Collider>().enabled = false;

        return street;
    }

}