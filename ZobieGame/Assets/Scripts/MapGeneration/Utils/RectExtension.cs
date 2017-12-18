using UnityEngine;
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
}

