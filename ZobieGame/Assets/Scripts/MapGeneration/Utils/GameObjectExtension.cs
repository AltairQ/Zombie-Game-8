using UnityEngine;
public static class GameObjectExtension
{
    public static void SetParent(this GameObject go, GameObject parent)
    {
        go.transform.parent = parent.transform;
    }
}

