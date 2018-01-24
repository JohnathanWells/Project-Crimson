using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum timeOfDay { morning, afternoon, evening };

public class GameManager : MonoBehaviour {

    [Header("Options")]
    public daysOfWeek whenAreStoresResupplied = daysOfWeek.Sunday;
    public int howOftenIsPlagueRateChecked = 7;

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
    public Transform popUpFamilyStats;
    public Transform[] popUpFamilyMembers;
    public Sprite[] popUpArrowSprites = new Sprite[3];
    Queue<notificationClass> notificationQueue = new Queue<notificationClass>();

    [Header("Pictures")]
    [Tooltip("0 = Error\n 1 = fight, 2 = member leaves house, 3 = suicide, 4 = murder, 5 = asylum, 6 = general death, 7 = information\n 8 = sick alert, 9 = item alert, 10 = services alert\n11 to 20 = Events\n21 to 100 = Activities")]
    public Sprite[] popUpIllustrations;
    public Sprite[] newsIllustrations;

    [Header("Game Over")]
    public Transform gameOverScreen;

    [Header("References")]
    public phoneScript Phone;
    public HouseScript House;
    public ActivityMenuScript Activities;

    [Header("Data to Pass")]
    public FamilyMembers[] Family = new FamilyMembers[Constants.familySize];
    public houseClass houseStats;
    public cityClass City;
    public bool houseNotification = false;
    public bool phoneNotification = false;
    public int costOfServices = 100;
    public ActivityClass servicePayActivity;
    int membersAlive = Constants.familySize;
    int membersInHouse = Constants.familySize;

    [Header("Files")]
    public TextAsset activityFile;
    public TextAsset statChangesFile;
    public TextAsset districtStatsFile;
    public TextAsset sicknessFile;
    public TextAsset storesFile;
    public TextAsset pricesFile;
    public TextAsset newsFile;
    public TextAsset wikiFile;
    
    [Header("Day Stuff")]
    public DayClass currentDay;
    public timeOfDay currentTime;
    public SpriteRenderer map;
    public Color[] timeColors = new Color[3];
    int daysSinceLastIllnessCheck = 0;
    int[] daysLeftForHealing = { 0, 0, 0, 0 };

    [Header("Permanent Stuff")]
    public bool dadStartsSick = false;
    public bool createSessionAtStart = false;
    public List<ActivityClass> mornActivities = new List<ActivityClass>();
    public List<ActivityClass> noonActivities = new List<ActivityClass>();
    public List<ActivityClass> evenActivities = new List<ActivityClass>();
    public List<sicknessClass> sicknesses = new List<sicknessClass>();
    public List<newsClass> News = new List<newsClass>();
    public wikiClass wikiDatabase;
    List<ActivityClass> stores = new List<ActivityClass>();
    savefileClass savedObject = new savefileClass();
    int[] itemPrices = new int[4];
    List<ObituaryClass> Obituaries = new List<ObituaryClass>();

    ActivityClass tempActivity;
    bool GAME_OVER = false;

    void Start()
    {
        Resources.UnloadUnusedAssets();

        //if (createSessionAtStart)
        //    newSession();
        //else
        //    loadData();

        newGameClass loadObject = new newGameClass();

        if (GameObject.FindWithTag("MainMenuObject") != null)
        {

            Transform obj = GameObject.FindWithTag("MainMenuObject").transform;
            obj.SendMessage("getClass", loadObject);

            if (loadObject.newGame)
            {
                newSession(loadObject);
            }
            else
            {
                loadData();
            }


            obj.SendMessage("signalLoaded");
        }
        else
        {
            SaveLoad.Load();
            if (createSessionAtStart || SaveLoad.savedGame == null || SaveLoad.savedGame.empty)
                newSession(null);
            else
                loadData();
        }

        //to here

        readWikiFile();

        updateComponents();

        initializeServicePayActivity();

        //Debug
        if (dadStartsSick)
            Family[0].getsSick(sicknesses[5]);

        if (notificationQueue.Count > 0)
            openPopUp(notificationQueue.Peek());

        //TODO REMOVE THIS AND REPLACE WITH SOMETHING BETTER    
        map.color = timeColors[(int)currentTime];
    }

    #region Time Progression
    //Time stuff
    public void transitionTime()
    {
        if (currentTime == timeOfDay.evening)
        {
            dayAdvance();
        }
        else
        {
            currentTime += 1;
        }

        //TODO REMOVE THIS AND REPLACE WITH SOMETHING BETTER    
        map.color = timeColors[(int)currentTime];

        updateComponents();

        if (GAME_OVER)
            SaveLoad.Delete();
        else
            saveData();

        if (notificationQueue.Count > 0 && notificationQueue.Peek() != null)
           openPopUp(notificationQueue.Peek());

        if (currentDay.month == 04 && currentDay.day == 31)
        {
            gameWon();
        }
    }

