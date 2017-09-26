using System.Collections;
using UnityEngine;

[System.Serializable]
public class ItemClass{

    public enum itemType { Food, Cleaning, Hygiene, Medicines };
    

    int cost;
    itemType type;
    int stock;

    public ItemClass(int itemCost, itemType itemType, int stockQ)
    {
        cost = itemCost;
        type = itemType;
        stock = stockQ;
    }
}
