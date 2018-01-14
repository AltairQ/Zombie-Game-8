using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall {
    public const float WallDepth = 0.4f;
    public bool ShadowsEnabled { get; set; }

    private List<WallPart> _parts = new List<WallPart>();
    private float _height;
    public Wall(Vector2 p1, Vector2 p2, float height = 2)
    {
        ShadowsEnabled = false;
        _height = height;
        _parts.Add(new WallPart(p1, p2));
    }

    public bool Overlaps(Rect rect)
    {
        foreach(var part in _parts)
        {
            if(rect.Overlaps(part.rect))
            {
                return true;
            }
        }
        return false;
    }

    public GameObject Make()
    {
        GameObject go = Utils.TerrainObject("Wall");
        foreach(var part in _parts)
        {
            var partGO = part.Make(_height, ShadowsEnabled);
            partGO.transform.parent = go.transform;
        }

        return go;
    }

    public bool AddPart(Vector2 p1, Vector2 p2, PartType type)
    {
        var partToSplit = FindInPart(p1, p2);
        if(partToSplit == null)
        {
            return false;
        }
        
        WallPart newPart = new WallPart(p1, p2, type);
        var newAfterSplit = partToSplit.Split(newPart);
        if(newAfterSplit == null)
        {
            return false;
        }

        _parts.Add(newPart);
        _parts.Add(newAfterSplit);
        return true;
    }

    private WallPart FindInPart(Vector2 p1, Vector2 p2)
    {
        return _parts.Find(part => part.rect.ContainsE(p1) && part.rect.ContainsE(p2));
    }

    public bool ContainsInPart(Vector2 p1, Vector2 p2)
    {
        return FindInPart(p1, p2) != null;
    }

    public void RandomWindowToDoor()
    {
        var windows = _parts.FindAll(w => w.type == PartType.Window);
        int idx = Random.Range(0, windows.Count);
        windows[idx].type = PartType.Door;
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
            rect = Utils.SegmentToRect(p1, p2, WallDepth);
        }

        public GameObject Make(float height, bool shadowsEnabled)
        {
            float centerY = height / 2;
            float scaleH = height;
            if(type != PartType.Wall)
            {
                centerY = height * 0.95f;
                scaleH = height * 0.1f;
            }

            GameObject go = Utils.TerrainObject(PrimitiveType.Cube, "WallPart: " + type);
            go.transform.position = rect.Center(centerY);
            go.transform.localScale = rect.Scale(scaleH);
            go.SetMaterial(GeneratorAssets.Get().WallMaterial);
            RemoveShadows(go, shadowsEnabled);

            if(type == PartType.Window)
            {
                GameObject go2 = Utils.TerrainObject(PrimitiveType.Cube, "WallPart2: " + type);
                go2.transform.position = rect.Center(height * 0.2f);
                go2.transform.localScale = rect.Scale(height * 0.4f);
                go2.SetMaterial(GeneratorAssets.Get().WallMaterial);
                RemoveShadows(go2, shadowsEnabled);

                GameObject window = Utils.TerrainObject("WindowPack");
                go.transform.parent = window.transform;
                go2.transform.parent = window.transform;

                return window;
            }

            return go;
        }

        private void RemoveShadows(GameObject go, bool shadowsEnabled)
        {
            if(shadowsEnabled)
            {
                return;
            }

            var mr = go.GetComponent<MeshRenderer>();
            if (mr == null)
            {
                Debug.Log("no mesh renderer!");
                return;
            }
            mr.receiveShadows = false;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        // changes size of current object and returns other half
        public WallPart Split(WallPart part)
        {
            if(Utils.TheSame(rect.xMin, part.rect.xMin))
            {
                float firstEnd = part.rect.yMin;
                float secondBeg = part.rect.yMax;
                float secondEnd = rect.yMax;
                rect.height = firstEnd - rect.yMin;
                
                Vector2 p1 = new Vector2(rect.xMin + WallDepth / 2, secondBeg);
                Vector2 p2 = new Vector2(rect.xMin + WallDepth / 2, secondEnd);
                return new WallPart(p1, p2, type);
            }
            if (Utils.TheSame(rect.yMin, part.rect.yMin))
            {
                float firstEnd = part.rect.xMin;
                float secondBeg = part.rect.xMax;
                float secondEnd = rect.xMax;
                rect.width = firstEnd - rect.xMin;


                Vector2 p1 = new Vector2(secondBeg, rect.yMin + WallDepth / 2);
                Vector2 p2 = new Vector2(secondEnd, rect.yMin + WallDepth / 2);
                return new WallPart(p1, p2, type);
            }

            Debug.LogWarning("WallPart.Split(): null returning!");
            return null;
        }
    }
}
