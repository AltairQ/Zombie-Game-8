using UnityEngine;

[CreateAssetMenu(fileName = "CitySettings", menuName = "Generator/CitySettings")]
public class CitySettings : ScriptableObject
{
    public float EstateStreetOffset;
    public float MinEstateEdge;
    public float StreetSize;
    public float SpaceBetweenHouses;
}