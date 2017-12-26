using UnityEngine;

[ExecuteInEditMode]
public class GeneratorAssets : MonoBehaviour
{
    public Material streetMaterial;
    private static GeneratorAssets _instance = null;
    private void Start()
    {
        _instance = this;
        Debug.Log("GenAss instance created");
    }

    public static GeneratorAssets Get()
    {
        return _instance;
    }
}
