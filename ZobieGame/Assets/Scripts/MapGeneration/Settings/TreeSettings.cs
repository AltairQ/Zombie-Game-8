using UnityEngine;

[CreateAssetMenu(fileName = "TreeSettings", menuName = "Generator/TreeSettings")]
public class TreeSettings : ScriptableObject
{
    public float MinLowerHeight;
    public float MaxLowerHeight;

    public float MinUpperHeight;
    public float MaxUpperHeight;
}
