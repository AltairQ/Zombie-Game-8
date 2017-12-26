using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(City))]
public class CityEditor : Editor
{
    private City _city;
    private void OnEnable()
    {
        _city = target as City;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        _city.Width = EditorGUILayout.IntSlider("Width", (int)_city.Width, 100, 200);
        _city.Depth = EditorGUILayout.IntSlider("Depth", (int)_city.Depth, 100, 200);

        _city.MinEstateEdge = EditorGUILayout.Slider("MinEstateEdge", _city.MinEstateEdge, 10, 20);
        _city.StreetSize = EditorGUILayout.Slider("StreetSize", _city.StreetSize, 2, 4);

        if (GUILayout.Button("Generate"))
        {
            _city.Generate();
            _city.Make();
        }
    }

}


