using UnityEngine;

[ExecuteInEditMode]
public class GeneratorAssets : MonoBehaviour
{
    public Material StreetMaterial { get; private set; }
    public Material FloorMaterial { get; private set; }
    public Material WallMaterial { get; private set; }
    public Material GroundMaterial { get; private set; }
    public Material TreeTopMaterial { get; private set; }
    public Material TreeBottomMaterial { get; private set; }
    public Material RockMaterial { get; private set; }

    public HouseSettings HouseSettings { get; private set; }
    public CitySettings CitySettings { get; private set; }
    public ForestSettings ForestSettings { get; private set; }
    public LootSettings LootSettings { get; private set; }
    public FurnitureSettings FurnitureSettings { get; private set; }
    public Object3DSetting BushSetting { get; private set; }
    public Object3DSetting RockSetting { get; private set; }
    public Object3DSetting TreeTopSetting { get; private set; }
    public Object3DSetting TreeBottomSetting { get; private set; }

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
        TreeTopMaterial = Resources.Load("Materials/TreeTopMaterial", typeof(Material)) as Material;
        TreeBottomMaterial = Resources.Load("Materials/TreeBottomMaterial", typeof(Material)) as Material;
        RockMaterial = Resources.Load("Materials/RockMaterial", typeof(Material)) as Material;

        HouseSettings = Resources.Load("ScriptableObjects/HouseSettings", typeof(HouseSettings)) as HouseSettings;
        CitySettings = Resources.Load("ScriptableObjects/CitySettings", typeof(CitySettings)) as CitySettings;
        ForestSettings = Resources.Load("ScriptableObjects/ForestSettings", typeof(ForestSettings)) as ForestSettings;
        LootSettings = Resources.Load("ScriptableObjects/LootSettings", typeof(LootSettings)) as LootSettings;
        FurnitureSettings = Resources.Load("ScriptableObjects/FurnitureSettings", typeof(FurnitureSettings)) as FurnitureSettings;
        BushSetting = Resources.Load("ScriptableObjects/Nature/BushSetting", typeof(Object3DSetting)) as Object3DSetting;
        RockSetting = Resources.Load("ScriptableObjects/Nature/RockSetting", typeof(Object3DSetting)) as Object3DSetting;
        TreeTopSetting = Resources.Load("ScriptableObjects/Nature/TreeTopSetting", typeof(Object3DSetting)) as Object3DSetting;
        TreeBottomSetting = Resources.Load("ScriptableObjects/Nature/TreeBottomSetting", typeof(Object3DSetting)) as Object3DSetting;

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
