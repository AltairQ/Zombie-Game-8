using UnityEngine;

[ExecuteInEditMode]
public class GeneratorAssets : MonoBehaviour
{
    public Material StreetMaterial { get; private set; }
    public Material FloorMaterial { get; private set; }
    public Material WallMaterial { get; private set; }
    public Material GroundMaterial { get; private set; }
    public Material UpperTreeMaterial { get; private set; }
    public Material LowerTreeMaterial { get; private set; }
    public HouseSettings HouseSettings { get; private set; }
    public CitySettings CitySettings { get; private set; }
    public TreeSettings TreeSettings { get; private set; }

    private static GeneratorAssets _instance = null;
    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (_instance == null)
        {
            _instance = this;
            Load();
        }
    }

    public void Load()
    {
        StreetMaterial = Resources.Load("Materials/StreetMaterial", typeof(Material)) as Material;
        FloorMaterial = Resources.Load("Materials/FloorMaterial", typeof(Material)) as Material;
        WallMaterial = Resources.Load("Materials/WallMaterial", typeof(Material)) as Material;
        GroundMaterial = Resources.Load("Materials/GroundMaterial", typeof(Material)) as Material;
        UpperTreeMaterial = Resources.Load("Materials/UpperTreeMaterial", typeof(Material)) as Material;
        LowerTreeMaterial = Resources.Load("Materials/LowerTreeMaterial", typeof(Material)) as Material;

        HouseSettings = Resources.Load("ScriptableObjects/HouseSettings", typeof(HouseSettings)) as HouseSettings;
        CitySettings = Resources.Load("ScriptableObjects/CitySettings", typeof(CitySettings)) as CitySettings;
        TreeSettings = Resources.Load("ScriptableObjects/TreeSettings", typeof(TreeSettings)) as TreeSettings;

        Debug.Log("GeneratorAssets loaded");
    }

    public static GeneratorAssets Get()
    {
        if(_instance == null)
        {
            _instance = FindObjectOfType<GeneratorAssets>();
        }
        
        return _instance;
    }
}
