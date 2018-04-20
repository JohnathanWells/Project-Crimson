using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class cityClass{

    [System.Serializable]
    public struct changeOfStats
    {
        public DayClass date;
        public float inflation;
        public float crime;
        public float filth;
        public float chaos;

        public changeOfStats(int m, int d, float inf, float cr, float fl, float ch)
        {
            date = new DayClass(m, d);

            inflation = inf;
            crime = cr;
            filth = fl;
            chaos = ch;

            return;
        }
    }

    public string stateName = "Little Venice";
    public string countryName = "Republic of Aurum";

    public districtClass[] districts;
    Queue<changeOfStats> changes = new Queue<changeOfStats>();

    //These all go from 1 to 10
    public float currentInflation;
    public float currentCrime;
    //These all go from 0 to 10
    public float currentFilth;
    public float currentChaos;

    //TODO test these

    public cityClass()
    {
        stateName = "Generic";
        currentInflation = 1;
        currentCrime = 1;
        currentFilth = 0;
        currentChaos = 0;
    }

    public cityClass(TextAsset districtStatFile, TextAsset statChangesFile)
    {
        //I can change the initial conditions here
        currentInflation = 1;
        currentCrime = 1;
        currentFilth = 0;
        currentChaos = 0;

        string[] dStatsLines = districtStatFile.text.Split('\n');
        districts = new districtClass[dStatsLines.Length];
        string[] statCLines = statChangesFile.text.Split('\n');
        string[] tempValues;

        for (int n = 0; n < dStatsLines.Length; n++)
        {
            tempValues = dStatsLines[n].Split('\t');
            if (tempValues.Length > 4)
                districts[n] = new districtClass((n + 1), tempValues[0], float.Parse(tempValues[1]), float.Parse(tempValues[2]), float.Parse(tempValues[3]), float.Parse(tempValues[4]));
        }

        for (int a = 0; a < statCLines.Length; a++)
        {
            tempValues = statCLines[a].Split('\t');

            if (tempValues.Length > 5)
                changes.Enqueue(new changeOfStats(int.Parse(tempValues[0]), int.Parse(tempValues[1]), float.Parse(tempValues[2]), float.Parse(tempValues[3]), float.Parse(tempValues[4]), float.Parse(tempValues[5])));
        }

        //reorganizeListIntoStack(listChanges);
    }

    public void applyStatChanges(int currentMonth, int currentDay)
    {
        changeOfStats tempC;

        if (changes.Count > 0)
        {
            tempC = changes.Peek();

            while (changes.Count > 0 && currentMonth >= tempC.date.month && currentDay >= tempC.date.day)
            {
                tempC = changes.Peek();

                currentInflation = tempC.inflation;
                currentCrime = tempC.crime;
                currentFilth = tempC.filth;
                currentChaos = tempC.chaos;

                changes.Dequeue();
            }
        }
    }

    public void copyInto(cityClass from)
    {
        currentInflation = from.currentInflation;
        currentFilth = from.currentFilth;
        currentCrime = from.currentCrime;
        currentChaos = from.currentChaos;

        if (changes.Count > 0)
            changes.Clear();
        
        foreach (changeOfStats c in from.changes)
        {
            changes.Enqueue(c);
        }

        districts = new districtClass[from.districts.Length];
        for (int n = 0; n < from.districts.Length; n++)
            districts[n] = from.districts[n];

    }
}
