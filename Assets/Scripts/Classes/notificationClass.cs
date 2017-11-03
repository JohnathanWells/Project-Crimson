using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum notificationType{SimpleText, ActivityDescription};

public class notificationClass{

    public string text;
    public int pictureNum;
    public int[] moodChange;
    public notificationType type;

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
    }

    public notificationClass(ActivityClass activity)
    {
        moodChange = new int[activity.moraleChange.Length];
        for (int i = 0; i < activity.moraleChange.Length; i++)
            moodChange[i] = activity.moraleChange[i];

            text = activity.postActivityDescription;
        pictureNum = activity.pictureNumberUsed;
        type = notificationType.ActivityDescription;
    }

    public notificationClass(string Text, int pN)
    {
        text = Text;
        pictureNum = pN;
        type = notificationType.SimpleText;
    }
}
