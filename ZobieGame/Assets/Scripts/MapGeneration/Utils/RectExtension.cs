﻿using UnityEngine;
public static class RectExtension
{ 
    public static Vector3 Center(this Rect rect, float y)
    {
        return new Vector3(rect.center.x, y, rect.center.y);
    }

    public static Vector3 Scale(this Rect rect, float height)
    {
        return new Vector3(rect.width, height, rect.height);
    }

    public static Vector2 LeftTop(this Rect rect)
    {
        return new Vector2(rect.xMin, rect.yMin);
    }
    public static Vector2 LeftBottom(this Rect rect)
    {
        return new Vector2(rect.xMin, rect.yMax);
    }
    public static Vector2 RightTop(this Rect rect)
    {
        return new Vector2(rect.xMax, rect.yMin);
    }
    public static Vector2 RightBottom(this Rect rect)
    {
        return new Vector2(rect.xMax, rect.yMax);
    }

    public static Vector2[] AllPoints(this Rect rect)
    {
        return new Vector2[]{ rect.LeftTop(), rect.RightTop(), rect.RightBottom(), rect.LeftBottom(), rect.LeftTop()};
    }

    public static Rect[] Split(this Rect rect, Vector2 p1, Vector2 p2)
    {
        Rect[] ans = new Rect[2];
        if (p1.x == p2.x)
        {
            float height = rect.height;
            ans[0] = new Rect(rect.LeftTop(), new Vector2(p1.x - rect.xMin, height));
            ans[1] = new Rect(rect.LeftTop() + new Vector2(p1.x - rect.xMin, 0), new Vector2(rect.xMax - p1.x, height));
        }
        else if(p1.y == p2.y)
        {
            float width = rect.width;
            ans[0] = new Rect(rect.LeftTop(), new Vector2(width, p1.y - rect.yMin));
            ans[1] = new Rect(rect.LeftTop() + new Vector2(0, p1.y - rect.yMin), new Vector2(width, rect.yMax - p1.y));
        }

        return ans;
    }

    public static float Area(this Rect rect)
    {
        return rect.width * rect.height;
    }

    /*true if p is inside or on the edge*/
    public static bool ContainsE(this Rect rect, Vector2 p)
    {
        if(rect.Contains(p))
        {
            return true;
        }

        return rect.ContainsOnEdge(p);
    }

    public static GameObject ToTerrainQuad(this Rect rect, string name, float height)
    {
        GameObject go = Utils.TerrainObject(PrimitiveType.Quad, name);
        
        go.transform.position = rect.Center(height);
        go.transform.localScale = new Vector3(rect.width, rect.height, 1);
        go.transform.Rotate(new Vector3(90, 0, 0));

        return go;
    }

    public static bool ContainsOnEdge(this Rect rect, Vector2 p)
    {
        var points = rect.AllPoints();
        for (int i = 0; i < 4; i++)
        {
            if (Utils.Contains(points[i], points[i + 1], p))
            {
                return true;
            }
        }
        return false;
    }
}

