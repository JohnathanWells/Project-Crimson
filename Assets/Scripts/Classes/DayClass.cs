using System.Collections;
using UnityEngine;

public class DayClass{

    public ActivityClass[] MorningActivities;
    public ActivityClass[] AfternoonActivities;
    public ActivityClass[] EveningActivities;

    public DayClass(int MornActN, int AfternoonActN, int EvenActN)
    {
        MorningActivities = new ActivityClass[MornActN];
        AfternoonActivities = new ActivityClass[AfternoonActN];
        EveningActivities = new ActivityClass[EvenActN];
    }
}
