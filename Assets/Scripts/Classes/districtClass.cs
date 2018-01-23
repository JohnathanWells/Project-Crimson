using System.Collections;
using UnityEngine;

[System.Serializable]
public class districtClass{
    public enum sector { A, B, C, D, E, F };

    public string districtName;
    public sector Sector;

    //All go from 0% to 100%
    //The initial stats
    public float baseCrimeRateMorning;
    public float baseCrimeRateNoon;
    public float baseCrimeRateEvening;
    //The current stats
    public float currentCRMorning;
    public float currentCRNoon;
    public float currentCREvening;
    public float traffic;
    
    public districtClass(int sec, string name, float crM, float crN, float crE, float tr)
    {
        switch (sec)
        {
            case 1:
                {
                    Sector = sector.A;
                    break;
                }
            case 2:
                {
                    Sector = sector.B;
                    break;
                }
            case 3:
                {
                    Sector = sector.C;
                    break;
                }
            case 4:
                {
                    Sector = sector.D;
                    break;
                }
            case 5:
                {
                    Sector = sector.E;
                    break;
                }
            case 6:
                {
                    Sector = sector.F;
                    break;
                }
            default:
                {
                    Sector = sector.A;
                    break;
                }
        };

        districtName = name;
        baseCrimeRateMorning = crM;
        baseCrimeRateNoon = crN;
        baseCrimeRateEvening = crE;
        traffic = tr;
    }

    public void calculateCrimeRates(float cityCrime)
    {
        currentCRMorning = baseCrimeRateMorning * cityCrime;
        currentCRNoon = baseCrimeRateNoon * cityCrime;
        currentCREvening = baseCrimeRateEvening * cityCrime;
    }
}
