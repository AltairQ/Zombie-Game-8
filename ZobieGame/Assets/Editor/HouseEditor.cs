using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(House))]
public class HouseEditor : Editor
{
    private House _house;
    private void OnEnable()
    {
        _house = target as House;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        _house.Width = EditorGUILayout.IntSlider("Width", (int)_house.Width, 5, 50);
        _house.Height = EditorGUILayout.Slider("Height", _house.Height, 1, 4);
        _house.Depth = EditorGUILayout.IntSlider("Depth", (int)_house.Depth, 5, 50);

        _house.MinRoomEdge = EditorGUILayout.Slider("MinRoomEdge", _house.MinRoomEdge, 2, 10);
        _house.MinRoomArea = EditorGUILayout.Slider("MinRoomArea", _house.MinRoomArea, 4, 10);
        _house.MaxRoomArea = EditorGUILayout.Slider("MaxRoomArea", _house.MaxRoomArea, 20, 100);

        _house.DoorSize = EditorGUILayout.Slider("DoorSize", _house.DoorSize, 1f, 4f);
        _house.WindowSize = EditorGUILayout.Slider("WindowSize", _house.WindowSize, 0.5f, 2f);
        _house.SpaceBetweenWindows = EditorGUILayout.Slider("SpaceBetweenWindows", _house.SpaceBetweenWindows, 0.5f, 2f);

        if (GUILayout.Button("Generate"))
        {
            _house.Generate();
            _house.Make();
        }
    }

}


