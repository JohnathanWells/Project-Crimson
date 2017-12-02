using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseMenuScript : MonoBehaviour {

    [Header("Reference To Main House Script")]
    public HouseScript mainHouseScript;

    [Header("Reference To Icons - Page 1")]
    public TextMesh familyNameDisplay;
    public TextMesh moneyDisplay;
    public TextMesh serviceStatusDisplay;
    public activityButtonScript servicePayActivityButton;
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
    public Transform[] medicationButton = new Transform[4];
    public Transform[] foodButton = new Transform[4];
    public TextMesh[] moraleStatus = new TextMesh[4];
    public Color[] moraleColors = new Color[5];

    [Header("Family Information")]
    FamilyMembers [] FamilyMembers;

    [Header("Tabs")]
    //Showing Window pages
    public Transform[] tabs;
    public Transform[] tabKeepers;
    int currentTab = -1;

    public void Start()
    {
        healthColors = mainHouseScript.healthColors;
        moraleColors = mainHouseScript.moraleColors;
    }

    //Menu Open
    public void openMenu(FamilyMembers [] members)
    {
        FamilyMembers = members;
        updateFamilyInfo();

        updateTab(0);
    }
    
    //Updates
    public void updateHouseInfo(GameManager manager)
    {
        houseClass yourHouse = manager.houseStats;
        DayClass currentDay = manager.currentDay;
        cityClass city = manager.City;

        familyNameDisplay.text = "The " + FamilyMembers[0].lastName + " House";
        moneyDisplay.text = "MONEY: $" + yourHouse.getMoney();

        if (yourHouse.servicesPaid)
        {
            serviceStatusDisplay.text = "PAID\n(Next Bill in " + yourHouse.timeLeftForPayment + " Days)";
            serviceStatusDisplay.color = goodColor;

            servicePayActivityButton.gameObject.SetActive(false);
        }
        else
        {
            serviceStatusDisplay.text = "NOT PAID\n(Next Bill in " + yourHouse.timeLeftForPayment + " Days)";
            serviceStatusDisplay.color = badColor;

            servicePayActivityButton.gameObject.SetActive(true);
            servicePayActivityButton.SendMessage("setActivity", manager.servicePayActivity);
            servicePayActivityButton.SendMessage("updateActivityStatus", manager.currentTime);
        }

        foodQuantity.text = "" + yourHouse.getFoodQ() + " Kg.";

        if (yourHouse.getFoodQ() == 0)
            foodQuantity.color = badColor;
        else
            foodQuantity.color = new Color(1, 1, 1, 1);

        medicineQuantity.text = "" + yourHouse.getMedicines() + " Mg.";

        if (yourHouse.getMedicines() == 0)
            medicineQuantity.color = badColor;
        else
            medicineQuantity.color = new Color(1, 1, 1, 1);

        hygieneQuantity.text = "" + yourHouse.getHygiene() + " items.";

        if (yourHouse.getHygiene() == 0)
            hygieneQuantity.color = badColor;
        else
            hygieneQuantity.color = new Color(1, 1, 1, 1);
        
        cleanQuantity.text = "" + yourHouse.getCleaningQ() + " Liters.";

        if (yourHouse.getCleaningQ() == 0)
            cleanQuantity.color = badColor;
        else
            cleanQuantity.color = new Color(1, 1, 1, 1);



        //plagueRateDisplay.text = "PLAGUE RATE: " + yourHouse.getPlagueRate(city) + "%";
    }

    public void updateFamilyInfo()
    {
        for (int n = 0; n < 4; n++)
        {
            updateFamilyMember(n);
        }
    }

    public void updateFamilyMember(int number)
    {
        int tempInt;

        if (!FamilyMembers[number].dead && !FamilyMembers[number].gone)
        {
            familyIcons[number].gameObject.SetActive(true);
            familyNames[number].text = FamilyMembers[number].firstName;

            if (FamilyMembers[number].status.ID != 0)
            {
                sickStatuses[number].gameObject.SetActive(true);
                
                if (sickStatuses[number].gameObject.activeInHierarchy)
                    sickStatuses[number].gameObject.SendMessage("setSickness", FamilyMembers[number].status);
            }
            else
            {
                sickStatuses[number].gameObject.SetActive(false);
            }

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

            ////Take these out later
            //healthStatus[number].text = "" + FamilyMembers[number].health;
            //moraleStatus[number].text = "" + FamilyMembers[number].morale;

            updateFamilyMemberInv(number);
        }
        else
        {
            familyIcons[number].gameObject.SetActive(false);
        }
        
    }

    public void updateFamilyMemberInv(int number)
    {
        if (foodButton[number].gameObject.activeInHierarchy && medicationButton[number].gameObject.activeInHierarchy)
        {
            medicationButton[number].SendMessage("updateNumber", FamilyMembers[number].medicine);
            foodButton[number].SendMessage("updateNumber", FamilyMembers[number].food);
        }
    }

    //These functions deal with changing tabs
    public void changeTab(int direction)
    {
        currentTab = (currentTab + direction) % tabs.Length;

        Debug.Log("Tab changed");
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

            updateFamilyInfo();
        }
    }

}
