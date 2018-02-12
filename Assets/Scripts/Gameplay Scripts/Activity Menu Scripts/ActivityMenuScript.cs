using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityMenuScript : MonoBehaviour {

    [Header("Buttons")]
    public Transform[] ActivityButtons;
    public CategoryScript[] categoryButtons;

    [Header("Sounds")]
    public AudioClip highlightSound;
    public AudioClip executeSound;
    public AudioClip failedExecuteSound;
    public AudioClip categoryChangeSound;

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

    [Header("Pointers")]
    public Transform [] MapPointers;
    public Transform cityMap;
    public Transform blockingFilm;

    void Awake()
    {
        for (int n = 0; n < Mathf.Min(MapPointers.Length, ActivityButtons.Length); n++)
        {
            ActivityButtons[n].GetComponent<activityButtonScript>().correspondingPointer = MapPointers[n];
        }
    }

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

        Manager.audioManager.SendMessage("playSFX", categoryChangeSound);

        toggleFilm(false);

        categoryButtons[(int)Category].SendMessage("turnOn");

        foreach (Transform t in MapPointers)
            t.gameObject.SetActive(true);

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

                //Statements regarding the pointers
                MapPointers[count].SendMessage("setActivity", temp[n]);
                MapPointers[count].position = cityMap.transform.TransformPoint(new Vector2( temp[n].locationOnMap[0], temp[n].locationOnMap[1]));

                count++;
            }

        }

        for (; count < ActivityButtons.Length; count++)
        {
            ActivityButtons[count].SendMessage("setEmpty");
            MapPointers[count].gameObject.SetActive(false);
        }

            return;
    }

    public void executeActivity(ActivityClass activity)
    {
        if (!Manager.currentlyExecutingActivity)
        {
            Manager.SendMessage("executeActivity", activity);
            Manager.audioManager.SendMessage("playSFX", executeSound);
        }
        else
        {
            Manager.audioManager.SendMessage("playSFX", failedExecuteSound);
        }
    }

    public void playHighlightSound()
    {
        if (!Manager.currentlyExecutingActivity)
            Manager.audioManager.SendMessage("playSFX", highlightSound);
    }

    public GameManager getGameManagerReference()
    {
        return Manager;
    }

    public int getHouseMoney()
    {
        return Manager.houseStats.getMoney();
    }

    public void hideAllPointers()
    {
        foreach (Transform t in MapPointers)
            t.gameObject.SetActive(false);
    }

    public void toggleFilm(bool enabled)
    {
        blockingFilm.gameObject.SetActive(enabled);
    }
}
