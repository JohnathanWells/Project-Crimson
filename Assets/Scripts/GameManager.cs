using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum timeOfDay { Morning, Afternoon, Evening };

public class GameManager : MonoBehaviour {

    [Header("Screens")]
    public Transform mainGameplayScreen;
    public Transform shopScreen;

    [Header("Pop-Up")]
    public Transform popUpWindow;
    public SpriteRenderer popUpPicture;
    public TextMesh popUpText;
    public int popUpParagraphWidth;
    public int popUpMaxNumberOfLines;
    public SpriteRenderer[] popUpMoraleArrows;
    public TextMesh[] popUpMoraleNumbers;
    public Color[] colors = new Color[3];
    public Transform[] popUpFamilyMembers;
    [Tooltip("0 = Error\n 1 = fight, 2 = member leaves house, 3 = suicide, 4 = murder, 5 = asylum\n 6 = food alert, 7 = services alert, 8 = sick alert, 9 = item alert, 10 = depressed alert\n11 to 20 = Events\n21 to 100 = Activities")]
    public Sprite[] popUpIllustrations;
    public Sprite[] popUpArrowSprites = new Sprite[3];
    Queue<notificationClass> notificationQueue = new Queue<notificationClass>();

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
    public ActivityClass servicePayActivity;
    int membersAlive = 4;
    int membersInHouse = 4;

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
    public bool dadStartsSick = false;
    public bool createSessionAtStart = false;
    public List<ActivityClass> mornActivities = new List<ActivityClass>();
    public List<ActivityClass> noonActivities = new List<ActivityClass>();
    public List<ActivityClass> evenActivities = new List<ActivityClass>();
    public List<sicknessClass> sicknesses = new List<sicknessClass>();
    savefileClass savedObject = new savefileClass();


