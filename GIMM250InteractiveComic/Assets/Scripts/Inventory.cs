using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class Inventory
{
    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();

        AddItem(new Item { itemType = Item.ItemType.Barrel, amount = 1});
        AddItem(new Item { itemType = Item.ItemType.Trigger, amount = 1});
        AddItem(new Item { itemType = Item.ItemType.GunBase, amount = 1});

        Debug.Log(itemList.Count);
    }
    public void AddItem(Item item)
    {
        itemList.Add(item);
    }
    public List<Item> GetItemList()
    {
        return itemList;
    }




}

