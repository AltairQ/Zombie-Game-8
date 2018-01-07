using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CityComponent))]
public class CityEditor : Editor
{
    private CitySettings _settings;

    private void OnEnable()
    {
        _settings = GeneratorAssets.Get().CitySettings;
    }

    private float _width = 100;
    private float _depth = 100;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Instance settings");

        _width = EditorGUILayout.IntSlider("Width", (int)_width, 100, 200);
        _depth = EditorGUILayout.IntSlider("Depth", (int)_depth, 100, 200);

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Global settings");

        _settings.EstateStreetOffset = EditorGUILayout.Slider("EstateStreetOffset", _settings.EstateStreetOffset, 1, 4);
        _settings.MinEstateEdge = EditorGUILayout.Slider("MinEstateEdge", _settings.MinEstateEdge, 10, 20);
        _settings.StreetSize = EditorGUILayout.Slider("StreetSize", _settings.StreetSize, 2, 6);
        _settings.SpaceBetweenHouses = EditorGUILayout.Slider("SpaceBetweenHouses", _settings.SpaceBetweenHouses, 2, 5);

        EditorUtility.SetDirty(_settings);

        if (GUILayout.Button("Generate"))
        {
            Rect rect = new Rect(-_width / 2, -_depth / 2, _width, _depth);
            (target as CityComponent).Create(new City(rect));
        }
    }

}


