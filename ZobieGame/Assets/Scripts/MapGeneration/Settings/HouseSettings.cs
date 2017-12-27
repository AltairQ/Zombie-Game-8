using UnityEngine;

[CreateAssetMenu(fileName = "HouseSettings", menuName = "Generator/HouseSettings")]
public class HouseSettings : ScriptableObject
{
    public float MinHouseEdge { get; set; }
    public float MaxHouseEdge { get; set; }
    public float Height { get; set; }

    public float MinRoomEdge { get; set; }
    public float MinRoomArea { get; set; }
    public float MaxRoomArea { get; set; }

    public float DoorSize { get; set; }
    public float WindowSize { get; set; }
    public float SpaceBetweenWindows { get; set; }
}