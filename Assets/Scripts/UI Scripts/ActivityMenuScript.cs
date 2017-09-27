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
    private GameManager.timeOfDay time;
    private ActivityClass.category selectedCategory;

    [Header("Activities")]
    private ActivityClass[] mornAct;
    private ActivityClass[] noonAct;
    private ActivityClass[] evnAct;

	// Use this for initialization
	void Start () {
        //updateMenu();
	}

    public void updateMenu()
    {
        //for (int n = 0; n < categoryButtons.Length; n++ )
        //{
        //    if (categoryButtons[n].Category == ActivityClass.category.Work)
        //    {
        //        categoryButtons[n].SendMessage("turnOn");
        //        break;
        //    }
        //}

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

        if (time == GameManager.timeOfDay.Morning)
        {
            temp = mornAct;
        }
        else if (time == GameManager.timeOfDay.Afternoon)
        {
            temp = noonAct;
        }
        else /*if (time == GameManager.timeOfDay.Evening)*/
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
}
