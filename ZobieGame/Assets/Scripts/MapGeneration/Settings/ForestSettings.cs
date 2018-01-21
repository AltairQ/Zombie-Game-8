using UnityEngine;

[CreateAssetMenu(fileName = "ForestSettings", menuName = "Generator/ForestSettings")]
public class ForestSettings : ScriptableObject
{
    public float MinTreeBottomHeight;
    public float MaxTreeBottomHeight;

    public float MinTreeTopHeight;
    public float MaxTreeTopHeight;

    public float MinRockBottomHeight;
    public float MaxRockBottomHeight;

    public float MinRockTopHeight;
    public float MaxRockTopHeight;

    public float MinBushBottomHeight;
    public float MaxBushBottomHeight;

    public float MinBushTopHeight;
    public float MaxBushTopHeight;
}
