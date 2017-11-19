using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum notificationType{SimpleText, ActivityDescription};

[System.Serializable]
public class notificationClass{

    public string text;
    public int pictureNum;
    //public Color color = Color.white;
    public int[] moodChange;
    public notificationType type;
    public float[] rgb = new float[4];

    public notificationClass(int premade)
    {
        if (premade == 0)
        {
            text = "Another day passes.";
            type = notificationType.SimpleText;
        }
        else
        {
            text = "ERROR";
        }

        rgb[0] = 1;
        rgb[1] = 1;
        rgb[2] = 1;
        rgb[3] = 1;
    }

    public notificationClass(ActivityClass activity)
    {
        moodChange = new int[activity.moraleChange.Length];
        for (int i = 0; i < activity.moraleChange.Length; i++)
            moodChange[i] = activity.moraleChange[i];

            text = activity.postActivityDescription;
        pictureNum = activity.pictureNumberUsed;
        type = notificationType.ActivityDescription;
        rgb[0] = 1;
        rgb[1] = 1;
        rgb[2] = 1;
        rgb[3] = 1;
    }

    public notificationClass(string Text, int pN)
    {
        text = Text;
        pictureNum = pN;
        type = notificationType.SimpleText;
        rgb[0] = 1;
        rgb[1] = 1;
        rgb[2] = 1;
        rgb[3] = 1;
    }

    public notificationClass(string Text, int pN, Color col)
    {
        text = Text;
        pictureNum = pN;
        type = notificationType.SimpleText;
        rgb[0] = col.r;
        rgb[1] = col.g;
        rgb[2] = col.b;
        rgb[3] = col.a;
    }

    public Color color()
    {
        return new Color(rgb[0], rgb[1], rgb[2], rgb[3]);
    }

    public void setColor(Color col)
    {
        rgb[0] = col.r;
        rgb[1] = col.g;
        rgb[2] = col.b;
        rgb[3] = col.a;
    }
}
