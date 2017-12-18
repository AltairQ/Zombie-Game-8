using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {
    public const float WallDepth = 0.2f;

    private List<WallPart> _parts = new List<WallPart>();
    private float _height;
    public Wall(Vector2 p1, Vector2 p2, float height = 2)
    {
        _height = height;
        _parts.Add(new WallPart(p1, p2));
    }

    public GameObject Generate()
    {
        GameObject go = new GameObject();
        go.name = "Wall";
        foreach(var part in _parts)
        {
            var partGO = part.Generate(_height);
            partGO.transform.parent = go.transform;
        }

        return go;
    }

    public void AddPart(Vector2 p1, Vector2 p2, PartType type)
    {
        WallPart newPart = new WallPart(p1, p2, type);
        var partToSplit = _parts.Find(part => part.rect.Contains(newPart.rect.center));

        var newAfterSplit = partToSplit.Split(newPart);
        _parts.Add(newPart);
        _parts.Add(newAfterSplit);
    }

    public enum PartType
    {
        Wall,
        Door,
        Window
    }

    private class WallPart
    {
        public PartType type;
        public Rect rect;

        public WallPart(Vector2 p1, Vector2 p2, PartType partType = PartType.Wall)
        {
            type = partType;

            float left = Mathf.Min(p1.x, p2.x);
            float top = Mathf.Min(p1.y, p2.y);
            float width = Mathf.Abs(p1.x - p2.x);
            float height = Mathf.Abs(p1.y - p2.y);
            if (width==0)
            {
                width = WallDepth;
                left -= WallDepth / 2;
            }
            if (height == 0)
            {
                height = WallDepth;
                top -= WallDepth / 2;
            }

            rect = new Rect(left, top, width, height);
        }

        public GameObject Generate(float height)
        {
            float centerY = height / 2;
            float scaleH = height;
            if(type != PartType.Wall)
            {
                centerY = height * 0.95f;
                scaleH = height * 0.1f;
            }

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "WallPart: " + type;
            go.transform.position = rect.Center(centerY);
            go.transform.localScale = rect.Scale(scaleH);

            return go;
        }

        // changes size of current object and returns other half
        public WallPart Split(WallPart part)
        {
            if(rect.xMin == part.rect.xMin)
            {
                float firstEnd = part.rect.yMin;
                float secondBeg = part.rect.yMax;
                float secondEnd = rect.yMax;
                rect.height = firstEnd - rect.yMin;

                
                Vector2 p1 = new Vector2(rect.xMin + WallDepth / 2, secondBeg);
                Vector2 p2 = new Vector2(rect.xMin + WallDepth / 2, secondEnd);
                return new WallPart(p1, p2, type);
            }
            if (rect.yMin == part.rect.yMin)
            {
                float firstEnd = part.rect.xMin;
                float secondBeg = part.rect.xMax;
                float secondEnd = rect.xMax;
                rect.width = firstEnd - rect.xMin;


                Vector2 p1 = new Vector2(secondBeg, rect.yMin + WallDepth / 2);
                Vector2 p2 = new Vector2(secondEnd, rect.yMin + WallDepth / 2);
                return new WallPart(p1, p2, type);
            }

            return null;
        }
    }
}
