using UnityEngine;

public abstract class MapObject 
{ 
    public Rect Rect { get; private set; }
    public MapObject(Rect rect)
    {
        Rect = rect;
    }

    public abstract void Generate();
    public abstract GameObject Make();
}
