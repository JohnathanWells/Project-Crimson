using System.Collections;
using UnityEngine;

[System.Serializable]
public class ItemClass{

    public enum itemType { Food, Cleaning, Hygiene, Medicines };
    
    int cost;
    itemType type;
    int stock;

    public ItemClass(itemType itemType, int stockN)
    {
        type = itemType;
        stock = stockN;
    }

    public ItemClass(itemType itemType)
    {
        type = itemType;
    }

    public itemType getType()
    {
        return type;
    }

    public void setItem(int itemCost, int stockQ)
    {
        cost = itemCost;
        stock = stockQ;
    }

    public int getCost()
    {
        return cost;
    }

    public void setItemCost(int itemCost)
    {
        cost = itemCost;
    }

    public int getCostPerUnit()
    {
        return cost;
    }

    public int getStock()
    {
        return stock;
    }

    public void buyItem(int quantity)
    {
        stock = Mathf.Clamp(stock - quantity, 0, stock);
    }
}