    public void dayAdvance()
    {
        currentDay.advanceDay();
        houseStats.timeLeftForPayment--;
        currentTime = timeOfDay.morning;
        daysSinceLastIllnessCheck++;

        if (currentDay.calculateDayOfWeek() == whenAreStoresResupplied && Random.Range(1, 2) == 1)
        {
            storeSupplying();
        }

        //Stats change according to time
        City.applyStatChanges(currentDay.month, currentDay.day);

        //House services are paid
        if (houseStats.timeLeftForPayment <= 0)
        {
            houseStats.payServices(costOfServices, currentDay); //I need to decide if the cost of services will be changed by inflation

            if (houseStats.servicesPaid)
            {
                enqueuePopUp("Service Pay Day. #n " + costOfServices + "$ have been discounted from your account.", 7); 
            }
            else
            {
                enqueuePopUp("Service Pay Day. #n You didn't have enough money to pay, so services such as water, electricity and internet have been cut. ", 10);
            }
        }

        //House Stats and Item Consumption are calculated
        if (houseStats.getCleaningQ() > 0)
        {
            houseStats.modCleaning(-1);
            if (houseStats.getCleaningQ() <= 0)
            {
                houseStats.setCleaning(0);
                enqueuePopUp("You ran out of cleaning items!", 11);
            }
        }

        if (houseStats.getHygiene() > 0 && (int)currentDay.calculateDayOfWeek() == 6)
        {
            houseStats.modHygiene(-1 * membersAlive);

            if (houseStats.getHygiene() <= 0)
            {
                houseStats.setHygiene(0);
                enqueuePopUp("You ran out of hygiene items!", 11);
            }
        }

        houseStats.calculatePlagueRate(City);

        //Check if a member becomes sick
        if (daysSinceLastIllnessCheck >= howOftenIsPlagueRateChecked && Random.Range(1, 2) == 1)
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

    void storeSupplying()
    {
        int[] inflatedPrices = new int[4];

        for (int n = 0; n < 4; n++)
        {
            //Debug.Log((float)itemPrices[n] * City.currentInflation);
            inflatedPrices[n] = Mathf.RoundToInt((float)itemPrices[n] * City.currentInflation);
        }
        

        foreach (ActivityClass a in stores)
        {
            ShopClass t = a.shopAttached;
            a.shopAttached.setItems(inflatedPrices[0], Mathf.FloorToInt(t.budgets[0] / inflatedPrices[0]), inflatedPrices[1], Mathf.FloorToInt(t.budgets[1] / inflatedPrices[1]), inflatedPrices[2], Mathf.FloorToInt(t.budgets[2] / inflatedPrices[2]), inflatedPrices[3], Mathf.FloorToInt(t.budgets[3] / inflatedPrices[3]));
        }
    }

    #endregion

    //Family effects
    public void illnessChecker()
    {
        int chosenFamilyMember = Random.Range(0, 3);

        if (!Family[chosenFamilyMember].gone && !Family[chosenFamilyMember].dead)
        {
            int worseAvailableSickness = 0;
            float currentPlagueRate = houseStats.plagueRate;
            int chosenSickness;
            float tempH = Family[chosenFamilyMember].health * (2 / 3);

            if (Family[chosenFamilyMember].status.ID == 0 && Random.Range(1, 100) > (tempH) && Random.Range(1, 100) <= currentPlagueRate)
            {
                //Debug.Log(Family[chosenFamilyMember].health);
                int n = 0;
                foreach (sicknessClass s in sicknesses )
                {
                    if (s.minPRCheck <= currentPlagueRate)
                    {
                        worseAvailableSickness = n;
                    }
                    else
                    {
                        break;
                    }
                    n++;
                }

                if (worseAvailableSickness > 0)
                {
                    chosenSickness = Random.Range(1, worseAvailableSickness);
                    Family[chosenFamilyMember].getsSick(sicknesses[chosenSickness]);
                    enqueuePopUp((Family[chosenFamilyMember].firstName + " hasn't been feeling well lately. Maybe you should check on them."), 8);
                }
            }
        }
    }

    #region Progression Effects and Wining/Losing Conditions
    //Family stats changes with time
    public void dailyFoodConsumption()
    {
        int foodConsumed = 0;

        for (int n = 3; n >= 0; n--)
        {
            if (!Family[n].dead && !Family[n].gone)
            {
                foodConsumed = Family[n].food;

                if (foodConsumed <= 0)
                    enqueuePopUp(Family[n].firstName + " did not eat yesterday. Their health will deteriorate as a result.", 0);

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
                        if (houseStats.getFoodQ() > 0)
                            enqueuePopUp("You ran out of food!", 9);

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

        for (int n = 0; n < Constants.familySize; n++)
        {
            if (!Family[n].gone && !Family[n].dead)
            {
                temp = Family[n].moraleHealthDrop(houseStats.servicesPaid, houseStats.getHygiene() > 0);

                if (n != 0 && membersInHouse > 1) //This can never happen to the father
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

                            enqueuePopUp(Family[n].firstName + " got into a fight with " + Family[temp2].firstName + ".", 1);
                            //Debug.Log(Family[n].firstName + " got into a fight with " + Family[temp2].firstName);
                            Family[temp2].moraleChange(-5); //The people they get in a fight with loses 5 morale
                        }
                    }
                    else if (temp == 2) //There is a 25% chance the character leaves the house.
                    {
                        enqueuePopUp("During the night, when everyone was sleeping, " + Family[n].firstName + " secretly packed all of their things and left the house.", 2, Color.yellow);
                        missingLevel += Family[n].leavesTheHouse();
                    }
                    else if (temp == 3) //There is a 10% chance the character kills themselves
                    {
                        enqueuePopUp(Family[n].firstName + " has commited suicide.", 3, Color.red);
                        mourningLevel += Family[n].dies();
                        missingLevel += 1;
                    }
                    else if (temp == 4 || temp == 5) //There is a 5% chance they kill someone else
                    {
                        Debug.Log("Murder happening");
                        if (membersInHouse >= 3) //If there are more than 2 people in the house
                        {
                            int temp2;

                            do
                            {
                                temp2 = Random.Range(1, 3);
                            } while (Family[temp2].dead || Family[temp2].gone || temp2 == n); //We make sure the selected member isnt dead or gone or is the character we are currently looking at

                            enqueuePopUp("During a particularly heated discussion, " + Family[temp2].firstName + " is killed by " + Family[n].firstName + ".", 4, Color.red);
                            mourningLevel += Family[temp2].dies(); //The selected character dies
                            missingLevel += 1;
                        }
                        else //If there are only two people in the house and one of them is unstable enough to commit murder, game over
                        {
                            enqueuePopUp("While you were sleeping, " + Family[n].firstName + " locks the door and sets the house on fire. Your neighbors will never forget your screams for the rest of their lives.", 4, Color.red);
                            GAME_OVER = true;
                            Family[n].deathCause = "Murder (Fire)";
                        }


                        if (temp == 5) //If the character is a kid, they leave the house 
                        {
                            enqueuePopUp("After the murder, there is no other choice but to send " + Family[n].firstName + " to a mental asylum.", 5, Color.yellow);
                            missingLevel += Family[n].leavesTheHouse();
                        }
                        else
                        {
                            enqueuePopUp("Moved by guilt and sorrow over the death of her child, " + Family[n].firstName + " commits suicide.", 3, Color.red);
                            mourningLevel += Family[n].dies(); //If a character is the mother, they kill themselves
                            missingLevel += 1;
                        }

                    }
                }
                else if (n == 0 && membersInHouse == 1) //The dad kills himself
                {
                    enqueuePopUp("The solitude, sorrow and grief finally takes over you. You throw yourself from a balcony in your office to your death.", 3, Color.red);
                    mourningLevel += Family[n].dies();
                    missingLevel += 1;
                    Family[n].deathCause = "Suicide";
                    GAME_OVER = true;
                }
            }
        }


        for (int n = 0; n < Constants.familySize; n++) //We go through each member
            if (!Family[n].dead && !Family[n].gone) //If the member is in the house and alive
            {
                temp = Family[n].healthDrift(); //Their health drifts as usual
                mourningLevel += temp;

                if (temp > 0)
                {
                    if (Family[n].status.ID != 0)
                    {
                        enqueuePopUp(Family[n].firstName + " wakes up dead that morning. #n The coroner says they died from <i> " + Family[n].status.name + " </i> .", 6, Color.red);
                        Family[n].deathCause = Family[n].status.name;

                        if (n == 0)
                            GAME_OVER = true;
                    }
                    else
                    {
                        enqueuePopUp(Family[n].firstName + " didn't wake up that morning. #n They starved to death.", 6, Color.red);
                        Family[n].deathCause = "Starved";

                        if (n == 0)
                            GAME_OVER = true;
                    }

                    saveNewObituary(new ObituaryClass(Family[n].firstName, Family[n].lastName, Family[n].deathCause, currentDay));
                }
            }

        if (mourningLevel > 0 || missingLevel > 0)
        {
            membersAlive -= mourningLevel;
            membersInHouse -= Mathf.Max(missingLevel, mourningLevel);

            for (int i = 0; i < Constants.familySize; i++)
            {
                Family[i].mourningAdd(mourningLevel);
                Family[i].missingAdd(missingLevel);
            }
        }
    }

