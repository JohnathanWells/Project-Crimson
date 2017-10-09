using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class FamilyMembers
{
    public enum famMember {Dad, Mom, Son, Dau, unknown};
    public enum emotionalHealth { Healthy, Depressed, Unstable };
    public famMember member;
    public string firstName;
    public string lastName;
    public int morale;
    public int health;
    public int meanHealth;
    public int medicine;
    public int food;
    public sicknessClass status;
    public emotionalHealth psyche;
    public int mourningLevel;
    public bool gone;

    int maxMorale = 100;
    int[] healthDecreaseRates = {20, 5, 2, 0};
    int[] healthIncreaseRates = { 0, 2, 5, 10 };

    public FamilyMembers(famMember familyMember)
    {
        member = familyMember;
        morale = 100;
        health = 100;
        medicine = 0;
        food = 3;
        mourningLevel = 0;
        gone = false;
        status = new sicknessClass();
        psyche = emotionalHealth.Healthy;
        meanHealth = 100;
    }

    public void setName(string newName, string newLName)
    {
        firstName = newName;
        lastName = newLName;
    }

    public int healthDrift()
    {
        if (!gone)
        {
            //food %= 4;

            if (health > meanHealth)
            {
                health -= healthDecreaseRates[food];
            }
            else if (health < meanHealth)
            {
                health += healthIncreaseRates[food];
            }

            Mathf.Clamp(health, 0, 100);

            if (health <= 0)
            {
                dies();
                return 1;
            }
            else
                return 0;
        }
        else
            return 0;
    }

    public int dies()
    {
        gone = true;
        return 1;
    }
    
    public void moraleChange(int change)
    {
        morale += change;
        Mathf.Clamp(morale, 0, maxMorale);
    }

    public void mourningAdd(int levels)
    {
        mourningLevel += levels;

        switch (mourningLevel)
        {
            case 0:
                maxMorale = 100;
                break;
            case 1:
                maxMorale = 80;
                break;
            case 2:
                maxMorale = 60;
                break;
            default:
                maxMorale = 40;
                break;
        }
    }
}