using UnityEngine;

[CreateAssetMenu(fileName = "CitySettings", menuName = "Generator/CitySettings")]
public class CitySettings : ScriptableObject
{
    public float MinEstateEdge { get; set; }
    public float StreetSize { get; set; }
}