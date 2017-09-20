using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cityClass{

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

    public districtClass[] districts = new districtClass[5];
    Queue<changeOfStats> changes = new Queue<changeOfStats>();

    //These all go from 1 to 10
    public float currentInflation;
    public float currentCrime;
    //These all go from 0 to 10
    public float currentFilth;
    public float currentChaos;

    //TODO test these

    public cityClass(TextAsset districtStatFile, TextAsset statChangesFile)
    {
        //I can change the initial conditions here
        currentInflation = 1;
        currentCrime = 1;
        currentFilth = 0;
        currentChaos = 0;

        string[] dStatsLines = districtStatFile.text.Split('\n');
        string[] statCLines = statChangesFile.text.Split('\n');
        string[] tempValues;

        for (int n = 0; n < 5; n++)
        {
            tempValues = dStatsLines[n].Split('\t');
            districts[n] = new districtClass((n + 1), tempValues[0], float.Parse(tempValues[1]), float.Parse(tempValues[2]), float.Parse(tempValues[3]), float.Parse(tempValues[4]));
        }

        for (int a = 0; a < statCLines.Length; a++)
        {
            tempValues = statCLines[a].Split('\t');
            changes.Enqueue(new changeOfStats(int.Parse(tempValues[0]), int.Parse(tempValues[1]), float.Parse(tempValues[2]), float.Parse(tempValues[3]), float.Parse(tempValues[4]), float.Parse(tempValues[5])));
        }

        //reorganizeListIntoStack(listChanges);
    }

    public void applyStatChanges(int currentMonth, int currentDay)
    {
        changeOfStats tempC = changes.Peek();

        if (currentMonth >= tempC.date.month && currentDay >= tempC.date.day)
        {
            currentInflation = tempC.inflation;
            currentCrime = tempC.crime;
            currentFilth = tempC.filth;
            currentChaos = tempC.chaos;

            changes.Dequeue();       
        }
    }
}
