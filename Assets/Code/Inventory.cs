using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //Bsp Liste von Items
    public List<ItemType> items = new();

    public event System.Action<ItemType> onItemCollected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool HasItem(ItemType type)
    {
        return CountItems(type) > 0;
    }

    public void AddItem(Item item)
    {
        items.Add(item.type); //ItemType aus dem neuen item holen und zur Liste hinzufügen
        onItemCollected?.Invoke(item.type);
    }

    public int CountItems(ItemType type)
    {
        int count = 0;

        //items.Count -> Länge der Liste
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == type)
                count++;
        }

        return count;
    }

}
