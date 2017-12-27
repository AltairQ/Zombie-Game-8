﻿using System.ComponentModel;
using UnityEngine;

[ExecuteInEditMode]
public class GeneratorAssets : MonoBehaviour
{
    public Material StreetMaterial { get; private set; }
    public HouseSettings HouseSettings { get; private set; }

    private static GeneratorAssets _instance = null;
    private void Start()
    {
        _instance = this;
        StreetMaterial = Resources.Load("Materials/StreetMaterial.mat", typeof(Material)) as Material;
        HouseSettings = Resources.Load("ScriptableObjects/HouseSettings", typeof(HouseSettings)) as HouseSettings;
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
