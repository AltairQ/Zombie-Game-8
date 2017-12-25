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
        bool sameX = IsTrue(new Vector2[]{ p1,p2,v1,v2}, v => Mathf.Approximately(v.x, p1.x));
        bool sameY = IsTrue(new Vector2[] { p1, p2, v1, v2 }, v => Mathf.Approximately(v.y, p1.y));

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
}