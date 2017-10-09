using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class sicknessClass {
    public int ID;
    public string name;
    public int moralePerDay;
    public int healthPerDay;
    public int recoveryTimeM;
    public int recoveryTimeN;
    public float minPRCheck;

    public sicknessClass(int id, string Name, int mpd, int hpd, int rtN, int rtM, float mPR)
    {
        ID = id;
        name = Name;
        moralePerDay = mpd;
        healthPerDay = hpd;
        recoveryTimeN = rtN;
        recoveryTimeM = rtM;
        minPRCheck = mPR;
    }

    public sicknessClass()
    {
        ID = 0;
        name = "Healthy";
        moralePerDay = 0;
        healthPerDay = 0;
        recoveryTimeN = 0;
        recoveryTimeM = 0;
        minPRCheck = 0;
    }

    public sicknessClass healthy()
    {
        return new sicknessClass(0, "Health", 0, 0, 0, 0, 0);
    }
}
