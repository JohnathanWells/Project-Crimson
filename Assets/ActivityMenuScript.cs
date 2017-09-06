using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityMenuScript : MonoBehaviour {

    public TextMesh[] ActivityButtons;
    public CategoryScript[] categoryButtons;
    public GameManager Manager;
    private DayClass currentDay;
    private GameManager.timeOfDay time;
    private ActivityClass.category selectedCategory;

	// Use this for initialization
	void Start () {
        //updateMenu();
	}

    public void updateMenu()
    {
        for (int n = 0; n < categoryButtons.Length; n++ )
        {
            if (categoryButtons[n].Category == ActivityClass.category.Work)
            {
                categoryButtons[n].SendMessage("turnOn");
                break;
            }
        }
        currentDay = Manager.currentDay;
        time = Manager.currentTime;
        changeActCat(ActivityClass.category.Work);
    }

    public void changeActCat(ActivityClass.category Category)
    {
        ActivityClass[] temp;
        selectedCategory = Category;
        int count = 0;

        Debug.Log(time);

        if (time == GameManager.timeOfDay.Morning)
        {
            temp = currentDay.MorningActivities;
        }
        else if (time == GameManager.timeOfDay.Afternoon)
        {
            temp = currentDay.AfternoonActivities;
        }
        else /*if (time == GameManager.timeOfDay.Evening)*/
        {
            temp = currentDay.EveningActivities;
        }

        for (int n = 0; n < temp.Length; n++)
        {
            if (temp[n].activityCategory == selectedCategory)
            {
                ActivityButtons[count].text = temp[n].activityName;
                count++;
            }
        }

        for (; count < ActivityButtons.Length; count++)
        {
            ActivityButtons[count].text = " ";
        }

            return;
    }
}
