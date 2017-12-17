using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    private MapGenerator _mapGen;
    private void OnEnable()
    {
        _mapGen = target as MapGenerator;
    }

    private int _width = 10;
    private float _height = 2;
    private int _depth = 10;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        _width = EditorGUILayout.IntSlider("Width", _width, 5, 50);
        _height = EditorGUILayout.Slider("Height", _height, 1, 4);
        _depth = EditorGUILayout.IntSlider("Depth", _depth, 5, 50);

        if (GUILayout.Button("Click"))
        {
            _mapGen.GenerateHouse(_width, _height, _depth);
        }
    }

}


