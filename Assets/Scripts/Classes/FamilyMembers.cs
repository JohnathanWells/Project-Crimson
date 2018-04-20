using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class FamilyMembers
{
    int minHealthByDepression = 20; //constant variable for keepying the people from dying from depression

    public enum famMember {Dad, Mom, Son, Dau, unknown};
    public enum emotionalHealth { Healthy, Depressed, Unstable };
    public enum gender { him, her};
    public famMember member;
    public string firstName;
    public string lastName;
    public gender sex;
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
    public string deathCause = "Unknown";
    int daysLeftOfSickness = 0;
    int daysUnstable = 0;
    int sicknessHealthDrop;
    int sicknessMoraleDrop;


    int maxMorale;
    int maxHealth;
    int[] healthDecreaseRates = {20, 5, 2, 0};
    int[] healthIncreaseRates = { 0, 2, 5, 10 };
    int[] healthDropsAccordingToPsyche = { 0, -2, -4 };
    int[] moraleDropsAccordingToPsyche = { 0, -3, -5 };
    int[] medicineMoraleIncreaseAccordingToPsyche = { 3, 5, 8 };

    public FamilyMembers(famMember familyMember)
    {
        member = familyMember;

        if (familyMember == famMember.Dad || familyMember == famMember.Son)
            sex = gender.him;
        else
            sex = gender.her;

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
        if (!dead && !gone)
        {
            //food %= 4;

            if (health > meanHealth)
            {
                healthChange(-healthDecreaseRates[food]);
            }
            else if (health < meanHealth)
            {
                healthChange(healthIncreaseRates[food]);
            }

            if (health <= 0)
            {
                //Debug.Log(firstName + " dies from starvation.");
                dies();
                return 1;
            }
            else
            {
                healthChange(sicknessHealthDrop);

                if (health <= 0)
                {
                    Debug.Log(firstName + " dies from " + status.name + ".");
                    dies();
                    return 1;
                }
                else
                {
                    health = Mathf.Clamp(health + healthDropsAccordingToPsyche[(int)psyche], minHealthByDepression, maxHealth); //Health drops according to psyche, but we make sure it cant kill the member
                    return 0;
                }
            }
        }
        else
            return 0;
    }

    public void cureIllness()
    {
        daysLeftOfSickness = 0;
        sicknessHealthDrop = 0;
        sicknessMoraleDrop = 0;
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
                //medicineConsumed = -1;
                daysLeftOfSickness -= 2;
            }
            else
            {
                daysLeftOfSickness -= 1;
            }
            //Debug.Log("Days left of sickness " + daysLeftOfSickness);
            if (daysLeftOfSickness <= 0)
            {
                cureIllness();
                medicineConsumed = medicine;
                medicine = 0;
            }
            else
            {
                maxHealth = 75;
                sicknessHealthDrop = status.healthPerDay;
                sicknessMoraleDrop = status.moralePerDay;
            }

            return medicineConsumed;
        }
        else
            return 0;
    }

    public int moraleHealthDrop(bool servicesPaid, bool personalHygiene) //Takes care of health and morale drop due to famine, unpaid services, etc
    {
        int moraleDropQuantity = 0;

        //People morale drops if they dont have access to home services
        if (!servicesPaid)
            moraleDropQuantity += Constants.unpaidServicesMoraleDrop;
        
        //People morale drops if they cant shower properly
        if (!personalHygiene)
            moraleDropQuantity += Constants.unshowerMoraleDrop;

        //Psyche morale and health drop
        moraleDropQuantity += moraleDropsAccordingToPsyche[(int)psyche];

        //Morale increases if they are not sick and consume medicine
        if (status.ID == 0 && medicine > 0)
        {
            Debug.Log("Medicine increasing morale by " + medicineMoraleIncreaseAccordingToPsyche[(int)psyche]);
            //medicine--;
            moraleDropQuantity -= Mathf.RoundToInt(medicineMoraleIncreaseAccordingToPsyche[(int)psyche] * 1.25f * medicine);
            medicine = 0;
        }

        //Morale is affected by how many people have left the house
        moraleDropQuantity += missingLevel;

        moraleChange(-moraleDropQuantity);

        //Unstability events
        if (psyche == emotionalHealth.Unstable)
        {
            int randomnum = Random.Range(0, 100);

            if (randomnum < Constants.unstabilityEventsProbability[0])
            {
                if (randomnum < Constants.unstabilityEventsProbability[1])
                {
                    if (randomnum < Constants.unstabilityEventsProbability[2])
                    {
                        if (randomnum < Constants.unstabilityEventsProbability[3])
                        {
                            if (member == famMember.Mom || member == famMember.Dad)
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
        //Debug.Log(firstName + " dies");
        dead = true;
        return 1;
    }

    public int leavesTheHouse()
    {
        //Debug.Log(firstName + " leaves");
        gone = true;
        return 1;
    }

    public int comesBack()
    {
        gone = false;
        return -1;
    }
    
    public void moraleChange(int change)
    {
        morale = Mathf.Clamp(morale + change, 0, maxMorale);

        if (morale <= Constants.depressionTop && status.ID == 0)
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