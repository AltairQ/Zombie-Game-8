using UnityEngine;

public abstract class MapObject 
{ 
    public Rect Rect { get; private set; }
    public MapObject(Rect rect)
    {
        Rect = rect;
    }

    private bool _generated = false;
    public bool IsGenerated()
    {
        return _generated;
    }
    
    public bool IsMade()
    {
        return _go != null;
    }
    private GameObject _go = null;

    public void Generate(bool forceGenerate = false)
    {
        if (_generated)
        {
            return;
        }

        var mapSystem = MapSystem.Get();
        if (forceGenerate || mapSystem == null || !mapSystem.LazyGenerationEnabled())
        {
            DoGenerate();
            _generated = true;
        }
        else
        {
            MapSystem.Get().LazyCreate(this);
        }
    }
    protected abstract void DoGenerate();

    public GameObject Make()
    {
        if(!IsGenerated())
        {
            Debug.LogError("MapObject not generated yet!");
            return null;
        }

        if (_go == null)
        {
            _go = DoMake();
        }
        return _go;
    }
    protected abstract GameObject DoMake();
}
