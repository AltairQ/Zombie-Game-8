using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using System.Linq;

public class GraphDrawer : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rt;
    [SerializeField]
    private UILineRenderer _line;

    public float[] Values;
    
    public void Draw()
    {
        List<float> tmpValues = new List<float>();
        int points = 20;
        for(int i = 0; i < points; i++)
        {
            tmpValues.Add(1f / (i + 1));
        }

        _line.Points = Normalize(tmpValues.ToArray());
    }

    private Vector2[] Normalize(float[] valuesArr)
    {
        List<Vector2> points = new List<Vector2>();
        List<float> values = new List<float>(valuesArr);
        float max = values.Max();
        float min = values.Min();

        float height = _rt.rect.height;
        float width = _rt.rect.width;

        for(int i = 0; i < values.Count; i++)
        {
            float x = -width / 2 + width * i / (values.Count - 1);
            float y = (values[i] - min) / (max - min) * height - height/2;
            points.Add(new Vector2(x, y));
        }

        return points.ToArray();
    }
}
