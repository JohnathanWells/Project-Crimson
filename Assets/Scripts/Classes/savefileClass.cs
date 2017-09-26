using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class savefileClass{

    cityClass savedCity;
    houseClass savedHouse;
    DayClass currentDay;
    List<ActivityClass> morningActivities;
    List<ActivityClass> noonActivities;
    List<ActivityClass> eveningActivities;
    FamilyMembers[] savedFamily;
    GameManager.timeOfDay savedTime;

    public void saveData(cityClass city, houseClass house, DayClass day, GameManager.timeOfDay time, FamilyMembers[] family, List<ActivityClass> mAc, List<ActivityClass> nAc, List<ActivityClass> eAc)
    {
        savedCity = city;
        savedHouse = house;
        currentDay = day;
        savedTime = time;
        savedFamily = family;
        morningActivities = mAc;
        noonActivities = nAc;
        eveningActivities = eAc;

        SaveLoad.savedGame = this;
        SaveLoad.Save();
    }

    public void loadData(cityClass city, houseClass house, DayClass day, GameManager.timeOfDay time, FamilyMembers[] family, List<ActivityClass> mAc, List<ActivityClass> nAc, List<ActivityClass> eAc)
    {
        SaveLoad.Load();
        savefileClass temp = SaveLoad.savedGame;

        city = temp.savedCity;
        house = temp.savedHouse;
        day = temp.currentDay;
        time = temp.savedTime;

        //Copying the family
        for (int n = 0; n < 4; n++)
        {
            family[n] = temp.savedFamily[n];
        }


        //Copying the activities
        for (int n = 0; n < temp.morningActivities.Count; n++)
        {
            mAc.Add(temp.morningActivities[n]);
        }

        for (int n = 0; n < temp.noonActivities.Count; n++)
        {
            nAc.Add(temp.noonActivities[n]);
        }

        for (int n = 0; n < temp.eveningActivities.Count; n++)
        {
            eAc.Add(temp.eveningActivities[n]);
        }

        //Debug.Log(tempFamily[0].firstName + "\n" + tempFamily[1].firstName);
    }

}
