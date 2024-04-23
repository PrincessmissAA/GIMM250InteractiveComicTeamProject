using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ItemType 
    {
        Barrel,
        Trigger,
        Handle,
        GunBase,
    }
    public ItemType itemType;
    public int amount;
    
    //public string itemType;
    
}
