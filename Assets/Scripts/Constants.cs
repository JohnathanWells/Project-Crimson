using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants{
    public static float mouseHoldSecondsBeforeAutoActivate = 0.7f;
    public static float mouseHoldSecondsPerItem = 0.1f;

    //Family Constants
    public static int familySize = 4;

    //Health constants


    //Morale Constants
    public static int depressionTop = 20;
    public static int unpaidServicesMoraleDrop = 5;
    public static int unshowerMoraleDrop = 2;
    public static int[] unstabilityEventsProbability = { 50, 25, 15, 10, 5 };


    //Music constants
    public static int lengthOfPlaylist = 15;

    //Game Constants
    public static DayClass endDate = new DayClass(10, 2);
}
