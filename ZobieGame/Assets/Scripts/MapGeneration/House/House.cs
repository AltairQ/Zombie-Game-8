using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour {
    private Rect _rect;
    private List<Wall> _walls = new List<Wall>();
    private List<Room> _rooms = new List<Room>();

    public float Width { get; set; }
    public float Height { get; set; }
    public float Depth { get; set; }

    public float MinRoomEdge { get; set; }
    public float MinRoomArea { get; set; }
    public float MaxRoomArea { get; set; }

    public void Generate()
    {
        _rect = new Rect(-Width / 2, -Depth / 2, Width, Depth);
        _walls.Clear();
        _rooms.Clear();

        var points = _rect.AllPoints();
        for(int i=0;i<4;i++)
        {
            AddWall(points[i], points[(i + 1) % 4]);
        }

        GenerateRooms(_rect);
        GenerateWindows();
        GenerateDoor();
    }

    private void AddWall(Vector2 p1, Vector2 p2)
    {
        _walls.Add(new Wall(p1, p2, Height));
    }

    private bool CanBeSplitVertically(Rect rect)
    {
        return rect.width > MinRoomEdge * 2;
    }
    private bool CanBeSplitHorizontally(Rect rect)
    {
        return rect.height > MinRoomEdge * 2;
    }

    private void GenerateRooms(Rect rect)
    {
        if(!CanBeSplitVertically(rect) && !CanBeSplitHorizontally(rect)) // we cannot split given rect
        { 
            _rooms.Add(new Room(rect));
            return;
        }
        if(RandomEndRoom(rect))
        {
            _rooms.Add(new Room(rect));
            return;
        }

        bool splitVertical = CanBeSplitVertically(rect);
        if(CanBeSplitHorizontally(rect) && CanBeSplitVertically(rect))
        {
            splitVertical = Random.Range(0, 2) == 0;
        }

        Vector2 p1, p2;
        if(splitVertical)
        {
            float deltaX = rect.width - 2 * MinRoomEdge;
            float randX = Random.Range(0, deltaX);
            p1 = new Vector2(rect.xMin + MinRoomEdge + randX, rect.yMin);
            p2 = p1 + new Vector2(0, rect.height);
        }
        else
        {
            float deltaY = rect.height - 2 * MinRoomEdge;
            float randY = Random.Range(0, deltaY);
            p1 = new Vector2(rect.xMin, rect.yMin + MinRoomEdge + randY);
            p2 = p1 + new Vector2(rect.width, 0);
        }

        AddWall(p1, p2);
        var newRects = rect.Split(p1, p2);
        GenerateRooms(newRects[0]);
        GenerateRooms(newRects[1]);
    }

    private bool RandomEndRoom(Rect rect)
    {
        float area = rect.Area();
        if(area > MaxRoomArea)
        {
            return false;
        }

        float createRoom = Random.Range(0, area / MinRoomArea); // the bigger area the lower chance
        return createRoom < 1.0f;
    }

    private void GenerateWindows()
    {
        for(int i=0;i<4;i++)
        {
            var initWall = _walls[i];
            var overlappingRooms = OverlapppingRooms(initWall);
            foreach (var room in overlappingRooms)
            {
                var points = room.Rect.AllPoints();
                for (int j = 0; j < 4; j++)
                {
                    if (initWall.ContainsInPart(points[j], points[j + 1]))
                    {
                        GenerateWindows(initWall, points[j], points[j + 1]);
                    }
                }
            }
        }
    }

    private void GenerateWindows(Wall wall, Vector2 p1, Vector2 p2)
    {
        if(p2.IsSmaller(p1))
        {
            Utils.Swap(ref p1, ref p2);
        }

        float windowSize = MinRoomEdge / 2;
        float distBetween = windowSize;

        float segmentLength = (p2-p1).magnitude;
        int windowsToPut = 0;
        float currentLenght = distBetween + windowSize;
        while(currentLenght < segmentLength)
        {
            currentLenght += distBetween + windowSize;
            windowsToPut++;
        }

        float finalLength = currentLenght -= distBetween + windowSize;
        Vector2 dir = (p2 - p1).normalized;
        float offset = (segmentLength - finalLength) / 2; // to center windows
        Vector2 windowStart = p1 + dir * (offset + distBetween/2);
        Vector2 windowEnd = windowStart + dir * windowSize;

        for (int i = 1; i <= windowsToPut; i++)
        {
            wall.AddPart(windowStart, windowEnd, Wall.PartType.Window);
            windowStart = windowEnd + dir * distBetween;
            windowEnd = windowStart + dir * windowSize;
        }
    }

    private void GenerateDoor()
    {
        int idx = Random.Range(0, 4);
        Wall doorWall = _walls[idx];
        doorWall.RandomWindowToDoor();
    }

    private List<Room> OverlapppingRooms(Wall w)
    {
        return _rooms.FindAll(r => w.Overlaps(r.Rect));
    }

    public GameObject Make()
    {
        GameObject go = new GameObject();
        go.name = "House";

        var floor = MakeFloor();
        floor.transform.parent = go.transform;

        foreach (var wall in _walls)
        {
            var wallGO = wall.Make();
            wallGO.transform.parent = go.transform;
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        go.transform.parent = transform;

        return go;
    }

    private GameObject MakeFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Quad);
        floor.name = "Floor";

        floor.transform.position = _rect.Center(0);
        floor.transform.localScale = new Vector3(_rect.width, _rect.height, 1);
        floor.transform.Rotate(new Vector3(90, 0, 0));

        return floor;
    }
}
