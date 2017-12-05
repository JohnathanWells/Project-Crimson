using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class savefileClass{

    public bool empty = false;
    cityClass savedCity;
    houseClass savedHouse;
    DayClass currentDay;
    List<ActivityClass> morningActivities;
    List<ActivityClass> noonActivities;
    List<ActivityClass> eveningActivities;
    FamilyMembers[] savedFamily;
    List<sicknessClass> illnesses;
    Queue<notificationClass> pendingNotifications;
    timeOfDay savedTime;
    int[] ItemPrices;
    List<ActivityClass> Stores;
    List<ObituaryClass> Obituaries = new List<ObituaryClass>();
    List<newsClass> newList = new List<newsClass>();

    public savefileClass()
    {
        empty = true;
    }

    public void saveData(cityClass city, houseClass house, DayClass day, timeOfDay time, FamilyMembers[] family, List<ActivityClass> mAc, List<ActivityClass> nAc, List<ActivityClass> eAc, List<sicknessClass> ill, Queue<notificationClass> notqueue, int[] prices, List<ActivityClass> stores)
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
        pendingNotifications = notqueue;
        empty = false;
        ItemPrices = prices;
        Stores = stores;


        SaveLoad.savedGame = this;
        SaveLoad.Save();
    }

    public void copyData(cityClass city, houseClass house, DayClass day, FamilyMembers[] family, List<ActivityClass> mAc, List<ActivityClass> nAc, List<ActivityClass> eAc, List<sicknessClass> ill, Queue<notificationClass> queue, int[] prices, List<ActivityClass> stores)
    {
        city.copyInto(savedCity);
        savedHouse.copyData(house);
        currentDay.copyTo(day);

        //Debug.Log((int)savedTime + " vs " + (int)savedTime);

        //Copying the family
        for (int n = 0; n < Constants.familySize; n++)
        {
            family[n] = savedFamily[n];
        }

        //Copying the activities
        if (mAc.Count > 0)
            mAc.Clear();

        for (int n = 0; n < morningActivities.Count; n++)
        {
            mAc.Add(morningActivities[n]);
        }

        if (nAc.Count > 0)
            nAc.Clear();

        for (int n = 0; n < noonActivities.Count; n++)
        {
            nAc.Add(noonActivities[n]);
        }

        if (eAc.Count > 0)
            eAc.Clear();

        for (int n = 0; n < eveningActivities.Count; n++)
        {
            eAc.Add(eveningActivities[n]);
        }

        if (ill.Count > 0)
            ill.Clear();
        
        for (int n = 0; n < illnesses.Count; n++)
        {
            ill.Add(illnesses[n]);
        }

        if (queue.Count > 0)
            queue.Clear();
        
        foreach (notificationClass n in pendingNotifications)
        {
            queue.Enqueue(n);
        }

        if (stores.Count > 0)
            stores.Clear();
        
        foreach(ActivityClass s in Stores)
        {
            stores.Add(s);
        }

        for (int n = 0; n < 4; n++)
        {
            prices[n] = ItemPrices[n];
        }
        //Debug.Log(tempFamily[0].firstName + "\n" + tempFamily[1].firstName);
    }

    public timeOfDay getSavedTime()
    {
        return savedTime;
    }

    public bool isDadDead()
    {
        return (savedFamily[0].dead || savedFamily[0].gone);
    }

    //Obituaries
    public void getObituaries(List<ObituaryClass> into)
    {
        if (Obituaries.Count > 0)
        {
            into.Clear();

            foreach (ObituaryClass d in Obituaries)
                into.Add(d);
        }
    }

    public void saveObituaries(List<ObituaryClass> list)
    {
        if (list.Count > 0)
        {
            Obituaries.Clear();

            foreach (ObituaryClass d in list)
                Obituaries.Add(d);
        }
    }

    public void saveNewObituaty(ObituaryClass ob)
    {
        Obituaries.Add(ob);
    }

    //News
    public void getNews(List<newsClass> into)
    {
        if (newList.Count > 0)
        {
            into.Clear();
            foreach (newsClass n in newList)
                into.Add(n);
        }
    }

    public void saveNews(List<newsClass> from)
    {
        if (newList.Count > 0)
        {
            newList.Clear();
            foreach (newsClass n in from)
                newList.Add(n);
        }
    }
}
