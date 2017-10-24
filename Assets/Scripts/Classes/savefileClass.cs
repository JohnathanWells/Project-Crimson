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
    List<sicknessClass> illnesses;
    timeOfDay savedTime;

    public void saveData(cityClass city, houseClass house, DayClass day, timeOfDay time, FamilyMembers[] family, List<ActivityClass> mAc, List<ActivityClass> nAc, List<ActivityClass> eAc, List<sicknessClass> ill)
    {
        savedCity = city;
        savedHouse = house;
        currentDay = day;
        savedTime = time;
        savedFamily = family;
        morningActivities = mAc;
        noonActivities = nAc;
        eveningActivities = eAc;
        illnesses = ill;

        SaveLoad.savedGame = this;
        SaveLoad.Save();
    }

    public void copyData(cityClass city, houseClass house, DayClass day, FamilyMembers[] family, List<ActivityClass> mAc, List<ActivityClass> nAc, List<ActivityClass> eAc, List<sicknessClass> ill)
    {
        city = savedCity;
        savedHouse.copyData(house);
        currentDay.copyTo(day);

        //Debug.Log((int)savedTime + " vs " + (int)savedTime);

        //Copying the family
        for (int n = 0; n < 4; n++)
        {
            family[n] = savedFamily[n];
        }

        //Copying the activities
        for (int n = 0; n < morningActivities.Count; n++)
        {
            mAc.Add(morningActivities[n]);
        }

        for (int n = 0; n < noonActivities.Count; n++)
        {
            nAc.Add(noonActivities[n]);
        }

        for (int n = 0; n < eveningActivities.Count; n++)
        {
            eAc.Add(eveningActivities[n]);
        }

        for (int n = 0; n < illnesses.Count; n++)
        {
            ill.Add(illnesses[n]);
        }
        //Debug.Log(tempFamily[0].firstName + "\n" + tempFamily[1].firstName);
    }

    public timeOfDay getSavedTime()
    {
        return savedTime;
    }
}
