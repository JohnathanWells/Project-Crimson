using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum daysOfWeek {Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday};
public enum monthsOfYear { January, February, March, April, May, June, July, August, September, October, November, December}

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

    public void changeDayTo(int Month, int Day)
    {
        if (month > 0 && month < 13 && day > 0 && day < 32)
        {
            month = Month;
            day = Day;
            dayCount = calculateCurrentDay();
        }
    }

    public void advanceDay()
    {
        dayCount++;

        if (month == 10 || month == 12 || month == 1 || month == 3)
        {
            if (day + 1 > 31)
            {
                if (month + 1 > 12)
                {
                    month = 1;
                }
                else
                    month++;

                day = 1;
            }
            else
                day++;
        }
        else if (month == 11 || month == 4 || month == 6 || month == 9)
        {
            if (day + 1 > 30)
            {
                month++;
                day = 1;
            }
            else
                day++;
        }
        else if (month == 2)
        {
            if (day + 1 > 28)
            {
                month++;
                day = 1;
            }
            else
                day++;
        }
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

    public daysOfWeek calculateDayOfWeek()
    {
        return ((daysOfWeek)(dayCount % 7));
    }

    public string calculateMonth()
    {
        return ((monthsOfYear)(month - 1)).ToString();
    }

    public void copyTo(DayClass to)
    {
        to.day = day;
        to.month = month;
        to.dayCount = dayCount;
    }
}
