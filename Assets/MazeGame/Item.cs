using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{ 

    [SerializeField] private string itemType;
    public string getItemType()
    {
        return itemType;
    }

}
