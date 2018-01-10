using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.ComponentModel;

[CreateAssetMenu(fileName = "LootSettings", menuName = "Generator/LootSettings")]
public class LootSettings : ScriptableObject
{
    [Serializable]
    private class ListItem
    {
        public GameObject prefab = null;
        public int tokens = 0;

        public string chance = "";
        public void SetChance(float value)
        {
            chance =  ((int)(value * 100)) + "%";
        }
    }

    [SerializeField]
    private List<ListItem> _items;

    public void OnValidate()
    {
        FixItems();
        CalculateListChances();
    }

    private void FixItems()
    {
        _items.RemoveAll(item => item == null);
    }

    private int GetTokenSum()
    {
        int tokenSum = 0;
        _items.ForEach(item => tokenSum += item.tokens);
        return tokenSum;
    }

    private void CalculateListChances()
    {
        int tokenSum = GetTokenSum();
        if(tokenSum == 0)
        {
            return;
        }

        _items.ForEach(item => item.SetChance((float)item.tokens / tokenSum));
    }

    public GameObject GetRandomItem()
    {
        int tokenSum = GetTokenSum();
        int tokenNum = UnityEngine.Random.Range(1, tokenSum+1);
        //Debug.Log("num: " + tokenNum + ", sum: " + tokenSum);

        int currentSum = 0;
        foreach (var item in _items)
        {
            currentSum += item.tokens;
            if(tokenNum <= currentSum)
            {
                return Instantiate(item.prefab);
            }
        }
        return null;
    }
}