using System.ComponentModel;
using UnityEngine;

[ExecuteInEditMode]
public class GeneratorAssets : MonoBehaviour
{
    public Material StreetMaterial { get; private set; }
    public Material FloorMaterial { get; private set; }
    public Material WallMaterial { get; private set; }
    public Material TerrainMaterial { get; private set; }
    public HouseSettings HouseSettings { get; private set; }
    public CitySettings CitySettings { get; private set; }

    private static GeneratorAssets _instance = null;
    private void Start()
    {
        _instance = this;
        StreetMaterial = Resources.Load("Materials/StreetMaterial", typeof(Material)) as Material;
        FloorMaterial = Resources.Load("Materials/FloorMaterial", typeof(Material)) as Material;
        WallMaterial = Resources.Load("Materials/WallMaterial", typeof(Material)) as Material;
        TerrainMaterial = Resources.Load("Materials/TerrainMaterial", typeof(Material)) as Material;
        HouseSettings = Resources.Load("ScriptableObjects/HouseSettings", typeof(HouseSettings)) as HouseSettings;
        CitySettings = Resources.Load("ScriptableObjects/CitySettings", typeof(CitySettings)) as CitySettings;

        Debug.Log("GeneratorAssets instance created");
    }

    public static GeneratorAssets Get()
    {
        if(_instance == null)
        {
            _instance = GameObject.FindObjectOfType<GeneratorAssets>();
        }
        
        return _instance;
    }
}
