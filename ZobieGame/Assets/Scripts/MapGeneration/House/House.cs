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

    public float DoorSize { get; set; }
    public float WindowSize { get; set; }
    public float SpaceBetweenWindows { get; set; }

    public void Generate(Vector2 center)
    {
        _rect = new Rect(center.x - Width / 2, center.y - Depth / 2, Width, Depth);
        _walls.Clear();
        _rooms.Clear();

        var points = _rect.AllPoints();
        for(int i=0;i<4;i++)
        {
            AddWall(points[i], points[(i + 1) % 4]);
            _walls[i].ShadowsEnabled = true;
        }

        GenerateRooms(_rect);
        ConnectRooms();

        GenerateDoor();
        GenerateWindows();
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
    
    private void ConnectRooms()
    {
        var edges = RoomConnecting.GenerateConnections(_rooms, DoorSize);
        foreach(var edge in edges)
        {
            var doorPoints = RoomConnecting.Connection(_rooms[edge.v], _rooms[edge.w], DoorSize);
            var wall = _walls.Find(w => w.ContainsInPart(doorPoints[0], doorPoints[1]));
            if(wall != null)
            {
                AddDoor(wall, doorPoints[0], doorPoints[1]);
            }
        }
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
        float windowSize = WindowSize;
        float distBetween = SpaceBetweenWindows;

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
        int wallIdx = Random.Range(0, 4);
        Wall doorWall = _walls[wallIdx];

        var rooms = OverlapppingRooms(doorWall);
        int roomIdx = Random.Range(0, rooms.Count);
        Room doorRoom = rooms[roomIdx];

        var points = PointsInWall(doorWall, doorRoom);
        AddDoor(doorWall, points[0], points[1]);
    }

    private void AddDoor(Wall wall, Vector2 p1, Vector2 p2)
    {
        Vector2 center = (p1 + p2) / 2;
        Vector2 dir = (p1 - p2).normalized;
        wall.AddPart(center - dir * DoorSize / 2, center + dir * DoorSize / 2, Wall.PartType.Door);
    }

    private List<Room> OverlapppingRooms(Wall w)
    {
        return _rooms.FindAll(r => w.Overlaps(r.Rect));
    }

    private Vector2[] PointsInWall(Wall wall, Room room)
    {
        var points = room.Rect.AllPoints();
        for (int j = 0; j < 4; j++)
        {
            if (wall.ContainsInPart(points[j], points[j + 1]))
            {
                return new Vector2[] { points[j], points[j + 1] };
            }
        }

        return null;
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
        return _rect.ToQuad("Floor", 0.02f);
    }
}
