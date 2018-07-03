using System.Collections.Generic;
using UnityEngine;

public class City : MapObject
{   
    private List<Street> _streets = new List<Street>();
    private List<Estate> _estates = new List<Estate>();
    private List<Vector2> _initStreetPoints = new List<Vector2>();
    public List<Vector2> InitStreetPoints { get { return _initStreetPoints; } }

    private CitySettings _settings;
    public City(Rect rect) : base(rect)
    {
        _settings = GeneratorAssets.Get().CitySettings;
    }

    private float _firstShiftX = 0.0f;
    private float _firstShiftY = 0.0f;
    public void SetInitStreet(Vector2 edgePoint)
    {
        if (!Rect.ContainsOnEdge(edgePoint))
        {
            Debug.LogError("City.SetInitStreet() point not on edge!");
            return;
        }

        var points = Rect.AllPoints();

        Vector2 p1 = Vector2.zero, p2 = Vector2.zero;
        for (int i = 0; i < 4; i++)
        {
            if (Utils.Contains(points[i], points[i + 1], edgePoint))
            {
                p1 = points[i];
                p2 = points[i+1];
                break;
            }
        }

        Utils.OrderSwap(ref p1, ref p2);
        if(Utils.TheSame(p1.x, p2.x))
        {
            _firstShiftY = edgePoint.y;
        }
        if(Utils.TheSame(p1.y, p2.y))
        {
            _firstShiftX = edgePoint.x;
        }
    }

    protected override void DoGenerate()
    {
        _streets.Clear();
        _estates.Clear();
        _initStreetPoints.Clear();

        List<Rect> rects = new List<Rect> {
            Rect
        };
        rects = AddFirstStreet(rects, false);
        rects = AddFirstStreet(rects, true);

        if (rects.Count == 1)
        {
            GenerateEstates(rects[0], 0);
        }
        else
        {
            foreach(var rect in rects)
            {
                GenerateEstates(rect, 1);
            }
        }

        _streets.ForEach(street => street.Generate()); //have to be generated after all streats are created to detect lamp collisions
    }

    private List<Rect> AddFirstStreet(List<Rect> rects, bool vertical)
    {
        float shift = vertical ? _firstShiftX : _firstShiftY;
        if(Utils.TheSame(shift, 0f))
        {
            return rects;
        }

        List<Rect> ans = new List<Rect>();
        foreach (var rect in rects)
        {
            var twoRects = NormalSplit(rect, vertical, 0, shift);
            ans.Add(twoRects[0]);
            ans.Add(twoRects[1]);
        }
        return ans;
    }

    private void AddStreet(Vector2 p1, Vector2 p2, int deep)
    {
        Utils.OrderSwap(ref p1, ref p2);
        Vector2 shift = (p2 - p1).normalized;
        if (IsOnCityEdge(p1))
        {
            p1 += StreetCut(deep) * shift;
        }
        if (IsOnCityEdge(p2))
        {
            p2 -= StreetCut(deep) * shift;
        }

        if(deep == 0)
        {
            AddInitPoint(p1);
            AddInitPoint(p2);
        }

        Rect streetRect = Utils.SegmentToRect(p1, p2, _settings.StreetSize);
        Street street = new Street(this, streetRect);
        _streets.Add(street);
    }

    private void AddInitPoint(Vector2 p)
    {
        if (IsOnCityEdge(p))
        {
            _initStreetPoints.Add(p);
        }        
    }

    private float StreetCut(int deep)
    {
        if(deep == 0)
        {
            return 0f;
        }

        return _settings.MinEstateEdge / 2;
    }

    private void AddEstate(Rect rect)
    {
        var estate = new Estate(rect);
        estate.Generate();
        _estates.Add(estate);
    }

    private bool CanBeSplitVertically(Rect rect)
    {
        return rect.width > _settings.MinEstateEdge * 2;
    }
    private bool CanBeSplitHorizontally(Rect rect)
    {
        return rect.height > _settings.MinEstateEdge * 2;
    }

