using System.Collections;
using UnityEngine;

[System.Serializable]
public class DayClass{
    public int month;
    public int day;

    public int dayCount;

    public DayClass(int Month, int Day)
    {
        month = Month;
        day = Day;

        dayCount = calculateCurrentDay();
    }

    public int calculateCurrentDay()
    {
        int n = 10;
        int ac = 0;

        while (n < month)
        {
            if (n == 10 || n == 12 || n == 1 || n == 3)
            {
                ac += 31;
            }
            else if (n == 11)
            {
                ac += 30;
            }
            else if (n == 2)
            {
                ac += 28;
            }

            n++;
            if (n > 12)
            {
                n= (n % 12) + 1;
            }
        }

        ac += day;

        return ac;
    }
}
