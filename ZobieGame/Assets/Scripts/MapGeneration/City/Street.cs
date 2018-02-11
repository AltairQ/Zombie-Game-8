using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Street : MapObject
{
    public Street(Rect rect) : base(rect)
    {

    }

    protected override void DoGenerate()
    {
    }

    protected override GameObject DoMake()
    {
        var street =  Rect.ToTerrainQuad("Street", ObjectHeight.Floor);
        street.SetMaterial(GeneratorAssets.Get().StreetMaterial);
        street.GetComponent<Collider>().enabled = false;

        return street;
    }
}