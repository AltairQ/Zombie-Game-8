using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class MapSystem
{
    private Queue<MapObject> _toGenerate = new Queue<MapObject>();
    private Stack<MapObject> _toMake = new Stack<MapObject>();
    public void LazyCreate(MapObject obj)
    {
        _toGenerate.Enqueue(obj);
    }

    [SerializeField]
    private bool _lazyGeneration = true;
    public bool LazyGenerationEnabled()
    {
        return _lazyGeneration;
    }

    [SerializeField]
    private int _generateBudget = 10;
    [SerializeField]
    private int _makeBudget = 10;

    private void ProceedLazyCreation(bool forceCreate = false)
    {
        int budget = forceCreate ? int.MaxValue : _generateBudget;
        while(budget > 0 && _toGenerate.Count > 0)
        {
            var obj = _toGenerate.Dequeue();
            obj.Generate(true);
            _toMake.Push(obj);
            budget--;
        }

        if(_toGenerate.Count > 0)
        {
            return;
        }

        budget = forceCreate ? int.MaxValue : _makeBudget;
        while (budget > 0 && _toMake.Count > 0)
        {
            var obj = _toMake.Pop();
            obj.Make();
            budget--;
        }
    }
}
