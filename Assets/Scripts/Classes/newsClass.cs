using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class newsClass{
    public DayClass date;
    public string title;
    public string content;
    public int imageAttached;

    public newsClass()
    {
        date = new DayClass(10, 1);
        title = "ERROR";
        content = "Missing news";
    }

    //public newsClass(DayClass d, string t, string c)
    //{
    //    d = date;
    //    title = t;
    //    content = c;
    //}

    public newsClass(DayClass d, string t, string c, int picture)
    {
        date = d;
        title = t;
        content = c;
        imageAttached = picture;
    }

}
