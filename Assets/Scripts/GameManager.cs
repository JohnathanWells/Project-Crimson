using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum timeOfDay { Morning, Afternoon, Evening };

public class GameManager : MonoBehaviour {


    [Header("References")]
    public phoneScript Phone;
    public HouseScript House;
    public ActivityMenuScript Activities;

    [Header("Data to Pass")]
    public FamilyMembers[] Family = new FamilyMembers[4];
    public houseClass houseStats;
    public cityClass City;
    public bool houseNotification = false;
    public bool phoneNotification = false;
    public int costOfServices = 100;
    int membersAlive = 4;

    [Header("Files")]
    public TextAsset activityFile;
    public TextAsset statChangesFile;
    public TextAsset districtStatsFile;
    public TextAsset sicknessFile;
    
    [Header("Day Stuff")]
    public DayClass currentDay;
    public timeOfDay currentTime;
    int daysSinceLastIllnessCheck = 0;
    int[] daysLeftForHealing = { 0, 0, 0, 0 };

    [Header("Permanent Stuff")]
    //public DayClass[] sessionCalendar;
    public bool createSessionAtStart = false;
    public List<ActivityClass> mornActivities = new List<ActivityClass>();
    public List<ActivityClass> noonActivities = new List<ActivityClass>();
    public List<ActivityClass> evenActivities = new List<ActivityClass>();
    public List<sicknessClass> sicknesses = new List<sicknessClass>();
    savefileClass savedObject = new savefileClass();


    void Start()
    {
        if (createSessionAtStart)
            newSession();
        else
            loadData();

        updateComponents();
    }

    //Time stuff
    public void transitionTime()
    {
        if (currentTime == timeOfDay.Evening)
        {
            dayAdvance();
        }
        else
        {
            currentTime += 1;
            saveData();
        }

        updateComponents();
    }

    public void dayAdvance()
    {
        currentDay.advanceDay();
        houseStats.timeLeftForPayment--;
        currentTime = timeOfDay.Morning;
        daysSinceLastIllnessCheck++;

        //House services are paid
        if (houseStats.timeLeftForPayment <= 0)
        {
            houseStats.payServices(costOfServices, currentDay);
        }

        //House Stats and Item Consumption are calculated
        houseStats.modCleaning(-1);

        if (currentDay.dayCount % 7 == 6)
        {
            houseStats.modHygiene(-1 * membersAlive);
        }
        houseStats.calculatePlagueRate(City);

        //Check if a member becomes sick
        if (daysSinceLastIllnessCheck >= 7 && Random.Range(1, 2) == 1)
        {
            daysSinceLastIllnessCheck = 0;

            if (houseStats.plagueRate > 0)
            {
                illnessChecker();
            }
        }

        //Family members consume food and medicines
        dailyFoodConsumption();
        dailyHealthDrift();

        //Morale is affected
        dailyMoraleAndHealthDrop();
    }

    public void updateComponents()
    {
        Phone.SendMessage("updatePhone");
        House.SendMessage("closeHouseMenu");
        House.SendMessage("familyUpdate", Family);
        Activities.SendMessage("updateMenu");
    }

    //Family effects
    public void illnessChecker()
    {
        int worseAvailableSickness = 0;
        sicknessClass[] illnesses = sicknesses.ToArray();
        float currentPlagueRate = houseStats.plagueRate;
        int chosenSickness;
        int chosenFamilyMember = Random.Range(0, 3);
        int tempH = Family[chosenFamilyMember].health;

        if (Random.Range(1, 100) > (tempH * (2 / 3)) && Random.Range(1, 100) <= currentPlagueRate)
        {
            for (int n = 0; n < sicknesses.Count;  n++)
            {
                if (illnesses[n].minPRCheck <= currentPlagueRate)
                {
                    worseAvailableSickness = n;
                }
                else
                {
                    break;
                }
            }

            if (worseAvailableSickness > 0)
            {
                chosenSickness = Random.Range(1, worseAvailableSickness);
                Family[chosenFamilyMember].status = illnesses[chosenSickness];
            }
        }
    }

    //Family stats changes with time
    public void dailyFoodConsumption()
    {
        int foodConsumed = 0;

        for (int n = 3; n >= 0; n--)
        {
            if (!Family[n].gone)
            {
                foodConsumed = Family[n].food;

                if (houseStats.getFoodQ() > 0)
                {
                    if (houseStats.getFoodQ() - foodConsumed > 0)
                    {
                        houseStats.modFood(-foodConsumed);

                        if (foodConsumed == 3)
                        {
                            Family[n].meanHealth = 100;
                        }
                        else if (foodConsumed == 2)
                        {
                            Family[n].meanHealth = 66;
                        }
                        else if (foodConsumed == 1)
                        {
                            Family[n].meanHealth = 33;
                        }
                        else if (foodConsumed == 0)
                        {
                            Family[n].meanHealth = 0;
                        }
                    }
                    else
                    {
                        Family[n].food = houseStats.getFoodQ();

                        foodConsumed = Family[n].food;

                        houseStats.modFood(-foodConsumed);

                        if (foodConsumed == 3)
                        {
                            Family[n].meanHealth = 100;
                        }
                        else if (foodConsumed == 2)
                        {
                            Family[n].meanHealth = 66;
                        }
                        else if (foodConsumed == 1)
                        {
                            Family[n].meanHealth = 33;
                        }
                        else if (foodConsumed == 0)
                        {
                            Family[n].meanHealth = 0;
                        }
                    }
                }
                else
                {
                    Family[n].food = 0;
                    foodConsumed = 0;
                    Family[n].meanHealth = 0;
                }

            }
        }
    }

