using UnityEngine;

[CreateAssetMenu(fileName = "CitySettings", menuName = "Generator/CitySettings")]
public class CitySettings : ScriptableObject
{
    public float EstateStreetOffset;
    public float MinEstateEdge;
    public float StreetSize;
    public float RoadPercSize;
    public float SpaceBetweenHouses;
    public float SpaceBetweenLamps;
    public float MinSpaceBetweenCars;
    public float MaxSpaceBetweenCars;

    public GameObject LampPrefab;
    public GameObject CarPrefab;
    public float CarDWProportion; // depth (z) to width (x)
}