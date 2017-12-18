using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour {
    private Rect _rect;
    private float _height;
    private List<Wall> _walls = new List<Wall>();

    public House(Rect rect, float height)
    {
        _rect = rect;
        _height = height;

        AddWall(rect.LeftTop(), rect.LeftBottom());
        AddWall(rect.LeftTop(), rect.RightTop());
        AddWall(rect.RightBottom(), rect.RightTop());
        AddWall(rect.RightBottom(), rect.LeftBottom());

        float doorSize = 1;
        Vector2 delta = new Vector2(0, (rect.height - doorSize)/2);
        _walls[0].AddPart(rect.LeftTop() + delta, rect.LeftBottom() - delta, Wall.PartType.Door);
    }

    public void AddWall(Vector2 p1, Vector2 p2)
    {
        _walls.Add(new Wall(p1, p2, _height));
    }

    public GameObject Generate()
    {
        GameObject go = new GameObject();
        go.name = "House";

        var floor = GenerateFloor();
        floor.transform.parent = go.transform;

        foreach (var wall in _walls)
        {
            var wallGO = wall.Generate();
            wallGO.transform.parent = go.transform;
        }

        return go;
    }

    private GameObject GenerateFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Quad);
        floor.name = "Floor";

        floor.transform.position = _rect.Center(0);
        floor.transform.localScale = new Vector3(_rect.width, _rect.height, 1);
        floor.transform.Rotate(new Vector3(90, 0, 0));

        return floor;
    }
}
