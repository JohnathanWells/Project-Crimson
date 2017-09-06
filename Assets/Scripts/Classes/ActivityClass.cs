using System.Collections;
using UnityEngine;

public class ActivityClass{
    public enum sector { A, B, C, D, E };
    public enum category { Work, Shopping, Family, Personal };

    public string activityName;
    public sector area;
    public category activityCategory;

    public ActivityClass()
    {
        activityName = "Missing!";
        area = sector.A;
        activityCategory = category.Work;
    }

    public ActivityClass(string name, sector ar, category cat)
    {
        activityName = name;
        area = ar;
        activityCategory = cat;
    }

    //TODO
    //Effects of activity
}
