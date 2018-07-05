using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphsManager : MonoBehaviour
{
    [SerializeField]
    private GraphDrawer _graphDrawerPrefab;
    [SerializeField]
    private GameObject _graphDescriptionPrefab;

    [SerializeField]
    private RectTransform _graphsBackground;
    [SerializeField]
    private RectTransform _graphsDescriptions;

    private Dictionary<string, GraphDrawer> _graphs = new Dictionary<string, GraphDrawer>();

    //private void Start()
    //{
    //    var g1 = CreateGraph("graph1", Color.green);
    //    var g2 = CreateGraph("graph2", Color.blue);
    //    var g3 = CreateGraph("graph3", Color.red);
    //    g1.SetMinValue(-1);
    //    g1.SetMaxValue(10);
    //    g2.SetMinValue(0);

    //    for (int i = 0; i < 5; i++)
    //    {
    //        g1.AddValue(i);
    //        g2.AddValue(i * i);
    //        g3.AddValue(1);
    //    }
    //}

    public GraphDrawer CreateGraph(string name, Color lineColor)
    {
        GraphDrawer graph = Instantiate(_graphDrawerPrefab, _graphsBackground);
        graph.Init(_graphsBackground, lineColor);
        _graphs.Add(name, graph);

        var description = Instantiate(_graphDescriptionPrefab, _graphsDescriptions);
        var text = description.GetComponentInChildren<Text>();
        text.text = name;
        text.color = lineColor;

        return graph;
    }

    public GraphDrawer GetGraph(string name)
    {
        GraphDrawer ans;
        _graphs.TryGetValue(name, out ans);
        return ans;
    }

    public bool AddValue(string graphName, float value)
    {
        var graph = GetGraph(graphName);
        if(!graph)
        {
            return false;
        }

        return graph.AddValue(value);
    }

    public bool SetMaxValue(string graphName, float maxValue)
    {
        var graph = GetGraph(graphName);
        if (!graph)
        {
            return false;
        }

        graph.SetMaxValue(maxValue);
        return true;
    }

    public bool SetMinValue(string graphName, float minValue)
    {
        var graph = GetGraph(graphName);
        if (!graph)
        {
            return false;
        }

        graph.SetMinValue(minValue);
        return true;
    }

    public bool SetMaxPoints(string graphName, int maxPoints)
    {
        var graph = GetGraph(graphName);
        if (!graph)
        {
            return false;
        }

        graph.MaxPoints = maxPoints;
        return true;
    }

}
