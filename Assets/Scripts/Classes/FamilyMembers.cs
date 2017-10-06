using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class FamilyMembers
{
    public enum famMember {Dad, Mom, Son, Dau, unknown};
    public enum sickness { Healthy, Flu, Porcine, Dengue, Cholera, Rabies }
    public enum emotionalHealth { Healthy, Depressed, Unstable };
    public famMember member;
    public string firstName;
    public string lastName;
    public int morale;
    public int health;
    public int meanHealth;
    public int medicine;
    public int food;
    public sickness status;
    public emotionalHealth psyche;
    public bool mourning;
    public bool gone;

    int[] healthDecreaseRates = {20, 5, 2, 0};
    int[] healthIncreaseRates = { 0, 2, 5, 10 };

    public FamilyMembers(famMember member)
    {
        member = famMember.unknown;
        morale = 100;
        health = 100;
        medicine = 0;
        food = 3;
        mourning = false;
        gone = false;
        status = sickness.Healthy;
        psyche = emotionalHealth.Healthy;
        meanHealth = 100;
    }

    public void setName(string newName, string newLName)
    {
        firstName = newName;
        lastName = newLName;
    }

    public void healthDrift()
    {
        food %= 3;
        
        if (health > meanHealth)
        {
            health -= healthDecreaseRates[food];
        }
        else if (health < meanHealth)
        {
            health += healthIncreaseRates[food];
        }

        if (health <= 0)
        {
            dies();
        }
    }

    public void dies()
    {
        gone = true;
    }
    //WIP
}