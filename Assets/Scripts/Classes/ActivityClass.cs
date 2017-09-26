﻿using System.Collections;
using UnityEngine;

[System.Serializable]
public class ActivityClass{
    public enum sector { A, B, C, D, E };
    public enum category { Work, Shopping, Family, Personal };

    public string activityName;
    public sector area;
    public category activityCategory;
    public int cost;
    public int[] moraleChange = new int [4];
    public bool[] timeOfDayAvailability = new bool [3];
    public bool isItShop = false;
    public ShopClass shopAttached;

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
        moraleChange = morale;
        timeOfDayAvailability = ava;
        isItShop = isShop;
    }

    //TODO
    //Effects of activity
}
