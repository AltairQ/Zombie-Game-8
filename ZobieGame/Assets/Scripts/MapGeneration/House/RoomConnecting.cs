using System.Collections.Generic;
using UnityEngine;

public class RoomConnecting
{
    public struct Edge
    {
        public int v;
        public int w;
        public int cost;
        public Edge(int v, int w)
        {
            this.v = v;
            this.w = w;
            cost = Random.Range(0, 100);
        }
    }

    public static List<Edge> GenerateConnections(List<Room> rooms, float doorSize)
    {
        var edges = GenerateEdges(rooms, doorSize);
        var mstEdges = MstFilter(edges, rooms.Count);
        return mstEdges;
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

    private static List<int> _parent;
    private static List<int> _amount;
    private static void InitUnionFind(int N)
    {
        _parent = new List<int>(N);
        _amount = new List<int>(N);
        for(int i = 0; i < N; i++)
        {
            _parent.Add(i);
            _amount.Add(1);
        }
    }

    private static int Find(int x)
    {
        if(_parent[x] == x)
        {
            return x;
        }
        return _parent[x] = Find(_parent[x]);
    }

    private static bool Union(Edge edge)
    {
        int v = edge.v;
        int w = edge.w;
        int fv = Find(v);
        int fw = Find(w);

        if(fv == fw)
        {
            return false;
        }

        if(_amount[fv] < _amount[fw])
        {
            Utils.Swap(ref fv, ref fw);
        }
        _amount[fv] += _amount[fw];
        _parent[fw] = fv;

        return true;
    }
    private static List<Edge> MstFilter(List<Edge> edges, int N)
    {
        InitUnionFind(N);
        edges.Sort((e1, e2) => e2.cost.CompareTo(e1.cost));
        List<Edge> mstEdges = new List<Edge>();
        
        foreach(var edge in  edges)
        {
            if(Union(edge))
            {
                mstEdges.Add(edge);
            }
        }

        return mstEdges;
    }
}