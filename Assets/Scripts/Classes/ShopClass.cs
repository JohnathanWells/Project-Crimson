using System.Collections;
using UnityEngine;

[System.Serializable]
public class ShopClass {

    string shopName;
    public ItemClass[] inventory;
    public int id;

    public ShopClass(int foodItems, int cleaningItems, int hygieneItems, int medicineItems, int ID, string name)
    {
        shopName = name;

        inventory = new ItemClass[4];

        inventory[0] = new ItemClass(ItemClass.itemType.Food, foodItems);
        inventory[1] = new ItemClass(ItemClass.itemType.Cleaning, cleaningItems);
        inventory[2] = new ItemClass(ItemClass.itemType.Hygiene, hygieneItems);
        inventory[3] = new ItemClass(ItemClass.itemType.Medicines, medicineItems);

        id = ID;
    }

    public void setCosts(int foodCost, int cleaningCost, int hygieneCost, int medicineCost)
    {
        inventory[0].setItemCost(foodCost);
        inventory[1].setItemCost(cleaningCost);
        inventory[2].setItemCost(hygieneCost);
        inventory[3].setItemCost(medicineCost);
    }
}
