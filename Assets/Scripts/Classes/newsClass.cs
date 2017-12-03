using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class newsClass{
    public DayClass date;
    public string title;
    public string content;
    public Sprite imageAttached;
    public bool featuresImage = false;

    public newsClass()
    {
        date = new DayClass(10, 1);
        title = "ERROR";
        content = "Missing news";
        featuresImage = false;
    }

    public newsClass(DayClass d, string t, string c)
    {
        d = date;
        title = t;
        content = c;
        featuresImage = false;
    }

    public newsClass(DayClass d, string t, string c, Sprite picture)
    {
        d = date;
        title = t;
        content = c;
        imageAttached = picture;
        featuresImage = true;
    }

}
