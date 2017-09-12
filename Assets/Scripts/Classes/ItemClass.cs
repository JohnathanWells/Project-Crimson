using System.Collections;
using UnityEngine;

public class ItemClass{

    public enum itemType { Food, Cleaning, Hygiene, Medicines };

    int cost;
    itemType type;

    public ItemClass(int itemCost, itemType itemType)
    {
        cost = itemCost;
        type = itemType;
    }
}
