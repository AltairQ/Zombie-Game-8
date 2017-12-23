using System.Collections.Generic;
using UnityEngine;

public class RoomConnecting
{
    public struct Edge
    {
        public int v;
        public int w;

        public Edge(int v, int w)
        {
            this.v = v;
            this.w = w;
        }
    }

    public static List<Edge> GenerateConnections(List<Room> rooms, float doorSize)
    {
        var edges = GenerateEdges(rooms, doorSize);
        return edges;
    }

    private static List<Edge> GenerateEdges(List<Room> rooms, float doorSize)
    {
        List<Edge> edges = new List<Edge>();
        for(int i = 0; i < rooms.Count; i++)
        {
            for(int j = i+1; j < rooms.Count; j++)
            {
                if(Connection(rooms[i],rooms[j], doorSize) != null)
                {
                    edges.Add(new Edge(i, j));
                }
            }
        }

        return edges;
    }

    public static Vector2[] Connection(Room r1, Room r2, float doorSize)
    {
        var points1 = r1.Rect.AllPoints();
        var points2 = r2.Rect.AllPoints();
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                var merge = Utils.Merge(points1[i], points1[i + 1], points2[j], points2[j + 1]); 
                if(merge != null && (merge[1] - merge[0]).magnitude > doorSize)
                {
                    return merge;
                }
            }
        }

        return null;
    }
}