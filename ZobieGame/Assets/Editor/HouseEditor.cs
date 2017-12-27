﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(House))]
public class HouseEditor : Editor
{
    private House _house;
    private HouseSettings _settings;

    private void OnEnable()
    {
        _house = target as House;
        _settings = GeneratorAssets.Get().HouseSettings;
    }

    private float _width = 10;
    private float _depth = 10;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Instance settings");

        _width = EditorGUILayout.IntSlider("Width", (int)_width, 5, 50);
        _depth = EditorGUILayout.IntSlider("Depth", (int)_depth, 5, 50);

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Global settings");

        _settings.Height = EditorGUILayout.Slider("Height", _settings.Height, 1, 4);

        _settings.MinRoomEdge = EditorGUILayout.Slider("MinRoomEdge", _settings.MinRoomEdge, 2, 10);
        _settings.MinRoomArea = EditorGUILayout.Slider("MinRoomArea", _settings.MinRoomArea, 4, 100);
        _settings.MaxRoomArea = EditorGUILayout.Slider("MaxRoomArea", _settings.MaxRoomArea, 20, 300);

        _settings.DoorSize = EditorGUILayout.Slider("DoorSize", _settings.DoorSize, 1f, 4f);
        _settings.WindowSize = EditorGUILayout.Slider("WindowSize", _settings.WindowSize, 0.5f, 2f);
        _settings.SpaceBetweenWindows = EditorGUILayout.Slider("SpaceBetweenWindows", _settings.SpaceBetweenWindows, 0.5f, 2f);

        if (GUILayout.Button("Generate"))
        {
            Rect rect = new Rect(-_width / 2, -_depth / 2, _width, _depth);
            _house.Generate(rect);
            _house.Make();
        }
    }

}


