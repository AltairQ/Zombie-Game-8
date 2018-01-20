using UnityEngine;
using System;

public class Utils
{
    public static void Swap<T>(ref T e1, ref T e2)
    {
        T tmp = e1;
        e1 = e2;
        e2 = tmp;
    }

    public static void OrderSwap(ref Vector2 p1, ref Vector2 p2)
    {
        if (p2.IsSmaller(p1))
        {
            Swap(ref p1, ref p2);
        }
    }

    public static bool IsTrue<T>(T[] tab, Predicate<T> pred)
    {
        foreach(var e in tab)
        {
            if(!pred(e))
            {
                return false;
            }
        }
        return true;
    }

    public static Vector2[] Merge(Vector2 p1, Vector2 p2, Vector2 v1, Vector2 v2)
    {
        OrderSwap(ref p1, ref p2);
        OrderSwap(ref v1, ref v2);
        bool sameX = IsTrue(new Vector2[]{ p1,p2,v1,v2}, v => Utils.TheSame(v.x, p1.x));
        bool sameY = IsTrue(new Vector2[] { p1, p2, v1, v2 }, v => Utils.TheSame(v.y, p1.y));

        if(!sameX && !sameY)
        {
            return null;
        }

        Vector2[] ans = { new Vector2(Mathf.Max(p1.x, v1.x), Mathf.Max(p1.y, v1.y)),
                          new Vector2(Mathf.Min(p2.x, v2.x), Mathf.Min(p2.y, v2.y)) };

        if (!ans[0].IsSmaller(ans[1]))
        {
            return null;
        }

        return ans;
    }

    public static Rect SegmentToRect(Vector2 p1, Vector2 p2, float size)
    {
        float left = Mathf.Min(p1.x, p2.x);
        float top = Mathf.Min(p1.y, p2.y);
        float width = Mathf.Abs(p1.x - p2.x);
        float height = Mathf.Abs(p1.y - p2.y);

        if (TheSame(width, 0))
        {
            width = size;
            left -= size / 2;
        }
        if (TheSame(height, 0))
        {
            height = size;
            top -= size / 2;
        }

        return new Rect(left, top, width, height);
    }

    public static GameObject TerrainObject(string name)
    {
        GameObject go = new GameObject(name);
        go.layer = 8; // Terrain layer

        return go;
    }

    public static GameObject TerrainObject(PrimitiveType type, string name)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.layer = 8; // Terrain layer
        
        return go;
    }

    public static GameObject CreateGround(Rect rect, GameObject parent)
    {
        var terrain = rect.ToTerrainQuad("Ground", ObjectHeight.Ground);
        terrain.SetParent(parent);
        terrain.SetMaterial(GeneratorAssets.Get().GroundMaterial);

        return terrain;
    }

    public static bool TheSame(float a, float b)
    {
        return Mathf.Abs(a - b) < 1e-6;
    }

    public static bool Contains(Vector2 p1, Vector2 p2, Vector2 v)
    {
        OrderSwap(ref p1, ref p2);
        bool sameX = TheSame(p1.x, p2.x);
        bool sameY = TheSame(p1.y, p2.y);
        if(sameX)
        {
            return TheSame(p1.x, v.x) && p1.y <= v.y && v.y <= p2.y;
        }
        if(sameY)
        {
            return TheSame(p1.y, v.y) && p1.x <= v.x && v.x <= p2.x;
        }

        Debug.LogError("Utils.Contains() neither x and y are the same!");
        return false;
    }
}