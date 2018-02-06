using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Object3DSetting", menuName = "Generator/Object3DSetting")]
public class Object3DSetting : ScriptableObject
{
    public string Name;
    public int SegmentsNumber;
    public float MinHeight;
    public float MaxHeight;

    public AnimationCurve Shape;
    public Material Material;

    public string Axiom;
    public string FMove;
    public int Iterations;

    private List<Vector2> _pattern = new List<Vector2>();
    public List<Vector2> GetPattern()
    {
        return PatternGenerator.GeneratePattern(Axiom, FMove, Iterations);
    }
}
