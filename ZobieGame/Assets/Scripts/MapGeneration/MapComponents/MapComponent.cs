using UnityEngine;
using System.Collections;

public class MapComponent<T> : MonoBehaviour where T : MapObject
{
    public void Create(T mapObject)
    {
        mapObject.Generate();
        GameObject go = mapObject.Make();

        gameObject.Clear(DestroyImmediate);
        go.SetParent(gameObject);

        Utils.CreateGround(mapObject.Rect, gameObject);
    }


}
