using UnityEngine;

[CreateAssetMenu(fileName = "HouseSettings", menuName = "Generator/HouseSettings")]
public class HouseSettings : ScriptableObject
{
    public float MinHouseEdge;
    public float MaxHouseEdge;
    public float Height;

    public float MinRoomEdge;
    public float MinRoomArea;
    public float MaxRoomArea;

    public float DoorSize;
    public float WindowSize;
    public float SpaceBetweenWindows;
}