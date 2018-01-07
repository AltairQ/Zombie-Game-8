using UnityEngine;
using System;

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

    public static void Clear(this GameObject go, Action<GameObject> destroy)
    {
        for (int i = go.transform.childCount - 1; i >= 0; i--)
        {
            destroy(go.transform.GetChild(i).gameObject);
        }
    }

    public static Vector2 Get2dPos(this GameObject go)
    {
        Vector3 pos = go.transform.position;
        return new Vector2(pos.x, pos.z);
    }

    public static T GetOrAdd<T>(this GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if(comp == null)
        {
            return go.AddComponent<T>();
        }
        return comp;
    }

    public static void Combine(this GameObject go, GameObject toCombine)
    {
        var renderer = go.GetOrAdd<MeshRenderer>();
        var combineRenderer = toCombine.GetComponentInChildren<MeshRenderer>();
        if(combineRenderer == null)
        {
            Debug.LogError("Renderer not found for toCombine game object!");
            return;
        }

        var newMaterial = combineRenderer.sharedMaterial;
        var combines = GetCombines(toCombine);

        int newSize = renderer.sharedMaterials.Length + 1;
        if(newSize == 2 && renderer.sharedMaterial == null)
        {
            newSize = 1;
        }

        Material[] newMaterials = new Material[newSize];
        Material[] oldMaterials = renderer.sharedMaterials;

        for(int i=0; i < newSize-1; i++)
        {
            newMaterials[i] = oldMaterials[i];
        }
        newMaterials[newSize - 1] = newMaterial;
        renderer.sharedMaterials = newMaterials;

        var meshFilter = go.GetOrAdd<MeshFilter>();
        if(meshFilter.sharedMesh == null)
        {
            meshFilter.sharedMesh = new Mesh();
            meshFilter.sharedMesh.CombineMeshes(combines);
        }
        else
        {
            Mesh tmpMesh = new Mesh();
            tmpMesh.CombineMeshes(combines);
            CombineInstance combine = new CombineInstance();
            combine.mesh = tmpMesh;
            combine.transform = go.transform.localToWorldMatrix;

            var myCombine = go.GetCombine();
            meshFilter.sharedMesh = new Mesh();
            meshFilter.sharedMesh.CombineMeshes(new CombineInstance[]{ myCombine, combine}, false);
        }
        
    }

    private static CombineInstance[] GetCombines(GameObject go)
    {
        MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
            i++;
        }

        return combine;
    }

    public static CombineInstance GetCombine(this GameObject go)
    {
        return new CombineInstance() {
            mesh = go.GetComponent<MeshFilter>().sharedMesh,
            transform = go.transform.localToWorldMatrix
        };
    }
}