    public void dailyHealthDrift()
    {
        int mourningLevel = 0;

        for (int n = 0; n < 4; n++)
        {
            mourningLevel += Family[n].healthDrift();
        }

        if (mourningLevel > 0)
        {
            membersAlive -= mourningLevel;
            for (int i = 0; i < 4; i++)
            {
                Family[i].mourningAdd(mourningLevel);
            }
        }
    }

    public void dailyMedicineConsumption()
    {

    }

    public void dailyMoraleAndHealthDrop()
    {

    }

    //Activity and stuff
    public void executeActivity(ActivityClass activity)
    {
        if (!activity.isItShop)
        {
            houseStats.addMoney(-activity.cost);

            for (int n = 0; n < 4; n++)
            {
                Family[n].moraleChange(activity.moraleChange[n]);
            }
        }

        transitionTime();
    }

    //Data and saving management stuff
    void newSession()
    {
        City = new cityClass(districtStatsFile, statChangesFile);
        houseStats = new houseClass();
        currentDay = new DayClass(10, 1);
        currentTime = timeOfDay.Morning;

        readActivities();

        readSicknesses();

        Family[0] = new FamilyMembers(FamilyMembers.famMember.Dad);
        Family[0].setName("Pedro", "Perez");
        Family[1] = new FamilyMembers(FamilyMembers.famMember.Mom);
        Family[1].setName("Maria", "Perez");
        Family[2] = new FamilyMembers(FamilyMembers.famMember.Son);
        Family[2].setName("Jose", "Perez");
        Family[3] = new FamilyMembers(FamilyMembers.famMember.Dau);
        Family[3].setName("Juana", "Perez");

        houseStats.payServices(0, currentDay);

        saveData();
    }

    public void readSicknesses()
    {
        #region sicknessRelated
        string[] lines = sicknessFile.text.Split('\n');
        string[] numbers;
        sicknessClass tempS = new sicknessClass(0, "Healthy", 0, 0, 0, 0, 0);

        sicknesses.Add(tempS);

        for (int n = 0; n < lines.Length; n++)
        {
            numbers = lines[n].Split('\t');

            tempS = new sicknessClass(n + 1, numbers[0], int.Parse(numbers[1]), int.Parse(numbers[2]), int.Parse(numbers[3]), int.Parse(numbers[4]), float.Parse(numbers[5]));

            sicknesses.Add(tempS);
        }

        #endregion
    }

    public void readActivities()
    {
        #region activityRelated
        ActivityClass tempActivity;
        ActivityClass.sector tempSec;
        ActivityClass.category tempCat;
        string activityName;
        int tempCost;
        int[] tempMorale = new int[4];
        bool[] tempAva = new bool[3];
        bool tempShop;
        string[] lines = activityFile.text.Split('\n');
        string[] numbers;
        int tempNum;

        for (int n = 0; n < lines.Length; n++)
        {
            numbers = lines[n].Split('\t');

            activityName = numbers[0];

            #region sectorChoice(Number[1])

            tempNum = int.Parse(numbers[1]);

            if (tempNum == 1)
            {
                tempSec = ActivityClass.sector.A;
            }
            else if (tempNum == 2)
            {
                tempSec = ActivityClass.sector.B;
            }
            else if (tempNum == 3)
            {
                tempSec = ActivityClass.sector.C;
            }
            else if (tempNum == 4)
            {
                tempSec = ActivityClass.sector.D;
            }
            else
            {
                tempSec = ActivityClass.sector.E;
            }
            #endregion sectorChoice

            #region categoryChoice(Number[2])

            tempNum = int.Parse(numbers[2]);

            if (tempNum == 1)
            {
                tempCat = ActivityClass.category.Work;
            }
            else if (tempNum == 2)
            {
                tempCat = ActivityClass.category.Shopping;
            }
            else if (tempNum == 3)
            {
                tempCat = ActivityClass.category.Family;
            }
            else
            {
                tempCat = ActivityClass.category.Personal;
            }
            #endregion

            tempCost = int.Parse(numbers[3]);

            tempMorale[0] = int.Parse(numbers[4]);

            tempMorale[1] = int.Parse(numbers[5]);

            tempMorale[2] = int.Parse(numbers[6]);

            tempMorale[3] = int.Parse(numbers[7]);

            #region availabilityReading(number [8 to 10])
            if (int.Parse(numbers[8]) == 0)
                tempAva[0] = false;
            else
                tempAva[0] = true;

            if (int.Parse(numbers[9]) == 0)
                tempAva[1] = false;
            else
                tempAva[1] = true;

            if (int.Parse(numbers[10]) == 0)
                tempAva[2] = false;
            else
                tempAva[2] = true;
            #endregion

            if (int.Parse(numbers[11]) == 0)
                tempShop = false;
            else
                tempShop = true;


            tempActivity = new ActivityClass(activityName, tempSec, tempCat, tempCost, tempMorale, tempAva, tempShop);

            if (tempActivity.isItShop)
            {
                //Here I will assign it a shop
            }

            //This section choices what lists will be receiving the activities
            if (tempAva[0])
            {
                mornActivities.Add(tempActivity);
            }

            if (tempAva[1])
            {
                noonActivities.Add(tempActivity);
            }

            if (tempAva[2])
            {
                evenActivities.Add(tempActivity);
            }
        }
        #endregion
    }

    public void saveData()
    {
        savedObject.saveData(City, houseStats, currentDay, currentTime, Family, mornActivities, noonActivities, evenActivities, sicknesses);
    }

    public void loadData()
    {
        SaveLoad.Load();
        SaveLoad.savedGame.copyData(City, houseStats, currentDay, Family, mornActivities, noonActivities, evenActivities, sicknesses);
        currentTime = SaveLoad.savedGame.getSavedTime();
        //Debug.Log(currentTime);
    }
}
