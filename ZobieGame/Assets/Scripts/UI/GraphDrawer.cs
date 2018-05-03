using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using System.Linq;

public class GraphDrawer : MonoBehaviour
{
    private RectTransform _background;
    private UILineRenderer _line;
    private List<float> _values = new List<float>();
    public List<float> Values
    {
        get { return _values; }
        set
        {
            _values = value;
            Draw();
        }
    }

    private int _maxPoints = 10;
    public int MaxPoints
    {
        get { return _maxPoints; }
        set
        {
            _maxPoints = value;
            Draw();
        }
    }

    public void Init(RectTransform background, Color lineColor)
    {
        _background = background;
        _line = GetComponent<UILineRenderer>();
        _line.color = lineColor;

        _values.Clear();
    }
    
    public void AddValue(float value)
    {
        _values.Add(value);
        Draw();
    }

    private void Draw()
    {
        int pointsToDraw = Mathf.Min(_maxPoints, _values.Count);
        float[] valuesToDraw = new float[pointsToDraw];
        for(int i = 0; i < pointsToDraw; i++)
        {
            valuesToDraw[i] = _values[_values.Count - pointsToDraw + i];
        }

        _line.Points = Normalize(valuesToDraw);
    }

    private Vector2[] Normalize(float[] valuesArr)
    {
        List<Vector2> points = new List<Vector2>();
        List<float> values = new List<float>(valuesArr);
        float max = values.Max();
        float min = values.Min();

        float height = _background.rect.height;
        float width = _background.rect.width;

        for(int i = 0; i < values.Count; i++)
        {
            float x = -width / 2 + width * i / (_maxPoints - 1); // use max points not to stretch graph if there are less points
            float y = (values[i] - min) / (max - min) * height - height/2;
            points.Add(new Vector2(x, y));
        }

        return points.ToArray();
    }
}