    void Start()
    {
        Resources.UnloadUnusedAssets();

        if (createSessionAtStart)
            newSession();
        else
            loadData();

        updateComponents();

        initializeServicePayActivity();

        //Debug
        if (dadStartsSick)
            Family[0].getsSick(sicknesses[5]);

        enqueuePopUp("Pop Up Windows are supposed to display when a member of the family is sick or death. They might also be used to illustrate some mechanics.", 0);
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

        Debug.Log(notificationQueue.Count);

        if (notificationQueue.Peek() != null)
           openPopUp(notificationQueue.Dequeue());
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

        //Family members consume food and medicines. Their morale also is affected by their psyche
        dailyFoodConsumption();
        dailySickEffect();
        dailyHealthAndMoraleDrift();

        saveData();
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

        if (Family[chosenFamilyMember].status.ID != 0 && Random.Range(1, 100) > (tempH * (2 / 3)) && Random.Range(1, 100) <= currentPlagueRate)
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
                Family[chosenFamilyMember].getsSick(illnesses[chosenSickness]);
                Debug.Log(Family[chosenFamilyMember].firstName + " is sick with " + illnesses[chosenSickness].name);
            }
        }
    }

    //Family stats changes with time
    public void dailyFoodConsumption()
    {
        int foodConsumed = 0;

        for (int n = 3; n >= 0; n--)
        {
            if (!Family[n].dead && !Family[n].gone)
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

    public void dailyHealthAndMoraleDrift()
    {
        int mourningLevel = 0;
        int missingLevel = 0;

        int temp;

        for (int n = 0; n < 4; n++)
        {
            if (!Family[n].gone && !Family[n].dead)
            {
                temp = Family[n].moraleHealthDrop(houseStats.servicesPaid, houseStats.getHygiene() > 0);

                if (n != 0) //This can never happen to the father
                {
                    if (temp == 1) //The member of the family gets in a fight with other characters. 50% chance.
                    {
                        if (membersInHouse >= 3) //If there are three or more people in the house, the character gets in fight with two
                        {
                            Family[n].moraleChange(-2); //The unstable character loses 2 morale
                            int temp2;

                            do
                            {
                                temp2 = Random.Range(0, 3);
                            } while (Family[temp2].dead || Family[temp2].gone || temp2 == n); //This dowhile makes sure they dont get in a fight with dead people or is the one we are looking at

                            enqueuePopUp(Family[n].firstName + " got into a fight with " + Family[temp2].firstName, 1);
                            //Debug.Log(Family[n].firstName + " got into a fight with " + Family[temp2].firstName);
                            Family[temp2].moraleChange(-5); //The people they get in a fight with loses 5 morale
                        }
                    }
                    else if (temp == 2) //There is a 25% chance the character leaves the house.
                    {
                        enqueuePopUp(Family[n].firstName + " left the house.", 2);
                        //Debug.Log(Family[n].firstName + " left the house.");
                        missingLevel += Family[n].leavesTheHouse();
                    }
                    else if (temp == 3) //There is a 10% chance the character kills themselves
                    {
                        enqueuePopUp(Family[n].firstName + " commited suicide.", 3);
                        //Debug.Log(Family[n].firstName + " commited suicide.");
                        mourningLevel += Family[n].dies();
                        missingLevel += 1;
                    }
                    else if (temp == 4 || temp == 5) //There is a 5% chance they kill someone else
                    {
                        if (membersInHouse >= 3) //If there are more than 2 people in the house
                        {
                            int temp2;

                            do
                            {
                                temp2 = Random.Range(1, 3);
                            } while (Family[temp2].dead || Family[temp2].gone || temp2 == n); //We make sure the selected member isnt dead or gone or is the character we are currently looking at

                            enqueuePopUp(Family[temp2].firstName + " was killed by " + Family[n].firstName, 4);
                            //Debug.Log(Family[temp2].firstName + " was killed by " + Family[n].firstName);
                            mourningLevel += Family[temp2].dies(); //The selected character dies
                            missingLevel += 1;
                        }
                        else //If there are only two people in the house, game over
                            gameOver();


                        if (temp == 5) //If the character is a kid, they leave the house 
                        {
                            enqueuePopUp(Family[n].firstName + " is sent to an asylum.", 5);
                            missingLevel += Family[n].leavesTheHouse();
                        }
                        else
                        {
                            enqueuePopUp(Family[n].firstName + " commited suicide.", 3);
                            mourningLevel += Family[n].dies(); //If a character is the mother, they kill themselves
                            missingLevel += 1;
                        }

                    }
                }
            }
        }


        for (int n = 0; n < 4; n++) //We go through each member
            if (!Family[n].dead && !Family[n].gone) //If the member is in the house and alive
            {
                mourningLevel += Family[n].healthDrift(); //Their health drifts as usual
            }

        if (mourningLevel > 0 || missingLevel > 0)
        {
            membersAlive -= mourningLevel;
            membersInHouse -= Mathf.Max(missingLevel, mourningLevel);

            for (int i = 0; i < 4; i++)
            {
                Family[i].mourningAdd(mourningLevel);
                Family[i].missingAdd(missingLevel);
            }
        }
    }

    public void dailySickEffect()
    {
        for (int n = 0; n < 4; n++)
        {
            houseStats.modMed(Family[n].sickDayPasses());
        }
    }

    public void gameOver()
    {
        //Game over
    }

    //Activity and stuff
    public void executeActivity(ActivityClass activity)
    {
        if (!activity.isItShop && !activity.paysService)
        {
            enqueuePopUp(activity);
            houseStats.addMoney(-activity.cost);

            for (int n = 0; n < 4; n++)
            {
                Family[n].moraleChange(activity.moraleChange[n]);
            }

            transitionTime();
        }
        else if (activity.paysService)
        {
            enqueuePopUp(activity);
            houseStats.addMoney(-activity.cost);
            houseStats.servicesPaid = true;

            transitionTime();
        }
        else if (activity.isItShop)
        {
            changeScreen(1, activity.shopAttached);
        }
    }

    public void finishShopping()
    {
        changeScreen(0);
        transitionTime();
    }

    void changeScreen(int screen, ShopClass shop)
    {
        if (screen == 0)
        {
            mainGameplayScreen.gameObject.SetActive(true);
            shopScreen.gameObject.SetActive(false);
        }
        else
        {
            mainGameplayScreen.gameObject.SetActive(false);
            shopScreen.gameObject.SetActive(true);
            shopScreen.SendMessage("setInitialValues", shop);
        }
    }

    void changeScreen(int screen)
    {
        if (screen == 0)
        {
            mainGameplayScreen.gameObject.SetActive(true);
            shopScreen.gameObject.SetActive(false);
        }
    }

    void initializeServicePayActivity()
    {
        servicePayActivity = new ActivityClass("Pay Services", ActivityClass.sector.B, ActivityClass.category.Family, costOfServices, new int[] { 0, 0, 0, 0 }, new bool[] { true, true, false }, false);
        servicePayActivity.paysService = true;
    }

    //Pop up
    public void enqueuePopUp(string text, int pictureNum)
    {
        //Debug.Log(text + " - " + pictureNum);
        notificationQueue.Enqueue(new notificationClass(text, pictureNum));
    }

    public void enqueuePopUp(ActivityClass activity)
    {
        notificationQueue.Enqueue(new notificationClass(activity));
    }

    public void openPopUp(notificationClass not)
    {
        popUpWindow.gameObject.SetActive(true);
        popUpPicture.sprite = popUpIllustrations[Mathf.Clamp(not.pictureNum, 0, popUpIllustrations.Length)];
        popUpText.text = warppedText(popUpParagraphWidth, not.text);
        //int arrowTemp = 0;

        if (not.type == notificationType.ActivityDescription)
        {
            for (int n = 0; n < 4; n++)
            {
                if (!Family[n].dead && !Family[n].gone)
                {
                    popUpMoraleNumbers[n].text = not.moodChange[n].ToString(); //Error here

                    if (not.moodChange[n] > 0)
                        popUpMoraleNumbers[n].color = colors[2];
                    else if (not.moodChange[n] == 0)
                        popUpMoraleNumbers[n].color = colors[1];
                    else
                        popUpMoraleNumbers[n].color = colors[0];


                    //popUpMoraleArrows[n].sprite = popUpArrowSprites[arrowTemp];
                }
                else
                {
                    popUpFamilyMembers[n].gameObject.SetActive(false);
                    //popUpMoraleArrows[n].gameObject.SetActive(false);
                    popUpMoraleNumbers[n].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int n = 0; n < 4; n++)
            {
                popUpFamilyMembers[n].gameObject.SetActive(false);
                //popUpMoraleArrows[n].gameObject.SetActive(false);
                popUpMoraleNumbers[n].gameObject.SetActive(false);
            }
        }
    }

    public void closePopUp()
    {
        notificationClass temp;

        popUpWindow.gameObject.SetActive(false);

        if (notificationQueue.Peek() != null)
        { 
        temp = notificationQueue.Dequeue();

        openPopUp(temp);
        }
    }

    string warppedText(int width, string input)
    {
        string[] words = input.Split(" "[0]);
        string line = "";
        string temp;
        string result = "";
        int lineCount = 0;

        foreach(string s in words)
        {
            if (lineCount < popUpMaxNumberOfLines)
            {
                temp = line + s + " ";

                if (temp.Length > width)
                {
                    lineCount++;
                    result += line + "\n";
                    line = s + " ";
                }
                else
                {
                    line = temp;
                }
            }
            else
            {
                line = "";
                break;
            }
        }

        result += line;

        return result;
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
        sicknessClass tempS = new sicknessClass(0, "Healthy", 0, 0, 0, 0, 0, " ");

        sicknesses.Add(tempS);

        for (int n = 0; n < lines.Length; n++)
        {
            numbers = lines[n].Split('\t');

            tempS = new sicknessClass(n + 1, numbers[0], int.Parse(numbers[1]), int.Parse(numbers[2]), int.Parse(numbers[3]), int.Parse(numbers[4]), float.Parse(numbers[5]), numbers[6]);

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
        int tempShopID = 0;
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
            {
                tempShop = false;
            }
            else
            {
                tempShop = true;
            }


            tempActivity = new ActivityClass(activityName, tempSec, tempCat, tempCost, tempMorale, tempAva, tempShop);

            tempActivity.setNotiInfo(numbers[12], int.Parse(numbers[13]));

            if (tempActivity.isItShop)
            {
                //Here I will assign it a shop
                //TODO more complex stuff

                tempActivity.setStore(new ShopClass(10, 10, 10, 10, tempShopID, tempActivity.activityName));
                tempActivity.shopAttached.setCosts(1, 1, 1, 1);
                tempShopID++;
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