    public void dailySickEffect()
    {
        for (int n = 0; n < Constants.familySize; n++)
        {
            houseStats.modMed(Family[n].sickDayPasses());
        }
    }

    public void gameOver()
    {
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.SendMessage("setFamily", Family);
        gameOverScreen.SendMessage("setDay", currentDay);
        gameOverScreen.SendMessage("displayGameOver");
    }

    public void gameWon()
    {
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.SendMessage("setFamily", Family);
        gameOverScreen.SendMessage("setDay", currentDay);
        gameOverScreen.SendMessage("displayVictory");
    }
    #endregion

    #region Activity Execution
    //Activity and stuff
    public void executeActivity(ActivityClass activity)
    {
        tempActivity = activity;
        if (!activity.isItShop && !activity.paysService)
        {
            enqueuePopUp(activity);
            houseStats.modMoney(-activity.cost);

            for (int n = 0; n < Constants.familySize; n++)
            {
                Family[n].moraleChange(activity.moraleChange[n]);
            }

            transitionTime();
        }
        else if (activity.paysService)
        {
            enqueuePopUp(activity);
            houseStats.modMoney(-activity.cost);
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
            House.SendMessage("toggleDrawer", true);
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

    public void Kill(int famMember, string message)
    {
        if (famMember >= 0 && famMember < Family.Length && !Family[famMember].dead)
        {
            Family[famMember].dies();
            membersAlive--;

            if (!Family[famMember].gone)
                membersInHouse--;

            foreach (FamilyMembers f in Family)
            {
                f.mourningAdd(1);

                if (!Family[famMember].gone)
                    f.missingAdd(1);
            }

            enqueuePopUp(message, 6);

            if (famMember == 0)
                GAME_OVER = true;
        }
    }

    public void toggleGone(int famMember, string message, bool inOUT)
    {
        if (famMember >= 0 && famMember < Family.Length && !Family[famMember].dead)
        {
            if (inOUT && !Family[famMember].gone)
            {
                Family[famMember].leavesTheHouse();
                membersInHouse--;

                foreach (FamilyMembers f in Family)
                {
                    f.missingAdd(1);
                }

                enqueuePopUp(message, 0);
            }
            else if (inOUT && !Family[famMember].gone)
            {
                Family[famMember].comesBack();
                membersInHouse++;

                foreach (FamilyMembers f in Family)
                {
                    f.missingAdd(-1);
                }

                enqueuePopUp(message, 0);
            }
        }
    }
    #endregion

    #region Pop Up Functions
    //Pop up
    public void enqueuePopUp(string text, int pictureNum)
    {
        //Debug.Log(text + " - " + pictureNum);
        notificationQueue.Enqueue(new notificationClass(text, pictureNum));
    }

    public void enqueuePopUp(string text, int pictureNum, Color color)
    {
        //Debug.Log(text + " - " + pictureNum);
        notificationQueue.Enqueue(new notificationClass(text, pictureNum, color));
    }

    public void enqueuePopUp(ActivityClass activity)
    {
        notificationQueue.Enqueue(new notificationClass(activity));
    }

    public void openPopUp(notificationClass not)
    {
        popUpWindow.gameObject.SetActive(true);
        popUpPicture.sprite = popUpIllustrations[Mathf.Clamp(not.pictureNum, 0, popUpIllustrations.Length - 1)];
        popUpText.text = warppedText(popUpParagraphWidth, not.text);
        //int arrowTemp = 0;

        popUpText.color = not.color();

        if (not.type == notificationType.ActivityDescription)
        {
            popUpFamilyStats.gameObject.SetActive(true);

            for (int n = 0; n < Constants.familySize; n++)
            {
                if (!Family[n].dead && !Family[n].gone)
                {
                    string mc = not.moodChange[n].ToString();

                    if (not.moodChange[n] > 0)
                        mc = "+" + mc;

                    popUpMoraleNumbers[n].text = mc; //Error here

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
                    //popUpMoraleNumbers[n].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            popUpFamilyStats.gameObject.SetActive(false);
        }
    }

    public void closePopUp()
    {
        notificationQueue.Dequeue();
        notificationClass temp;

        popUpWindow.gameObject.SetActive(false);

        if (notificationQueue.Count > 0 && notificationQueue.Peek() != null)
        { 
        temp = notificationQueue.Peek();
        openPopUp(temp);
        }
        else if (GAME_OVER)
        {
            gameOver();
        }
        
    }

    string warppedText(int width, string input)
    {
        string[] words = input.Split(" "[0]);
        string line = "";
        string temp;
        string replacedWord;
        string result = "";
        int lineCount = 0;

        foreach(string s in words)
        {
            if (lineCount < popUpMaxNumberOfLines)
            {
                replacedWord = replaceReferences(s);
                temp = line + replacedWord;

                if (temp.Length > width && replacedWord != "\n")
                {
                    lineCount++;
                    result += line + "\n";
                    line = replacedWord + " ";
                }
                else if (temp.Length <= width && replacedWord == "\n")
                {
                    lineCount++;
                    result += temp;
                    line = "";
                }
                else
                {
                    line = temp + " ";
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

    string replaceReferences(string str)
    {
        if (str != "" && str[0] == '#')
        {
            switch (str[1])
            {
                case 'C':
                    if (str.Length > 2)
                        if (str[2] != 'n')
                            return City.districts[int.Parse(str[2].ToString()) - 1].districtName;
                        else
                            return City.districts[(int)tempActivity.area].districtName;
                    else
                        return str;
                case 'T': //Returns the current time slot
                    return currentTime.ToString();
                case 'W': //Returns the current day of the week
                    return currentDay.calculateDayOfWeek().ToString();
                case 'D': //Returns the current date
                    return currentDay.calculateMonth() + " " + currentDay.day + "th";
                case 'M': //Returns the current month
                    return currentDay.calculateMonth();
                case 'f': //Refering to the whole family
                    if (membersInHouse == Constants.familySize)
                        return "your family";
                    else if (membersInHouse > 1)
                        return "what's left of your family";
                    else
                        return "no one";
                case '0':
                    return Family[0].firstName;
                case '1':
                    return Family[1].firstName;
                case '2':
                    return Family[2].firstName;
                case '3':
                    return Family[3].firstName;
                case 'k': //Refering to the kids only
                    {
                        if (!Family[2].dead && !Family[2].gone && !Family[3].dead && !Family[3].gone)
                        {
                            return Family[2].firstName + " and " + Family[3].firstName;
                        }
                        else if (!Family[2].dead && !Family[2].gone && (Family[3].dead || Family[3].gone))
                        {
                            return Family[2].firstName;
                        }
                        else if ((Family[2].dead || Family[2].gone) && !Family[3].dead && !Family[3].gone)
                        {
                            return Family[3].firstName;
                        }
                        else
                        {
                            return "your sorrow and agony";
                        }
                    }
                case 'n':
                    return "\n";
                default:
                    return str;
            }
        }
        else
            return str;
    }
    #endregion

    #region Data and Saving Management
    //Data and saving management stuff
    void newSession(newGameClass startupData)
    {
        City = new cityClass(districtStatsFile, statChangesFile);
        houseStats = new houseClass();
        currentDay = new DayClass(10, 1);
        currentTime = timeOfDay.morning;

        setItemPrices();

        readActivities();

        readSicknesses();

        readNewsFile();

        if (startupData != null)
        {
            for (int n = 0; n < Constants.familySize; n++)
            {
                Family[n] = new FamilyMembers(startupData.family[n].member);
                Family[n].setName(startupData.family[n].firstName, startupData.family[n].lastName);
            }
        }
        else
        {
            Family[0] = new FamilyMembers(FamilyMembers.famMember.Dad);
            Family[0].setName("Pedro", "Perez");
            Family[1] = new FamilyMembers(FamilyMembers.famMember.Mom);
            Family[1].setName("Maria", "Perez");
            Family[2] = new FamilyMembers(FamilyMembers.famMember.Son);
            Family[2].setName("Jose", "Perez");
            Family[3] = new FamilyMembers(FamilyMembers.famMember.Dau);
            Family[3].setName("Juana", "Perez");
        }

        houseStats.payServices(0, currentDay);

        storeSupplying();

        enqueuePopUp("You can find more information about the game in your phone [WORK IN PROGRESS].", 0);

        savedObject.getObituaries(Obituaries);
        sortListOfDays(Obituaries);

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
        int[] tempMorale = new int[Constants.familySize];
        bool[] tempAva = new bool[3];
        bool tempShop;
        int tempShopID = 0;
        string[] lines = activityFile.text.Split('\n');
        string[] numbers;
        int tempNum;
        string[] storeLines = storesFile.text.Split('\n');
        string[] storeInfo;

        for (int n = 0; n < lines.Length; n++)
        {
            numbers = lines[n].Split('\t');
            //foreach (string s in numbers)
            //    Debug.Log(s);
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
                storeInfo = storeLines[tempShopID].Split('\t');
                ShopClass temp = new ShopClass(int.Parse(storeInfo[1]), int.Parse(storeInfo[2]), int.Parse(storeInfo[3]), int.Parse(storeInfo[4]), tempShopID, tempActivity.activityName, float.Parse(storeInfo[9]));
                temp.setShortageRates(int.Parse(storeInfo[5]), int.Parse(storeInfo[6]), int.Parse(storeInfo[7]), int.Parse(storeInfo[8]));
                tempActivity.setStore(temp);
                stores.Add(tempActivity);
                tempShopID++;
            }
            //Debug.Log(tempActivity.activityName);

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

    public void readNewsFile()
    {
        string [] articles = newsFile.text.Split('\n');

        foreach(string str in articles)
        {
            string[] parts = str.Split('\t');
            DayClass dateForCurrentNew = new DayClass(int.Parse(parts[0]), int.Parse(parts[1]));
            //Debug.Log(dateForCurrentNew.day + " - " + dateForCurrentNew.month);
            //Debug.Log(str);

            if (parts.Length > 4)
                News.Add(new newsClass(dateForCurrentNew, parts[2], parts[3], newsIllustrations[int.Parse(parts[4])]));
            else
                Debug.Log("ERROR reading news");
        }
    }

    public void readWikiFile()
    {
        wikiDatabase = new wikiClass();
        wikiDatabase.title = "Information";

        string[] articles = wikiFile.text.Split('\n');

        foreach (string str in articles)
        {
            //Debug.Log(str);
            string[] parts = str.Split('\t');
            string[] indexes = parts[0].Split('.');

            //Debug.Log(parts.Length + " / " + indexes.Length + " [] " + parts[0]);

            if (parts.Length > 2)
            {
                int firstIndex = int.Parse(indexes[0]);
                if (indexes.Length > 1)
                {
                   // Debug.Log(wikiDatabase.subordinates.Count + " vs " + firstIndex);
                    if (firstIndex < wikiDatabase.subordinates.Count && firstIndex >= 0)
                    {
                        Queue<int> nav = new Queue<int>();
                        foreach (string num in indexes)
                        {
                            nav.Enqueue(int.Parse(num));
                        }
                        //Debug.Log(parts[1] + ": " + parts[2]);

                        wikiDatabase.addSubordinate(new wikiClass(parts[1], parts[2]), nav);
                    }
                }
                else
                {
                    wikiDatabase.addSubordinate(new wikiClass(parts[1], parts[2]));
                }

            }
            else
                Debug.Log("ERROR reading wikiarticle: " + str);
        }

        //Debug.Log(wikiDatabase.subordinates.Count + " main categories in total");
    }

    public void saveData()
    {
        savedObject.saveData(City, houseStats, currentDay, currentTime, Family, mornActivities, noonActivities, evenActivities, sicknesses, notificationQueue, itemPrices, stores);
        savedObject.saveObituaries(Obituaries);
        savedObject.saveNews(News);
        SaveLoad.Save();
    }

    public void loadData()
    {
        City = new cityClass();
        SaveLoad.Load();
        SaveLoad.savedGame.copyData(City, houseStats, currentDay, Family, mornActivities, noonActivities, evenActivities, sicknesses, notificationQueue, itemPrices, stores);
        currentTime = SaveLoad.savedGame.getSavedTime();
        SaveLoad.savedGame.getObituaries(Obituaries);
        SaveLoad.savedGame.getNews(News);
        //Debug.Log(currentTime);
    }

    void setItemPrices()
    {
        string[] lines = pricesFile.text.Split('\n');
        string[] tempData;

        for (int n = 0; n < itemPrices.Length; n++)
        {
            tempData = lines[n].Split('\t');

            itemPrices[n] = int.Parse(tempData[1]);
        }

    }

    static int sortObituariesByDate(ObituaryClass A, ObituaryClass B)
    {
        if (A.dateOfDeath.month.CompareTo(B.dateOfDeath.month) == 0)
            return A.dateOfDeath.day.CompareTo(B.dateOfDeath.day);
        else
            return 0;
    }

    void sortListOfDays(List<ObituaryClass> list)
    {
        if (list.Count > 0)
            list.Sort(sortObituariesByDate);
    }

    void saveNewObituary(ObituaryClass ob)
    {
        savedObject.saveNewObituaty(ob);
        saveData();
    }

    #endregion

    #region eventKeeper

    //Put these inside a subclass so they are not just hanging around
    string[] robberyEvents = { "pickpocketing()", "discreteMugging()", "violentMugging()", "lethalMug()"};
    string[] kidnapEvents = { "expressKidnapping()", "desperateKidnapping()" };
    string[] trafficEvents = { "blockedRoad()", "smallCarAccident()" };
    string[] negligenceEvents = { "disservice()", "uselessness()" };
    int daysSinceLastRobbery = 0;

    bool kidnappedAlready = false;
    int timeBeforeRescue = -1;
    int kidnappedMember;

    int daysSinceLastTraffic = 0;

    int daysSinceNegligence = 0;

    bool executeEvent(string name)
    {
        switch (name)
        {
            case "pickpocketing()":
                return pickpocketing();
            case "discreteMugging()":
                return discreteMugging();
            case "violentMugging()":
                return violentMugging();
            case "lethalMug()":
                return lethalMug();
            case "expressKidnapping()":
                return expressKidnapping();
            case "desperateKidnapping()":
                return desperateKidnapping();
            case "blockedRoad()":
                return blockedRoad();
            case "smallCarAccident()":
                return smallCarAccident();
            case "disservice()":
                return disservice();
            default:
                return false;
        }
    }

    void eventCheck()
    {
            
    }

    //Robbery Events
    bool pickpocketing()
    {
        //Constant variables
        int moraleEffect = -5;
        int minMoneyLost = 10;
        int maxMoneyLost = 500;
        int pictureUsed = 0;
        int minTimePassed = 7;

        float currentDistrictCrime = 0;

        switch ((int)currentTime)
        {
            case 0:
                currentDistrictCrime = City.districts[(int)tempActivity.area].currentCRMorning;
                break;
            case 1:
                currentDistrictCrime = City.districts[(int)tempActivity.area].currentCRNoon;
                break;
            case 2:
                currentDistrictCrime = City.districts[(int)tempActivity.area].currentCREvening;
                break;
            default:
                currentDistrictCrime = 0;
                break;
        }
        float triggerMax = (currentDistrictCrime * City.currentCrime / 2);


        if (Random.Range(0, 100) < triggerMax && minTimePassed < daysSinceLastRobbery)
        {
            string descriptionText = "";

            //Effects on money
            int a = houseStats.getMoney();
            houseStats.modMoney(-Random.Range(minMoneyLost, maxMoneyLost));
            int b = houseStats.getMoney();

            Family[0].moraleChange(moraleEffect);
            descriptionText += "Without you noticing it, someone stole your wallet. ";

            //Money text variants
            if (a - b <= (maxMoneyLost - minMoneyLost) / 2)
            {
                descriptionText += "At least you weren't carrying too much cash.";
            }
            else
            {
                descriptionText += "You are robbed from this week's earnings.";
            }

            daysSinceLastRobbery = 0;

            enqueuePopUp(descriptionText, pictureUsed);

            return true;
        }
        else
        {
            return false;
        }
    }

    bool discreteMugging()
    {
        //Constant variables
        int moraleEffect = -10;
        int minMoneyLost = 10;
        int maxMoneyLost = 500;
        int pictureUsed = 0;
        int minTimePassed = 7;

        //Semi Constant variables
        bool isSpecificSector = (tempActivity.area == ActivityClass.sector.B || tempActivity.area == ActivityClass.sector.C || tempActivity.area == ActivityClass.sector.E || tempActivity.area == ActivityClass.sector.F);
        bool isSpecificTime = (currentTime == timeOfDay.evening);
        float triggerMax = (City.districts[(int)tempActivity.area].baseCrimeRateEvening) * City.currentCrime;

        if (minTimePassed < daysSinceLastRobbery && isSpecificTime && isSpecificSector && Random.Range(0, 100) < triggerMax)
        {
            string descriptionText = "";

            //Effects on money
            int a = houseStats.getMoney();
            houseStats.modMoney(-Random.Range(minMoneyLost, maxMoneyLost));
            int b = houseStats.getMoney();

            //Morale effect and text variants
            if (tempActivity.activityCategory == ActivityClass.category.Family)
            {
                foreach (FamilyMembers f in Family)
                {
                    f.moraleChange(moraleEffect);
                }

                descriptionText += "On the way to the car, a guy with a gun jumps on you and your family and assaults you. ";
            }
            else
            {
                Family[0].moraleChange(moraleEffect);
                descriptionText += "On the way to the car, a guy with a gun jumps on you and assaults you. ";
            }

            //Money text variants
            if (a - b <= (maxMoneyLost - minMoneyLost) / 2)
            {
                descriptionText += "At least you weren't carrying too much cash.";
            }
            else
            {
                descriptionText += "You are robbed from this week's earnings.";
            }

            daysSinceLastRobbery = 0;
            enqueuePopUp(descriptionText, pictureUsed);

            return true;
        }
        else
        {
            return false;
        }
    }

    bool violentMugging()
    {
        //Constant variables
        int moraleEffect = -15;
        int minMoneyLost = 10;
        int maxMoneyLost = 500;
        int pictureUsed = 0;
        int minTimePassed = 7;
        float exMinChaos = 4;

        //Semi Constant variables
        float currentDistrictCrime = 0;

        switch ((int)currentTime)
        {
            case 0:
                currentDistrictCrime = City.districts[(int)tempActivity.area].currentCRMorning;
                break;
            case 1:
                currentDistrictCrime = City.districts[(int)tempActivity.area].currentCRNoon;
                break;
            case 2:
                currentDistrictCrime = City.districts[(int)tempActivity.area].currentCREvening;
                break;
            default:
                currentDistrictCrime = 0;
                break;
        }
        float triggerMax = (currentDistrictCrime * City.currentCrime);

        if (City.currentChaos > exMinChaos && minTimePassed < daysSinceLastRobbery && Random.Range(0, 100) < triggerMax)
        {
            string descriptionText = "";

            //Effects on money
            int a = houseStats.getMoney();
            houseStats.modMoney(-Random.Range(minMoneyLost, maxMoneyLost));
            int b = houseStats.getMoney();

            //Morale effect and text variants
            if (tempActivity.activityCategory == ActivityClass.category.Family)
            {
                foreach (FamilyMembers f in Family)
                {
                    f.moraleChange(moraleEffect);
                }

                descriptionText += "On the way to the car, a guy with a gun jumps on you and your family and assaults you. ";
            }
            else
            {
                Family[0].moraleChange(moraleEffect);
                descriptionText += "On the way to the car, a guy with a gun jumps on you and assaults you. ";
            }

            //Money text variants
            if (a - b <= (maxMoneyLost - minMoneyLost) / 3)
            {
                if (tempActivity.activityCategory == ActivityClass.category.Family)
                {
                    if (membersAlive > 1 && membersInHouse > 1)
                    {
                        for (int n = 1; n < 4; n++)
                        {
                            if (!Family[n].dead && !Family[n].gone)
                            {
                                descriptionText += "Because you had so little money, the mugger gets angry and shot " + Family[n].firstName + " in the chest. ";

                                if (Random.Range(0, 4) > 2)
                                {
                                    descriptionText += "Fortunately, you manage to take them to the emergency room of the hospital and they manage to save their life.";

                                    foreach (FamilyMembers f in Family)
                                        f.moraleChange(moraleEffect);
                                }
                                else
                                {
                                    Kill(n, "The bullet pierces the lungs. " + Family[n].firstName + " dies in your arms drowned in their own blood.");

                                    foreach (FamilyMembers f in Family)
                                        f.moraleChange(moraleEffect * 2);
                                }


                                break;
                            }
                        }
                    }
                    else
                    {
                        descriptionText += "The mugger takes mercy on you and decides not to kill you.";
                    }
                }
                else
                    descriptionText += "At least you weren't carrying too much cash.";
            }
            else
            {
                descriptionText += "You are robbed from this week's earnings.";
            }

            daysSinceLastRobbery = 0;
            enqueuePopUp(descriptionText, pictureUsed);

            return true;
        }
        else
        {
            return false;
        }

    }

    bool lethalMug()
    {
        //Constant variables
        int moraleEffect = -15;
        int minMoneyLost = 10;
        int maxMoneyLost = 500;
        int pictureUsed = 0;
        int minTimePassed = 7;
        float exMinChaos = 7;
        float exMinCrime = 6;
        int maxSpareMoney = 100;

        //Semi Constant variables
        float currentDistrictCrime = 0;

        switch ((int)currentTime)
        {
            case 0:
                currentDistrictCrime = City.districts[(int)tempActivity.area].currentCRMorning;
                break;
            case 1:
                currentDistrictCrime = City.districts[(int)tempActivity.area].currentCRNoon;
                break;
            case 2:
                currentDistrictCrime = City.districts[(int)tempActivity.area].currentCREvening;
                break;
            default:
                currentDistrictCrime = 0;
                break;
        }
        float triggerMax = (currentDistrictCrime);

        if (City.currentChaos > exMinChaos && City.currentCrime > exMinCrime && minTimePassed < daysSinceLastRobbery && Random.Range(0, 100) < triggerMax)
        {
            string descriptionText = "";

            //Effects on money
            int a = houseStats.getMoney();
            houseStats.modMoney(-Random.Range(minMoneyLost, maxMoneyLost));
            int b = houseStats.getMoney();

            //Morale effect and text variants
            Family[0].moraleChange(moraleEffect);
            descriptionText += "On the way to the car, a guy with a gun jumps on you and assaults you. ";

            //Money text variants
            if (a - b < maxSpareMoney)
            {
                descriptionText += "The mugger takes your wallet and notices you don't even have " + maxSpareMoney + " on you. He gets angry and starts shouting at you.";
                Kill(0, "You see a flash of light and fall dead on the floor.");
            }
            else
            {
                descriptionText += "You are robbed from this week's earnings.";
            }
            daysSinceLastRobbery = 0;
            enqueuePopUp(descriptionText, pictureUsed);

            return true;
        }
        else
        {
            return false;
        }

    }


    //Kidnapping Events
    bool expressKidnapping()
    {
        //Constants
        float exMinChaos = 6;
        float exMinCrime = 4;
        int minMoney = 10000;
        int moodChange = -15;
        int pictureUsed = 0;

        //Semi constant
        float triggerMax = City.districts[3].currentCREvening;

        if (!kidnappedAlready && membersInHouse > 1 && City.currentCrime > exMinCrime && City.currentChaos > exMinChaos && Random.Range(0, 100) < triggerMax)
        {
            transitionTime();

            for (int n = 3; n > 0; n++)
            {
                if (!Family[n].dead && !Family[n].gone)
                {
                    Family[n].moraleChange(moodChange);
                    houseStats.modMoney(-minMoney);

                    kidnappedAlready = true;

                    enqueuePopUp("When you get home you receive a strange phone call that tells you " + Family[n].firstName + " has been kidnapped. You pay the rescue and a couple hours later they are delivered on a public park with little more than a couple bruises.", pictureUsed);

                    return true;
                }
            }

            return false;
        }
        else
            return false;
    }

    bool desperateKidnapping()
    {
        //Constants
        float exMinChaos = 7;
        float exMinCrime = 6;
        int minMoney = 10000;
        int daysCaptive = 7;
        int moraleDropForEverybody = -15;
        int moraleDropForKidnapped = -15;

        //Semi constant
        float triggerMax = City.districts[3].currentCREvening;

        if (!kidnappedAlready && membersInHouse > 1 && City.currentCrime > exMinCrime && City.currentChaos > exMinChaos && Random.Range(0, 100) < triggerMax)
        {
            transitionTime();

            for (int n = 3; n > 0; n++)
            {
                if (!Family[n].dead && !Family[n].gone)
                {
                    transitionTime();

                    toggleGone(n, "When you get home you receive a strange phone call that tells you " + Family[n].firstName + " has been kidnapped. The kidnapper tells a date and place " + daysCaptive + " days later where you need to leave the money.", true);

                    timeBeforeRescue = daysCaptive;

                    kidnappedMember = n;

                    houseStats.modMoney(-minMoney);

                    foreach (FamilyMembers f in Family)
                        f.moraleChange(moraleDropForEverybody);

                    Family[kidnappedMember].moraleChange(moraleDropForKidnapped);

                    return true;
                }
            }

            return false;
        }
        else
            return false;
    }

    void subKidnappingCounter()
    {
        if (timeBeforeRescue > 0)
        {
            timeBeforeRescue--;
        }
        else if (timeBeforeRescue == 0)
        {
            //Constant
            int percentageChanceOfDoubleCross = 20;

            timeBeforeRescue--;

            if (Random.Range(0, 100) <= percentageChanceOfDoubleCross)
            {
                transitionTime();

                Kill(kidnappedMember, "You leave a pack of bills inside the trash can of a public park and wait in the car for about forty five minutes. \n Then two hours. \n You never see " + Family[kidnappedMember].firstName + " again.");

            }
            else
            {
                transitionTime();

                toggleGone(kidnappedMember, "You leave a pack of bills inside the trash can of a public park and wait in the car for about forty five minutes. " + Family[kidnappedMember].firstName + " shows up running to the car, crying, skinny and covered in bruises. You take them back home.", false);
            }

        }
    }

        
    //Traffic events
    bool blockedRoad()
    {
        //Constants
        float exMinChaos = 2;
        int minTimePassed = 7;
        int moralEffect = -2;

        //Dinamic constants
        float triggerMax = ((City.currentChaos + City.districts[(int)tempActivity.area].traffic) * 10) / 2;
             
        if (currentTime != timeOfDay.evening && City.currentChaos > exMinChaos && daysSinceLastTraffic > minTimePassed && Random.Range(0, 100) < triggerMax)
        {
            transitionTime();

            Family[0].moraleChange(moralEffect);

            daysSinceLastTraffic = 0;

            enqueuePopUp("Apparently a jerk t-boned another car in the main avenue. You'll be stuck in traffic for a while.", 0);

            return true;
        }
        else
        {
            return false;
        }
    }

    bool smallCarAccident()
    {
        //Constants
        float exMinChaos = 5;
        int minTimePassed = 7;
        int moralEffect = -7;
        int moneyLost = -500;

        //Dinamic constants
        float triggerMax = ((City.currentChaos + City.districts[(int)tempActivity.area].traffic) * 5) / 3;

        if (currentTime != timeOfDay.evening && City.currentChaos > exMinChaos && daysSinceLastTraffic > minTimePassed && Random.Range(0, 100) < triggerMax)
        {
            transitionTime();

            Family[0].moraleChange(moralEffect);

            daysSinceLastTraffic = 0;

            enqueuePopUp("A jerk t-boned you in the main avenue. You'll be stuck here for a while until someone tows you away.", 0);

            houseStats.modMoney(moneyLost);

            return true;
        }
        else
        {
            return false;
        }
    }


    //Negligence Events
    bool disservice()
    {
        //Constants
        int minTimeForDisservice = 30;
        int exMinChaos = 2;
        float triggerMax = City.currentChaos * 5;

        if (City.currentChaos > exMinChaos && minTimeForDisservice < daysSinceNegligence && Random.Range(0, 100) < triggerMax)
        {
            //The stuff that is gonna happen

            return true;
        }
        else
            return false;
    }

    #endregion

    //Menus
    public void toMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
