using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseMenuScript : MonoBehaviour {

    [Header("Reference To Icons - Page 1")]
    public TextMesh familyNameDisplay;
    public TextMesh moneyDisplay;
    public TextMesh serviceStatusDisplay;
    public Color goodColor = new Color(0, 1, 0, 1);
    public Color badColor = new Color(1, 0, 0, 1);
    public TextMesh plagueRateDisplay;
    public TextMesh foodQuantity;
    public TextMesh medicineQuantity;
    public TextMesh hygieneQuantity;
    public TextMesh cleanQuantity;

    [Header("Reference To Icons - Page 2")]
    public Transform[] familyIcons = new Transform[4];
    public Transform[] sickStatuses = new Transform[4];
    public Transform[] unstableStatuses = new Transform[4];
    public TextMesh[] familyNames = new TextMesh[4];
    public TextMesh[] healthStatus = new TextMesh[4];
    public Color[] healthColors = new Color[5];
    public TextMesh[] medicationNumber = new TextMesh[4];
    public TextMesh[] foodNumber = new TextMesh[4];
    public TextMesh[] moraleStatus = new TextMesh[4];
    public Color[] moraleColors = new Color[5];

    [Header("Family Information")]
    FamilyMembers [] FamilyMembers;

    [Header("Tabs")]
    //Showing Window pages
    public Transform[] tabs;
    public Transform[] tabKeepers;
    int currentTab = -1;


    public void openMenu(FamilyMembers [] members)
    {
        FamilyMembers = members;
        updateFamilyInfo(members);

        updateTab(0);
    }

    public void updateHouseInfo(GameManager manager)
    {
        houseClass yourHouse = manager.houseStats;
        DayClass currentDay = manager.currentDay;
        cityClass city = manager.City;

        familyNameDisplay.text = "The " + FamilyMembers[0].lastName + " House";
        moneyDisplay.text = "MONEY: $" + yourHouse.getMoney();

        if (yourHouse.servicesPaid)
        {
            serviceStatusDisplay.text = "PAID\n(Next Bill in " + calculateDaysLeftForServices(currentDay) + " Days)";
            serviceStatusDisplay.color = goodColor;
        }
        else
        {
            serviceStatusDisplay.text = "NOT PAID";
            serviceStatusDisplay.color = badColor;
        }

        foodQuantity.text = "" + yourHouse.getFoodQ() + " Kg.";
        medicineQuantity.text = "" + yourHouse.getMedicines() + " Mg.";
        hygieneQuantity.text = "" + yourHouse.getHygiene() + " items.";
        cleanQuantity.text = "" + yourHouse.getCleaningQ() + " Liters.";

        //plagueRateDisplay.text = "PLAGUE RATE: " + yourHouse.getPlagueRate(city) + "%";
    }

    public int calculateDaysLeftForServices(DayClass currentDay)
    {
        int finalDay = 31;

        if (currentDay.month == 10 || currentDay.month == 12 || currentDay.month == 1 || currentDay.month == 3)
        {
            finalDay = 31;
        }
        else if (currentDay.month == 11)
        {
            finalDay = 30;
        }
        else if (currentDay.month == 2)
        {
            finalDay = 28;
        }

        int daysLeft = finalDay - currentDay.day;

        while ((currentDay.dayCount + daysLeft ) % 7 != 0)
        {
            daysLeft++;
        }

        return daysLeft;
        
    }

    public void updateFamilyInfo(FamilyMembers[] members)
    {
        for (int n = 0; n < 4; n++)
        {
            updateFamilyMember(n);
        }
    }

    public void updateFamilyMember(int number)
    {
        int tempInt;

        if (!FamilyMembers[number].gone)
        {
            familyIcons[number].gameObject.SetActive(true);
            familyNames[number].text = FamilyMembers[number].firstName;
            sickStatuses[number].gameObject.SetActive(FamilyMembers[number].status != global::FamilyMembers.sickness.Healthy);
            unstableStatuses[number].gameObject.SetActive(FamilyMembers[number].psyche == global::FamilyMembers.emotionalHealth.Unstable);

            #region healthStatus
            tempInt = FamilyMembers[number].health;

            if (tempInt > 75 && tempInt <= 100)
            {
                healthStatus[number].text = "Excelent";
                healthStatus[number].color = healthColors[4];
            }
            else if (tempInt > 50 && tempInt <= 75)
            {
                healthStatus[number].text = "Good";
                healthStatus[number].color = healthColors[3];
            }
            else if (tempInt > 25 && tempInt <= 50)
            {
                healthStatus[number].text = "Poor";
                healthStatus[number].color = healthColors[2];
            }
            else if (tempInt > 10 && tempInt <= 25)
            {
                healthStatus[number].text = "Weak";
                healthStatus[number].color = healthColors[1];
            }
            else if (tempInt <= 10)
            {
                healthStatus[number].text = "Dying";
                healthStatus[number].color = healthColors[0];
            }
            else
            {
                healthStatus[number].text = "ERROR";
                healthStatus[number].color = healthColors[0];
            }
            #endregion

            #region moraleStatus
            tempInt = FamilyMembers[number].morale;

            if (tempInt > 75 && tempInt <= 100)
            {
                moraleStatus[number].text = "Joyful";
                moraleStatus[number].color = moraleColors[4];
            }
            else if (tempInt > 50 && tempInt <= 75)
            {
                moraleStatus[number].text = "Happy";
                moraleStatus[number].color = moraleColors[3];
            }
            else if (tempInt > 25 && tempInt <= 50)
            {
                moraleStatus[number].text = "OK";
                moraleStatus[number].color = moraleColors[2];
            }
            else if (tempInt > 10 && tempInt <= 25)
            {
                moraleStatus[number].text = "Depressed";
                moraleStatus[number].color = moraleColors[1];
            }
            else if (tempInt <= 10)
            {
                moraleStatus[number].text = "Unstable";
                moraleStatus[number].color = moraleColors[0];
            }
            else
            {
                moraleStatus[number].text = "ERROR";
                moraleStatus[number].color = moraleColors[0];
            }
            #endregion

            medicationNumber[number].text = "" + FamilyMembers[number].medicine;
            foodNumber[number].text = "" + FamilyMembers[number].food;

        }
        
    }

    //These functions deal with changing tabs
    public void changeTab(int direction)
    {
        currentTab = (currentTab + direction) % tabs.Length;
    }

    public void updateTab(int tabNum)
    {
        if (tabNum != currentTab)
        {
            currentTab = tabNum;

            for (int n = 0; n < tabs.Length; n++)
            {
                if (n == tabNum)
                {
                    tabs[n].gameObject.SetActive(true);
                    tabKeepers[n].SendMessage("setTabActive", true);
                }
                else
                {
                    tabs[n].gameObject.SetActive(false);
                    tabKeepers[n].SendMessage("setTabActive", false);
                }
            }
        }
    }

}
