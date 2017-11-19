using System.Collections;
using UnityEngine;

[System.Serializable]
public class ShopClass {

    public string shopName;
    public ItemClass[] inventory;
    public int id;
    public int[] budgets = new int[4];
    public int[] ShortageRates = new int[4];
    public float multiplier = 1;

    public ShopClass(int foodBud, int medBud, int HygBud, int cleanBud, int ID, string name, float mult)
    {
        shopName = name;

        inventory = new ItemClass[4];

        inventory[0] = new ItemClass(ItemClass.itemType.Food, 0);
        inventory[1] = new ItemClass(ItemClass.itemType.Cleaning, 0);
        inventory[2] = new ItemClass(ItemClass.itemType.Hygiene, 0);
        inventory[3] = new ItemClass(ItemClass.itemType.Medicines, 0);

        budgets[0] = foodBud;
        budgets[1] = cleanBud;
        budgets[2] = HygBud;
        budgets[3] = medBud;

        id = ID;

        multiplier = Mathf.Clamp( mult, 1, 100);
    }

    public void setItems(int foodCost, int foodStock, int cleaningCost, int cleaningStock, int hygieneCost, int hygieneStock, int medicineCost, int medicineStock)
    {
        inventory[0].setItem(Mathf.RoundToInt(foodCost * multiplier), foodStock);
        inventory[1].setItem(Mathf.RoundToInt(cleaningCost * multiplier), cleaningStock);
        inventory[2].setItem(Mathf.RoundToInt(hygieneCost * multiplier), hygieneStock);
        inventory[3].setItem(Mathf.RoundToInt(medicineCost * multiplier), medicineStock);
    }

    public void setShortageRates(int f, int h, int c, int m)
    {
        ShortageRates[0] = f;
        ShortageRates[1] = h;
        ShortageRates[2] = c;
        ShortageRates[3] = m;
    }

    public void setBudgets(int foodBud, int medBud, int HygBud, int CleanBud)
    {
        budgets[0] = foodBud;
        budgets[1] = medBud;
        budgets[2] = HygBud;
        budgets[3] = CleanBud;
    }

    void externalItemSale()
    {
        for (int n = 0; n < 4; n++)
        {
            inventory[n].buyItem(ShortageRates[n]);
        }
    }
}
