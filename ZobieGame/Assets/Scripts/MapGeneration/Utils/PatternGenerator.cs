using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternGenerator
{
    private static Vector2[] _dir = new[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
    public static List<Vector2> GeneratePattern(string axiom, string fMove, int iterations)
    {
        string word = GenerateWord(axiom, fMove, iterations);

        int dirIdx = 0;
        List<Vector2> points = new List<Vector2>();
        Vector2 pos = Vector2.zero;

        System.Action<int> changeDir = shift => { dirIdx = ((dirIdx + shift) % 4 + 4) % 4; };
        foreach(char c in word)
        {
            if(c == 'F')
            {
                pos += _dir[dirIdx];
                points.Add(pos);
            }
            if(c == '+')
            {
                changeDir(1);
            }
            if(c == '-')
            {
                changeDir(-1);
            }
        }

        var last = points[points.Count - 1];
        if(!Utils.TheSame(last.x, 0f) || !Utils.TheSame(last.y, 0f))
        {
            Debug.Log("Pattern is not closed! Adding Vector2.zero");
            points.Add(Vector2.zero);
        }

        return Normalize(points);
    }

    private static List<Vector2> Normalize(List<Vector2> points)
    {
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;
        foreach(Vector2 v in points)
        {
            minX = Mathf.Min(minX, v.x);
            minY = Mathf.Min(minY, v.y);
            maxX = Mathf.Max(maxX, v.x);
            maxY = Mathf.Max(maxY, v.y);
        }

        float sizeX = maxX - minX;
        float sizeY = maxY - minY;
        Vector2 center = new Vector2((maxX + minX) / 2, (maxY + minY) / 2);
        for(int i = 0; i < points.Count; i++)
        {
            points[i] -= center;
            points[i] = new Vector2(points[i].x / sizeX, points[i].y / sizeY);
        }

        return points;
    }

    private static string GenerateWord(string axiom, string fMove, int iterations)
    {
        if(iterations == 0)
        {
            return axiom;
        }

        string str = "";
        foreach(char c in axiom)
        {
            if(c == 'F')
            {
                str += fMove;
            }
            else
            {
                str += c;
            }
        }

        return GenerateWord(str, fMove, iterations - 1);
    }
}
