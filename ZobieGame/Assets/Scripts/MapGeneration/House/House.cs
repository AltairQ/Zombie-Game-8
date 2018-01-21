using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MapObject
{
    private List<Wall> _walls = new List<Wall>();
    private List<Wall> _corners = new List<Wall>();
    private List<Room> _rooms = new List<Room>();
    private HouseSettings _settings;
    public House(Rect rect) : base(rect)
    {
        _settings = GeneratorAssets.Get().HouseSettings;
    }

    public override void Generate()
    {
        _corners.Clear();
        _walls.Clear();
        _rooms.Clear();

        InitWalls();    

        CreateRooms(Rect);
        ConnectRooms();

        GenerateDoors();
        GenerateWindows();
        GenerateRooms(); // rooms need doors and windows to be properly generated
    }

    private void InitWalls()
    {
        var points = Rect.AllPoints();
        Vector2 cornerOff = new Vector2(Wall.WallDepth/2, Wall.WallDepth/2);
        for (int i = 0; i < 4; i++)
        {
            AddWall(points[i], points[(i + 1) % 4]);
            AddCorner(points[i] - cornerOff, points[i] + cornerOff); //creates corners
            _walls[i].ShadowsEnabled = true;
        }
    }

    private void GenerateRooms()
    {
        foreach(var room in _rooms)
        {
            room.Generate();
        }
    }

    private void AddWall(Vector2 p1, Vector2 p2)
    {
        _walls.Add(new Wall(p1, p2, _settings.Height));
    }

    private void AddCorner(Vector2 p1, Vector2 p2)
    {
        _corners.Add(new Wall(p1, p2, _settings.Height));
    }

    private bool CanBeSplitVertically(Rect rect)
    {
        return rect.width > _settings.MinRoomEdge * 2;
    }
    private bool CanBeSplitHorizontally(Rect rect)
    {
        return rect.height > _settings.MinRoomEdge * 2;
    }

    private void CreateRooms(Rect rect)
    {
        if(!CanBeSplitVertically(rect) && !CanBeSplitHorizontally(rect)) // we cannot split given rect
        { 
            AddRoom(rect);
            return;
        }
        if(RandomEndRoom(rect))
        {
            AddRoom(rect);
            return;
        }

        bool splitVertical = CanBeSplitVertically(rect);
        if(CanBeSplitHorizontally(rect) && CanBeSplitVertically(rect))
        {
            splitVertical = Random.Range(0, 2) == 0;
        }

        Vector2 p1, p2;
        float minEdge = _settings.MinRoomEdge;
        if (splitVertical)
        {
            float deltaX = rect.width - 2 * minEdge;
            float randX = Random.Range(0, deltaX);
            p1 = new Vector2(rect.xMin + minEdge + randX, rect.yMin);
            p2 = p1 + new Vector2(0, rect.height);
        }
        else
        {
            float deltaY = rect.height - 2 * minEdge;
            float randY = Random.Range(0, deltaY);
            p1 = new Vector2(rect.xMin, rect.yMin + minEdge + randY);
            p2 = p1 + new Vector2(rect.width, 0);
        }

        AddWall(p1, p2);
        var newRects = rect.Split(p1, p2);
        CreateRooms(newRects[0]);
        CreateRooms(newRects[1]);
    }
    
    private void AddRoom(Rect rect)
    {
        Room room = new Room(rect);
        _rooms.Add(room);
    }

    private void ConnectRooms()
    {
        var edges = RoomConnecting.GenerateConnections(_rooms, _settings.DoorSize);
        foreach(var edge in edges)
        {
            var doorPoints = RoomConnecting.Connection(_rooms[edge.v], _rooms[edge.w], _settings.DoorSize);
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
        if(area > _settings.MaxRoomArea)
        {
            return false;
        }

        float createRoom = Random.Range(0, area / _settings.MinRoomArea); // the bigger area the lower chance
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
        float windowSize = _settings.WindowSize;
        float distBetween = _settings.SpaceBetweenWindows;

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

    private void GenerateDoors()
    {
        List<int> idxs = new List<int>();
        idxs.Add(Random.Range(0, 4));
        int other = Random.Range(0, 4);
        if(other == idxs[0])
        {
            other = (other + Random.Range(1, 4)) % 4;    
        }
        if(Random.Range(0f, 1f) <= _settings.SecondDoorChance)
        {
            idxs.Add(other);
        }
        
        foreach(int idx in idxs)
        {
            Wall doorWall = _walls[idx];

            var rooms = OverlapppingRooms(doorWall);
            int roomIdx = Random.Range(0, rooms.Count);
            Room doorRoom = rooms[roomIdx];

            var points = PointsInWall(doorWall, doorRoom);
            if(points == null)
            {
                Debug.LogWarning("GenerateDoors() null door points");
                continue;
            }

            AddDoor(doorWall, points[0], points[1]);
        }
    }

    private void AddDoor(Wall wall, Vector2 p1, Vector2 p2)
    {
        Vector2 center = (p1 + p2) / 2;
        Vector2 dir = (p1 - p2).normalized;
        wall.AddPart(center - dir * _settings.DoorSize / 2, center + dir * _settings.DoorSize / 2, Wall.PartType.Door);
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

    public override GameObject Make()
    {
        GameObject go = Utils.TerrainObject("House");
        
        var floor = MakeFloor(go);
        var walls = MakeWalls(go);
        var rooms = MakeRooms(go);
        if (_settings.Combine)
        {
            go.Combine(floor);
            go.Combine(walls);
            var mr = go.GetComponent<MeshRenderer>();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            mr.receiveShadows = false;
        }
       
        return go;
    }

    private GameObject MakeFloor(GameObject parent)
    {
        var floor = Rect.ToTerrainQuad("Floor", ObjectHeight.Floor);
        floor.SetParent(parent);
        floor.SetMaterial(GeneratorAssets.Get().FloorMaterial);
        return floor;
    }

    private GameObject MakeWalls(GameObject parent)
    {
        GameObject walls = new GameObject("Walls");
        walls.SetParent(parent);

        foreach (var wall in _walls)
        {
            var wallGO = wall.Make();
            wallGO.SetParent(walls);
        }
        foreach (var wall in _corners)
        {
            var wallGO = wall.Make();
            wallGO.SetParent(walls);
        }

        return walls;
    }

    private GameObject MakeRooms(GameObject parent)
    {
        GameObject rooms = new GameObject("Rooms");
        rooms.SetParent(parent);

        foreach (var room in _rooms)
        {
            var roomGO = room.Make();
            if(roomGO == null)
            {
                continue;
            }

            roomGO.SetParent(rooms);
        }

        return rooms;
    }
}
