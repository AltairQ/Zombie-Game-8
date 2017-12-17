using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
    private GameObject _lastHouse = null;
    private int num = 0;
    public void GenerateHouse(float width, float height, float depth)
    {
        DestroyImmediate(_lastHouse);

        _lastHouse = GameObject.CreatePrimitive(PrimitiveType.Quad);
        _lastHouse.name = "House" + num;
        num++;

        _lastHouse.transform.rotation = Quaternion.Euler(new Vector3(90, 0));
        _lastHouse.transform.localScale = new Vector3(width, depth, 1);


    }
}
