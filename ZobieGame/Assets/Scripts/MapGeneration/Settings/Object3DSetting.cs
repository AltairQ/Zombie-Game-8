using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Object3DSetting", menuName = "Generator/Object3DSetting")]
public class Object3DSetting : ScriptableObject
{
    [System.Serializable]
    public class LPattern
    {
        public string Axiom;
        public string FMove;
        public int Iterations;
    }

    public string Name;
    public int SegmentsNumber;
    public float MinHeight;
    public float MaxHeight;

    public Material Material;
    public AnimationCurve[] Shapes;
    public LPattern[] Patterns;

    public AnimationCurve RandomShape()
    {
        int idx = Random.Range(0, Shapes.Length);
        return Shapes[idx];
    }

    public List<Vector2> RandomPattern()
    {
        int idx = Random.Range(0, Patterns.Length);
        LPattern pattern = Patterns[idx];
        return PatternGenerator.GeneratePattern(pattern.Axiom, pattern.FMove, pattern.Iterations);
    }
}
