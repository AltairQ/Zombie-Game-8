using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {

    private Rect _rect;
    public Rect Rect 
    {
        get { return _rect; }
    }

    public Room(Rect rect)
    {
        _rect = rect;
    }
}
