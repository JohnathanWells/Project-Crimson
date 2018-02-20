using System.Collections;
using UnityEngine;

[System.Serializable]
public class ActivityClass{
    public enum sector { A, B, C, D, E, F };
    public enum category { Work, Shopping, Family, Personal, ALL };

    public string activityName;
    public sector area;
    public category activityCategory;
    public int cost;
    public int[] moraleChange = new int [4];
    public bool[] timeOfDayAvailability = new bool [3];
    public float[] locationOnMap = new float [2];
    public bool isItShop = false;
    public bool paysService = false;
    public ShopClass shopAttached;
    

    public string postActivityDescription = "ERROR";
    public int pictureNumberUsed = 0;

    public ActivityClass()
    {
        activityName = "Missing!";
        area = sector.A;
        activityCategory = category.Work;
    }

    public ActivityClass(string name, sector ar, category cat, int ActCost, int []morale, bool [] ava, bool isShop)
    {
        activityName = name;
        area = ar;
        activityCategory = cat;
        cost = ActCost;

        for (int n = 0; n < morale.Length; n++)
            moraleChange[n] = morale[n];

        for (int n = 0; n < ava.Length; n++)
            timeOfDayAvailability[n] = ava[n];

        isItShop = isShop;
    }

    public void setStore(ShopClass store)
    {
        shopAttached = store;
    }

    public void setNotiInfo(string description, int pictureUsed)
    {
        postActivityDescription = description;
        pictureNumberUsed = pictureUsed;
    }

    public void setPointerLocation(float[] location)
    {
        for(int n = 0; n < location.Length; n++)
            locationOnMap[n] = location[n];
    }
}