    private void GenerateEstates(Rect rect, int deep)
    {
        if (!CanBeSplitVertically(rect) && !CanBeSplitHorizontally(rect)) // we cannot split given rect
        {
            AddEstate(rect);
            return;
        }
        if (RandomEndEstate(rect))
        {
            AddEstate(rect);
            return;
        }

        bool splitVertical = CanBeSplitVertically(rect);
        bool crossroad = false;
        if (CanBeSplitHorizontally(rect) && CanBeSplitVertically(rect))
        {
            splitVertical = Random.Range(0, 2) == 0;
            crossroad = Random.Range(0, 2) == 0;
        }

        Rect[] newRects;
        if(crossroad)
        {
            newRects = CrossroadSplit(rect, deep);
        }
        else
        {
            newRects = NormalSplit(rect, splitVertical, deep);
        }   
        
        foreach(var newRect in newRects)
        {
            GenerateEstates(newRect, deep+1);
        }
    }

    private Rect[] CrossroadSplit(Rect rect, int deep)
    {
        var vertSplit = NormalSplit(rect, true, deep);
        var leftSplit = NormalSplit(vertSplit[0], false, deep);

        float shift = Mathf.Min(leftSplit[0].yMax, leftSplit[1].yMax);
        var rightSplit = NormalSplit(vertSplit[1], false, deep, shift);
        return new Rect[] { leftSplit[0], leftSplit[1], rightSplit[0], rightSplit[1] };
    }

    private Rect[] NormalSplit(Rect rect, bool splitVertical, int deep, float shift = float.PositiveInfinity)
    {
        Vector2 p1, p2;
        float minEdge = _settings.MinEstateEdge;
        bool randomShift = float.IsPositiveInfinity(shift);
        if (splitVertical)
        {       
            if (randomShift)
            {
                float deltaX = rect.width - 2 * minEdge;
                float randX = Random.Range(0, deltaX);
                shift = rect.xMin + minEdge + randX;
            }
            p1 = new Vector2(shift, rect.yMin);
            p2 = p1 + new Vector2(0, rect.height);
        }
        else
        {         
            if (randomShift)
            {
                float deltaY = rect.height - 2 * minEdge;
                float randY = Random.Range(0, deltaY);
                shift = rect.yMin + minEdge + randY;
            }
            p1 = new Vector2(rect.xMin, shift);
            p2 = p1 + new Vector2(rect.width, 0);
        }

        AddStreet(p1, p2, deep);
        return rect.Split(p1, p2);
    }

    private bool RandomEndEstate(Rect rect)
    {
        float area = rect.Area();
        //if (area > MaxEstateArea)
        //{
        //    return false;
        //}

        float minArea = _settings.MinEstateEdge * _settings.MinEstateEdge; 
        float createEstate = Random.Range(0, area / minArea); // the bigger area the lower chance
        return createEstate < 1.0f;
    }

    private bool IsOnCityEdge(Vector2 p)
    {
        return Rect.ContainsOnEdge(p);
    }

    public bool CheckOnStreetCollision(Street notToCheckStreet, Rect area)
    {
        return _streets.Find(street =>
        street.Rect != notToCheckStreet.Rect && street.Rect.Overlaps(area)
        ) != null;
    }

    protected override GameObject DoMake()
    {
        GameObject go = Utils.TerrainObject("City");
        MakeStreets(go);

        foreach (var estate in _estates)
        {
            var estateGO = estate.Make();
            estateGO.SetParent(go);
        }        
        return go;
    }

    private GameObject MakeStreets(GameObject parent)
    {
        if(_streets.Count == 0)
        {
            return null;
        }

        GameObject streets = new GameObject("Streets");
        streets.SetParent(parent);
        foreach (var street in _streets)
        {
            var streetGO = street.Make();
            streetGO.SetParent(streets);
        }

        return streets;
    }
}