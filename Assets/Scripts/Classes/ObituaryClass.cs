using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObituaryClass{
    public DayClass dateOfDeath;
    public string firstName;
    public string lastName;
    public string DeathCause;

    public ObituaryClass(string fn, string ln, string dc, DayClass day)
    {
        firstName = fn;
        lastName = ln;
        DeathCause = dc;
        dateOfDeath = day;
    }
}
