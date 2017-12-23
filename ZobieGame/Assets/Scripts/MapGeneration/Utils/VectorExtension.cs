using UnityEngine;

public static class VectorExtension
{
    public static bool IsSmaller(this Vector2 v, Vector2 v2)
    {
        if(v.x < v2.x)
        {
            return true;
        }

        if(v.x > v2.x)
        {
            return false;
        }

        return v.y <= v2.y;
    }
}

