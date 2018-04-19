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
    public Animator timeTransitionAnimator;

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
    [Tooltip("0 = Error\n 1 = fight, 2 = member leaves house, 3 = suicide, 4 = murder, 5 = asylum, 6 = general death, 7 = information\n 8 = sick alert, 9 = item alert, 10 = services alert\n11 to 25 = Events\n26 to 100 = Activities")]
    public Sprite[] popUpIllustrations;
    public Sprite[] newsIllustrations;

    [Header("Game Over")]
    public Transform gameOverScreen;

    [Header("References")]
    public phoneScript Phone;
    public HouseScript House;
    public ActivityMenuScript Activities;
    public soundScript audioManager;

    [Header("Data to Pass")]
    public FamilyMembers[] Family = new FamilyMembers[Constants.familySize];
    public houseClass houseStats;
    public cityClass City;
    public bool houseNotification = false;
    public bool phoneNotification = false;
    public int costOfServices = 100;
    public ActivityClass servicePayActivity;
    public int membersAlive = Constants.familySize;
    public int membersInHouse = Constants.familySize;
    public bool phoneOpen = false;
    public bool houseOpen = false;

    [Header("Files")]
    public TextAsset activityFile;
    public TextAsset statChangesFile;
    public TextAsset districtStatsFile;
    public TextAsset sicknessFile;
    public TextAsset storesFile;
    public TextAsset pricesFile;
    public TextAsset newsFile;
    public TextAsset wikiFile;
    public TextAsset decisionEventsFile;

    [Header("Day Stuff")]
    public DayClass currentDay;
    public timeOfDay currentTime;
    public SpriteRenderer map;
    public Color[] timeColors = new Color[3];
    int daysSinceLastIllnessCheck = 0;
    int[] daysLeftForHealing = { 0, 0, 0, 0 };
    int pendingTimeTransitions = 0;

    [Header("Events and Such")]
    public Animator loadingBarAnimator;
    public AnimationClip loadingInAn;
    public AnimationClip fillingAn;
    public bool currentlyExecutingActivity = false;
    public Transform decisionEventWindow;
    eventHandler Events;

    [Header("Sounds")]
    public AudioClip loadingSound;
    public AudioClip misstepSound;

    [Header("Permanent Stuff")]
    public bool dadStartsSick = false;
    public bool createSessionAtStart = false;
    public bool savingEnabled = true;
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
    List<decisionEventClass> decisionEvents = new List<decisionEventClass>();

    ////Tools for activity position
    //int selectedActivity = 0;
    //List<string> actNames;

    public ActivityClass tempActivity;
    bool GAME_OVER = false;
    bool GAME_WON = false;

    void Start()
    {
        Resources.UnloadUnusedAssets();

        //Debug.Log(Time.time + "At start of start");

        //if (createSessionAtStart)
        //    newSession();
        //else
        //    loadData();

        newGameClass loadObject = new newGameClass();

        if (savingEnabled)
        {
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
        }
        else
        {
            newSession(null);
        }

        //to here

        readWikiFile();

        readDecisions();

        updateComponents();

        initializeServicePayActivity();

        //Debug
        if (dadStartsSick)
            Family[0].getsSick(sicknesses[5]);

        if (notificationQueue.Count > 0)
            openPopUp(notificationQueue.Peek());

        //TODO REMOVE THIS AND REPLACE WITH SOMETHING BETTER    
        //map.color = timeColors[(int)currentTime];
        timeTransitionAnimator.SetInteger("TimeOfDay", ((int)currentTime + 1));

        //audioManager.StartAudio();

        /*string []list = activityFile.text.Split('\n');
        actNames = new List<string>();
        foreach(string s in list)
        {
            actNames.Add(s.Split('\t')[0]);
        }*/

        //Debug.Log(Time.time + "At end of start");

        //Debug.Log(News[0].title);

        //Resources.UnloadUnusedAssets();
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Debug.Log(actNames[selectedActivity] + " Pos: " + map.transform.InverseTransformPoint(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition)));
    //    }

    //    if (Input.GetKeyDown(KeyCode.DownArrow))
    //    {
    //        if (selectedActivity < actNames.Count - 1)
    //            selectedActivity++;
    //        Debug.Log("Selected Activity: " + actNames[selectedActivity]); 
    //    }
    //    else if (Input.GetKeyDown(KeyCode.UpArrow))
    //    {
    //        if (selectedActivity > 0)
    //            selectedActivity--;

    //        Debug.Log("Selected Activity: " + actNames[selectedActivity]);
    //    }
    //}

    #region Time Progression
    //Time stuff
    public void transitionTime()
    {
        if (pendingTimeTransitions > 0)
        {
            pendingTimeTransitions--;

            if (currentTime == timeOfDay.evening)
            {
                dayAdvance();
            }
            else
            {
                currentTime += 1;
            }

            if (GAME_OVER)
            {
                if (savingEnabled)
                    SaveLoad.Delete();
            }
            else
            {
                saveData();

                if (currentDay.month == Constants.endDate.month && currentDay.day == Constants.endDate.day)
                {
                    GAME_WON = true;
                    gameWon();
                }
                else
                {
                    timeTransitionAnimator.SetInteger("TimeOfDay", ((int)currentTime + 1));
                    transitionTime();
                }
            }
        }


        updateComponents();
    }

    public void dayAdvance()
    {
        currentDay.advanceDay();
        houseStats.timeLeftForPayment--;
        currentTime = timeOfDay.morning;
        daysSinceLastIllnessCheck++;

        Events.dayAdvance(); //Advance the day within the event handler
        Debug.Log(currentDay.calculateDayOfWeek().ToString());
        if (currentDay.calculateDayOfWeek() == whenAreStoresResupplied && Random.Range(1, City.currentChaos + 1) == 1)
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
                enqueuePopUp("You ran out of cleaning items!", 9);
            }
        }

        if (houseStats.getHygiene() > 0 && (int)currentDay.calculateDayOfWeek() == 6)
        {
            houseStats.modHygiene(-1 * membersAlive);

            if (houseStats.getHygiene() <= 0)
            {
                houseStats.setHygiene(0);
                enqueuePopUp("You ran out of hygiene items!", 9);
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

        Debug.Log("Crime: " + City.currentCrime + "\nChaos: " + City.currentChaos + "\nFilth: " + City.currentFilth);

        saveData();

        if (notificationQueue.Count > 0)
            openPopUp(notificationQueue.Peek());
    }

    public void updateComponents()
    {
        Phone.SendMessage("updatePhone");
        House.SendMessage("closeHouseMenu");
        House.SendMessage("familyUpdate", Family);
        Activities.SendMessage("updateMenu");
        Events.decisionEvents = decisionEvents;
    }

    void storeSupplying()
    {
        int[] inflatedPrices = new int[4];

        for (int n = 0; n < 4; n++)
        {
            //Debug.Log((float)itemPrices[n] * City.currentInflation);
            inflatedPrices[n] = Mathf.RoundToInt((float)itemPrices[n] * City.currentInflation);
            Debug.Log(n + " price : " + inflatedPrices[n].ToString());
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
                foreach (sicknessClass s in sicknesses)
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
                    enqueuePopUp((Family[chosenFamilyMember].firstName + " hasn't been feeling well lately. Maybe you should check on " + Family[chosenFamilyMember].sex.ToString() + "."), 8);
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
                if (houseStats.getFoodQ() > 0)
                {
                    if (houseStats.getFoodQ() >= Family[n].food)
                    {
                        foodConsumed = Family[n].food;
                    }
                    else
                    {
                        foodConsumed = houseStats.getFoodQ();
                    }
                }
                else
                {
                    foodConsumed = 0;
                    enqueuePopUp("<color=red> " + Family[n].firstName + " did not eat yesterday, " + ((Family[n].sex == FamilyMembers.gender.him) ? "his" : "her") + " health will deteriorate as a result. </color>", 27);

                }


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

                        //Family[n].food = houseStats.getFoodQ();

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
                    //Family[n].food = 0;
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

                if (n == 0 && Family[n].psyche != FamilyMembers.emotionalHealth.Healthy)
                {
                    if (Family[n].psyche == FamilyMembers.emotionalHealth.Unstable)
                    {
                        switch (Random.Range(1, 10))
                        {
                            case 1:
                                enqueuePopUp("You find yourself screaming in the shower every morning.", 7);
                                break;
                            case 2:
                                enqueuePopUp("You don't try to avoid a stray dog on the road.", 7);
                                break;
                            case 3:
                                enqueuePopUp("You had a dream where you kill someone.", 7);
                                break;
                            case 4:
                                enqueuePopUp("You cried yourself to sleep last night.", 7);
                                break;
                            case 5:
                                enqueuePopUp("You find yourself screaming in the shower this morning.", 7);
                                break;
                            case 6:
                                enqueuePopUp("You almost hit " + (((!Family[1].gone && !Family[1].dead) || (!Family[2].gone && !Family[2].dead) || (!Family[3].gone && !Family[3].dead)) ? " your family " : "someone") + " this morning.", 7);
                                break;
                            case 7:
                                enqueuePopUp("You didn't sleep last night.", 7);
                                break;
                            default:
                                enqueuePopUp("You are not feeling well.", 7);
                                break;
                        }
                    }
                    else if (Family[n].psyche == FamilyMembers.emotionalHealth.Depressed)
                    {
                        switch (Random.Range(1, 10))
                        {
                            case 1:
                                enqueuePopUp("You are not feeling hungry this morning.", 7);
                                break;
                            case 2:
                                enqueuePopUp("You are always sleepy.", 7);
                                break;
                            case 3:
                                enqueuePopUp("You had a nightmare where you kill someone.", 7);
                                break;
                            case 4:
                                enqueuePopUp("You cried last night.", 7);
                                break;
                            case 5:
                                enqueuePopUp("You find yourself standing motionless in the shower for hours this morning.", 7);
                                break;
                            case 6:
                                enqueuePopUp("You really want a beer right now.", 7);
                                break;
                            case 7:
                                enqueuePopUp("Why even bother?", 7);
                                break;
                            default:
                                enqueuePopUp("You might need some rest.", 7);
                                break;
                        }
                    }
                }

                if(temp > 0)
                //if (n != 0 && membersInHouse > 1) //This was so it could never happen to the father
                {
                    if (temp == 1 && n != 0) //The member of the family gets in a fight with other characters. 50% chance.
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
                    else if (temp == 2 && n != 0) //There is a 25% chance the character leaves the house.
                    {
                        enqueuePopUp("During the night, when everyone was sleeping, " + Family[n].firstName + " secretly packed all of " + ((Family[n].sex == FamilyMembers.gender.him) ? "his" : "her") + " things and left the house.", 2, Color.yellow);
                        missingLevel += Family[n].leavesTheHouse();
                    }
                    else if (temp == 3) //There is a 10% chance the character kills themselves
                    {
                        enqueuePopUp(Family[n].firstName + " has commited suicide.", 3, Color.red);
                        mourningLevel += Family[n].dies();
                        missingLevel += 1;

                        if (n == 0)
                        {
                            GAME_OVER = true;
                        }

                        Family[n].deathCause = "Suicide";
                    }
                    else if (temp == 4 || temp == 5) //There is a 5% chance they kill someone else
                    {
                        Debug.Log("Murder happening");
                        if (membersInHouse >= 3) //If there are at least 2 people in the house
                        {
                            int temp2;

                            do
                            {
                                temp2 = Random.Range(0, 3);
                            } while (Family[temp2].dead || Family[temp2].gone || temp2 == n); //We make sure the selected member isnt dead or gone or is the character we are currently looking at

                            enqueuePopUp("During a particularly heated discussion, " + Family[temp2].firstName + " is killed by " + Family[n].firstName + ".", 4, Color.red);
                            mourningLevel += Family[temp2].dies(); //The selected character dies
                            missingLevel += 1;

                            if (temp2 == 0)
                            {
                                GAME_OVER = true;
                            }


                            Family[temp2].deathCause = "Murder";
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
                            enqueuePopUp("Moved by guilt and sorrow over the death of the child, " + Family[n].firstName + " commits suicide.", 3, Color.red);
                            mourningLevel += Family[n].dies(); //If a character is the mother, they kill themselves
                            missingLevel += 1;

                            if (n == 0)
                                GAME_OVER = true;

                            Family[n].deathCause = "Suicide";
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
                        enqueuePopUp(Family[n].firstName + " wakes up dead that morning. #n The cause of death is <i> " + Family[n].status.name + " </i> .", 6, Color.red);
                        Family[n].deathCause = Family[n].status.name;

                        if (n == 0)
                            GAME_OVER = true;
                    }
                    else
                    {
                        enqueuePopUp(Family[n].firstName + " didn't wake up that morning. #n The cause of death is starvation.", 6, Color.red);
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
        House.closeHouseMenu();
        Phone.closePhone();
        currentlyExecutingActivity = true;
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.SendMessage("setFamily", Family);
        gameOverScreen.SendMessage("setDay", currentDay);
        gameOverScreen.SendMessage("displayVictory");
        audioManager.SendMessage("stopEverything");
    }
    #endregion

    #region Activity Execution
    //Activity and stuff
    public void executeActivity(ActivityClass activity)
    {
        if (!currentlyExecutingActivity)
        {
            tempActivity = activity;
            currentlyExecutingActivity = true;

            House.closeHouseMenu();

            if (!activity.isItShop && !activity.paysService)
            {
                houseStats.modMoney(-activity.cost);

                for (int n = 0; n < Constants.familySize; n++)
                {
                    Family[n].moraleChange(activity.moraleChange[n]);
                }

                finishActivityAndCheckEvents();

//                enqueuePopUp(activity);
            }
            else if (activity.paysService)
            {
               // enqueuePopUp(activity);
                houseStats.modMoney(-activity.cost);
                houseStats.servicesPaid = true;

                finishActivityAndCheckEvents();
            }
            else if (activity.isItShop)
            {
                //enqueuePopUp(activity);
                changeScreen(1, activity.shopAttached);
            }
        }
    }

    void finishActivityAndCheckEvents()
    {
        bool eventDetected = Events.eventCheck(); //This one will get the bool from the event function
        
        StartCoroutine(pauseOrNotPause(eventDetected));

        //Call animation that starts loading bar
        //That animation then calls a function that checks for the events and stops the loading if something happens
        //The loading out animation transitions time
        //TODO include a bit that makes sure the animator is always enabled when the popup function runs out of elements in the queue
    }

    IEnumerator pauseOrNotPause(bool eventDetected)
    {
        togglePauseLoadingAnimator(false);
        audioManager.playSFX(loadingSound);

        if (eventDetected)
        {
            float pauseTime = loadingInAn.length + (fillingAn.length / 2);
            loadingBarAnimator.Play("loadingIn");
            yield return new WaitForSeconds(pauseTime);
            togglePauseLoadingAnimator(true);
            audioManager.playSFX(misstepSound);
        }
        else
        {
            float pauseTime = loadingInAn.length + fillingAn.length;
            loadingBarAnimator.Play("loadingIn");
            yield return new WaitForSeconds(pauseTime);
            audioManager.stopSFX();
        }

        if (notificationQueue.Count == 0 || (notificationQueue.Count > 0 && notificationQueue.Peek().type != notificationType.Decision))
            concludeActivity();
        else
            openPopUp(notificationQueue.Peek());
    }

    void togglePauseLoadingAnimator(bool pause)
    {
        loadingBarAnimator.enabled = !pause;
    }

    void resetLoadingBarValues()
    {
        togglePauseLoadingAnimator(false);
        loadingBarAnimator.Play("Hidden");
    }

    public void finishShopping()
    {
        changeScreen(0);
        finishActivityAndCheckEvents();
    }

    public void concludeActivity()
    {
        currentlyExecutingActivity = false;
        
        enqueuePopUp(tempActivity);

        addTimeTransition();

        if (notificationQueue.Count > 0 && notificationQueue.Peek() != null)
            openPopUp(notificationQueue.Peek());
        //transitionTime();
    }

    public void addTimeTransition()
    {
        pendingTimeTransitions++;
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
        servicePayActivity.postActivityDescription = "You wasted your time in a line to pay the house services.";
        servicePayActivity.pictureNumberUsed = 30;
        servicePayActivity.paysService = true;
    }

    public void Kill(int famMember, string message, int pictureUsed)
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

            enqueuePopUp(message, pictureUsed);

            if (famMember == 0)
                GAME_OVER = true;
        }
    }

    public void toggleGone(int famMember, string message, bool inOUT, int pictureUsed)
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

                enqueuePopUp(message, pictureUsed);
            }
            else if (inOUT && !Family[famMember].gone)
            {
                Family[famMember].comesBack();
                membersInHouse++;

                foreach (FamilyMembers f in Family)
                {
                    f.missingAdd(-1);
                }

                enqueuePopUp(message, pictureUsed);
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

    public void enqueuePopUp(int decisionID)
    {
        notificationQueue.Enqueue(new notificationClass(decisionID));
    }

    public void openPopUp(notificationClass not)
    {
        if (not.type == notificationType.Decision)
        {
            popUpWindow.gameObject.SetActive(false);
            decisionEventWindow.gameObject.SetActive(true);
            decisionEventWindow.SendMessage("displayDecision", Events.getDecEvt(int.Parse(not.text)));
        }
        else
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
    }

    public void closePopUp()
    {
        notificationQueue.Dequeue();
        notificationClass temp;

        popUpWindow.gameObject.SetActive(false);
        decisionEventWindow.gameObject.SetActive(false);

        if (notificationQueue.Count > 0 && notificationQueue.Peek() != null)
        { 
        temp = notificationQueue.Peek();
        openPopUp(temp);
        }
        else if (GAME_OVER)
        {
            gameOver();
        }
        else if (GAME_WON)
        {
            gameWon();
        }
        else
        {
            transitionTime();
            resetLoadingBarValues();
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
                            return City.districts[Mathf.Clamp(int.Parse(str[2].ToString()) - 1, 0, City.districts.Length - 1)].districtName;
                        else
                            return City.districts[Mathf.Clamp((int)tempActivity.area, 0, City.districts.Length - 1)].districtName;
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
                    if (str.Length > 2 && str[2] == 'g')
                        return Family[0].sex.ToString();
                    else
                        return Family[0].firstName;
                case '1':
                    if (str.Length > 2 && str[2] == 'g')
                        return Family[0].sex.ToString();
                    else
                        return Family[1].firstName;
                case '2':
                    if (str.Length > 2 && str[2] == 'g')
                        return Family[0].sex.ToString();
                    else
                        return Family[2].firstName;
                case '3':
                    if (str.Length > 2 && str[2] == 'g')
                        return Family[0].sex.ToString();
                    else
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

        enqueuePopUp("<b> Welcome to Little Venice.</b> #n In this misery simulator, your objective is to keep yourself and your family alive until <b> Dec 31st </b>.", 38);

        enqueuePopUp("You can keep track of the date and time by looking at the phone. #n You may also click on it to read the news, the in-game wiki or change the music. ", 36);

        enqueuePopUp("You can <b>Right Click</b> the house to see or modify your household inventory, <b>Left Click</b> it to see the quick menu or <b>Middle Click</b> it to open the family menu.", 35);

        enqueuePopUp("To pass time and earn resources, click on the <i>Activities</i> bellow.", 37);

        savedObject.getObituaries(Obituaries);
        sortListOfDays(Obituaries);
        
        Events = new eventHandler(this, new eventHandler.variables());

        //foreach (districtClass d in City.districts)
        //    Debug.Log(d.districtName + ": " + d.baseCrimeRateMorning + " - " + d.baseCrimeRateNoon + " - " + d.baseCrimeRateEvening);

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

            //This section puts the location in the object
            string[] coors = numbers[14].Split(',');

            if (coors.Length >= 2)
                tempActivity.setPointerLocation(new float[]{ float.Parse(coors[0]), float.Parse(coors[1]) } );

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
                News.Add(new newsClass(dateForCurrentNew, parts[2], parts[3], int.Parse(parts[4])));
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

    public void readDecisions()
    {
        string[] lines = decisionEventsFile.text.Split('\n');

        foreach (string s in lines)
        {
            decisionEvents.Add(new decisionEventClass(s));
        }
    }

    public void saveData()
    {
        if (savingEnabled)
        {
            savedObject.saveData(City, houseStats, currentDay, currentTime, Family, mornActivities, noonActivities, evenActivities, sicknesses, notificationQueue, itemPrices, stores, Events.vars);
            savedObject.saveObituaries(Obituaries);
            savedObject.saveNews(News);
        }
    }

    public void loadData()
    {
        if (savingEnabled)
        {
            City = new cityClass();
            Events = new eventHandler(this, new eventHandler.variables());
            SaveLoad.Load();
            SaveLoad.savedGame.copyData(City, houseStats, currentDay, Family, mornActivities, noonActivities, evenActivities, sicknesses, notificationQueue, itemPrices, stores, Events.vars);
            currentTime = SaveLoad.savedGame.getSavedTime();
            SaveLoad.savedGame.getObituaries(Obituaries);
            SaveLoad.savedGame.getNews(News);
            //Debug.Log(currentTime);
        }
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


    public void savePlaylist(List<int> from)
    {
        SaveLoad.playlist = from.ToArray();
    }

    public void loadPlaylist(List<int> to)
    {
        SaveLoad.Load();

        for (int n = 0; n < SaveLoad.playlist.Length; n++)
            to.Add(SaveLoad.playlist[n]);
    }

    #endregion

   

    //Menus
    public void toMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
