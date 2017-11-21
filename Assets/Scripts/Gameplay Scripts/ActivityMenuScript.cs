using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityMenuScript : MonoBehaviour {

    [Header("Buttons")]
    public Transform[] ActivityButtons;
    public CategoryScript[] categoryButtons;

    [Header("References")]
    public GameManager Manager;

    [Header("Time Stuff")]
    private DayClass currentDay;
    private timeOfDay time;
    private ActivityClass.category selectedCategory;

    [Header("Activities")]
    private ActivityClass[] mornAct;
    private ActivityClass[] noonAct;
    private ActivityClass[] evnAct;

    public void updateMenu()
    {
        mornAct = Manager.mornActivities.ToArray();
        noonAct = Manager.noonActivities.ToArray();
        evnAct = Manager.evenActivities.ToArray();

        time = Manager.currentTime;
        changeActCat(ActivityClass.category.Work);
    }

    public void changeActCat(ActivityClass.category Category)
    {
        ActivityClass[] temp;
        selectedCategory = Category;
        int count = 0;

        //Debug.Log(time);

        categoryButtons[(int)Category].SendMessage("turnOn");

        if (time == timeOfDay.morning)
        {
            temp = mornAct;
        }
        else if (time == timeOfDay.afternoon)
        {
            temp = noonAct;
        }
        else /*if (time == timeOfDay.Evening)*/
        {
            temp = evnAct;
        }

        for (int n = 0; n < temp.Length && count < ActivityButtons.Length; n++)
        {
            if (temp[n].activityCategory == selectedCategory)
            {
                ActivityButtons[count].SendMessage("setActivity", temp[n]);
                count++;
            }

        }

        for (; count < ActivityButtons.Length; count++)
        {
            ActivityButtons[count].SendMessage("setEmpty");
        }

            return;
    }

    public void executeActivity(ActivityClass activity)
    {
        Manager.SendMessage("executeActivity", activity);
    }

    public GameManager getGameManagerReference()
    {
        return Manager;
    }

    public int getHouseMoney()
    {
        return Manager.houseStats.getMoney();
    }
}
