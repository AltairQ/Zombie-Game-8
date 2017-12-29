using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
    public void GenerateHouse(float width, float height, float depth)
    {
        Clear();
    }

    private void Clear()
    {
        for(int i = transform.childCount-1; i>=0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
