using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Object3DSetting", menuName = "Generator/Object3DSetting")]
public class Object3DSetting : ScriptableObject
{
    public string Name;
    public int SegmentsNumber;
    public float MinHeight;
    public float MaxHeight;
    public AnimationCurve Shape;
    public Material Material;
}
