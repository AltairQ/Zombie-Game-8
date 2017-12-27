using UnityEngine;
using UnityEditor;

public class Estate
{
    private Rect _rect;
    public Rect Rect 
    {
        get { return _rect; }
    }

    public Estate(Rect rect)
    {
        _rect = rect;
    }

    public GameObject Make(House h)
    {
        Rect rect = new Rect(_rect.xMin + 2, _rect.yMin + 2, _rect.width - 4, _rect.height - 4);
        h.Generate(rect);
        return h.Make();
    }
}