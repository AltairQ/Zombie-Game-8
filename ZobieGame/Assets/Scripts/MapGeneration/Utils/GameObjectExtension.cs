using UnityEngine;
public static class GameObjectExtension
{
    public static void SetParent(this GameObject go, GameObject parent)
    {
        go.transform.parent = parent.transform;
    }

    public static void SetMaterial(this GameObject go, Material material)
    {
        go.GetComponent<MeshRenderer>().material = material;
    }
}

