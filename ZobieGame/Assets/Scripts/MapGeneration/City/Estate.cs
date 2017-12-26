using UnityEngine;
using UnityEditor;

public class Estate
{
    private Rect _rect;
    public Rect Rect {
        get { return _rect; }
    }

    public Estate(Rect rect)
    {
        _rect = rect;
    }

    public GameObject Make(House h)
    {
        h.Generate(_rect.center);
        return h.Make();
    }
}