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
    public int missingLevel;
    public bool dead;
    public bool gone;
    int daysLeftOfSickness = 0;
    int daysUnstable = 0;

    int maxMorale;
    int maxHealth;
    int[] healthDecreaseRates = {20, 5, 2, 0};
    int[] healthIncreaseRates = { 0, 2, 5, 10 };
    int[] healthDropsAccordingToPsyche = { 0, -1, -1 };
    int[] moraleDropsAccordingToPsyche = { 0, -1, -1 };

    public FamilyMembers(famMember familyMember)
    {
        member = familyMember;
        morale = 100;
        health = 100;
        medicine = 0;
        food = 3;
        mourningLevel = 0;
        missingLevel = 0;
        dead = false;
        status = new sicknessClass();
        psyche = emotionalHealth.Healthy;
        meanHealth = 100;
        maxMorale = 100;
        maxHealth = 100;
        daysLeftOfSickness = 0;
    }

    public void setName(string newName, string newLName)
    {
        firstName = newName;
        lastName = newLName;
    }

    public int healthDrift()
    {
        if (!dead)
        {
            //food %= 4;

            if (health > meanHealth)
            {
                health = Mathf.Clamp(health - healthDecreaseRates[food], 0, maxHealth);
            }
            else if (health < meanHealth)
            {
                health = Mathf.Clamp(health + healthIncreaseRates[food], 0, maxHealth);
            }

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

    public void cureIllness()
    {
        daysLeftOfSickness = 0;
        status = new sicknessClass();
    }

    public void getsSick(sicknessClass illness)
    {
        status = illness;
        daysLeftOfSickness = illness.recoveryTimeN;
    }

    public int sickDayPasses() //Takes care of health and morale drop for sickness as well as medicine consumption
    {
        int medicineConsumed = 0;
        
        if (status.ID != 0)
        {
            if (medicine > 0)
            {
                medicine--;
                medicineConsumed = -1;
                daysLeftOfSickness -= 2;
            }
            else
            {
                daysLeftOfSickness -= 1;
            }

            if (daysLeftOfSickness <= 0)
            {
                cureIllness();
                medicineConsumed = medicine;
                medicine = 0;
            }
            else
            {
                maxHealth = 75;
                healthChange(status.healthPerDay);
                moraleChange(status.moralePerDay);
            }

            return medicineConsumed;
        }
        else
            return 0;
    }

    public int moraleHealthDrop(bool servicesPaid, bool personalHygiene) //Takes care of health and morale drop due to famine, unpaid services, etc
    {
        int moraleDropQuantity = 0;

        if (!servicesPaid)
            moraleDropQuantity += 2;

        if (!personalHygiene)
            moraleDropQuantity += 2;

        healthChange(healthDropsAccordingToPsyche[(int)psyche]);
        moraleChange(moraleDropsAccordingToPsyche[(int)psyche]);

        moraleChange(-missingLevel);

        if (psyche == emotionalHealth.Unstable)
        {
            int randomnum = Random.Range(0, 100);

            if (randomnum < 50)
            {
                if (randomnum < 25)
                {
                    if (randomnum < 10)
                    {
                        if (randomnum < 5)
                        {
                            if (member == famMember.Mom)
                            {
                                return 4;
                            }
                            else
                            {
                                return 5;
                            }
                        }
                        else
                        {
                            return 3;
                        }
                    }
                    else
                    {
                        leavesTheHouse();
                        return 2;
                    }
                }
                else
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
        dead = true;
        return 1;
    }

    public int leavesTheHouse()
    {
        gone = true;
        return 1;
    }
    
    public void moraleChange(int change)
    {
        morale = Mathf.Clamp(morale + change, 0, maxMorale);

        if (morale <= 10 && status.ID == 0)
            psyche = emotionalHealth.Unstable;
        else if (morale <= 25)
            psyche = emotionalHealth.Depressed;
        else
            psyche = emotionalHealth.Healthy;
    }

    public void healthChange(int change)
    {
        health = Mathf.Clamp(health + change, 0, maxHealth);
    }

    public void mourningAdd(int levels)
    {
        mourningLevel += levels;
        missingLevel += levels;

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

    public void missingAdd(int levels)
    {
        missingLevel += levels;
    }
}