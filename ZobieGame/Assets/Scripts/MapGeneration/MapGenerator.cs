using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
    private int num = 0;
    public void GenerateHouse(float width, float height, float depth)
    {
        Clear();

        GameObject house = new GameObject();
        house.transform.parent = transform;
        house.name = "House";

        GameObject floor = Create("Floor", house.transform, Vector3.zero, Vector3.up,  new Vector3(width, depth));

        Vector2[] points = { new Vector2(-width / 2, 0), new Vector2(width / 2, 0),
                             new Vector2(0, -depth / 2),  new Vector2(0, depth / 2)};

        for(int i=0;i<4;i++)
        {
            Vector3 position = new Vector3(points[i].x, height / 2, points[i].y);
            Vector3 normal = Vector3.forward;
            Vector2 size = new Vector2(width, height);
            if (position.x != 0)
            {
                size.x = depth;
                normal = Vector3.right;
            }

            Create("Wall" + i, house.transform, position, normal, size);
        }
    }

    private GameObject Create(string name, Transform parent, Vector3 position, Vector3 normal, Vector2 size)
    {
        GameObject obj = new GameObject();
        
        obj.name = name;

        AddQuad(obj.transform, false);
        AddQuad(obj.transform, true);
        
        obj.transform.LookAt(normal);
        obj.transform.position = position;
        obj.transform.localScale = new Vector3(size.x, size.y, 1);

        obj.transform.parent = parent;
        return obj;
    }

    private void AddQuad(Transform parent, bool reversed)
    {
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.parent = parent;     

        if (reversed)
        {
            quad.transform.Rotate(new Vector3(0, 180, 0));
        }
    }

    private void Clear()
    {
        for(int i = transform.childCount-1; i>=0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
