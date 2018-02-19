using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventHandler{

    GameManager manager;
    public variables vars;
    List<string[]> eventIndex;

    [System.Serializable]
    public class variables
    {
        public string[] robberyEvents = { "pickpocketing()", "discreteMugging()", "violentMugging()", "lethalMug()" };
        public string[] kidnapEvents = { "expressKidnapping()", "desperateKidnapping()" };
        public string[] trafficEvents = { "blockedRoad()", "smallCarAccident()" };
        public string[] negligenceEvents = { "disservice()", "uselessness()" };
        public string[] accidentEvents = { "fireworksAccident()", "flood()", "cousinDeath()", "lostBullet()" };
        public string[] fortuneEvents = { "jerkUncleDeath()", "kidnappedEscapes()" };

        public int daysSinceLastRobbery = 0;

        public bool kidnappedAlready = false;
        public int timeBeforeRescue = -1;
        public int kidnappedMember;
        public int priceOfKidnapping = 300;

        public int daysSinceLastTraffic = 0;

        public int daysSinceNegligence = 0;
        public int daysForServiceFix = -1;

        public int daysSinceLastEvent = 0;

        public variables()
        {
            //List<string> temp = new List<string>();
            //temp.Add("pickpocketing()");
            //temp.Add("discreteMugging()");
            //temp.Add("violentMugging()");
            //temp.Add("lethalMug()");

            //robberyEvents = temp.ToArray();
            //temp.Clear();

            //temp.Add("expressKidnapping()");
            //temp.Add("desperateKidnapping()");

            //kidnapEvents = temp.ToArray();
            //temp.Clear();

            //temp.Add("blockedRoad()");
            //temp.Add("smallCarAccident()");

            //trafficEvents = temp.ToArray();
            //temp.Clear();

            //temp.Add("disservice()");
            //temp.Add("uselessness()");

            //negligenceEvents = temp.ToArray();
            //temp.Clear();

            //temp.Add("fireworksAccident()");
            //temp.Add("flood()");
            //temp.Add("cousinDeath()");
            //temp.Add("lostBullet()");

            //accidentEvents = temp.ToArray();
            //temp.Clear();

            //temp.Add("jerkUncleDeath()");
            //temp.Add("kidnappedEscapes()");

            //fortuneEvents = temp.ToArray();
            //temp.Clear();
            ;
        }
    }



    public eventHandler(GameManager newMan, eventHandler.variables newVars)
    {
        manager = newMan;
        vars = newVars;

        eventIndex = new List<string[]>();

        eventIndex.Add(vars.robberyEvents);
        eventIndex.Add(vars.kidnapEvents);
        eventIndex.Add(vars.trafficEvents);
        eventIndex.Add(vars.negligenceEvents);
        eventIndex.Add(vars.accidentEvents);
        eventIndex.Add(vars.fortuneEvents);
    }


    public bool executeEvent(string name)
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
            case "uselessness()":
                return uselessness();
            case "fireworksAccident()":
                return fireworksAccident();
            case "flood()":
                return flood();
            case "cousinDeath()":
                return cousinDeath();
            case "lostBullet()":
                return lostBullet();
            case "jerkUncleDeath()":
                return jerkUncleDeath();
            case "kidnappedEscapes()":
                return kidnappedEscapes();
            default:
                {
                    //vars.daysSinceLastEvent--;
                    return false;
                }
        }
    }

    public bool eventCheck()
    {
        //Constants
        int rollingChance = 10;


        int RN = Random.Range(0, rollingChance);
    
        if (RN < eventIndex.Count)
        {
            int specificEvent = Random.Range(0, eventIndex[RN].Length - 1);

            //Debug.Log("Event Type: " + RN + "/" + eventIndex.Count + "/" + rollingChance + "\tSpecific: " + specificEvent + "/" + (eventIndex[RN].Length - 1) + " (" + eventIndex[RN][specificEvent] + ")");

            return executeEvent(eventIndex[RN][specificEvent]);
        }
        else
        {
            return false;
        }
    }
    
    public void dayAdvance()
    {
        vars.daysSinceLastRobbery++;

        vars.daysSinceLastTraffic++;

        vars.daysSinceNegligence++;

        if (vars.daysForServiceFix > 0)
        {
            vars.daysForServiceFix--;
        }

        subKidnappingCounter();

        vars.daysSinceLastEvent++;
    }

    void subKidnappingCounter()
    {
        if (vars.timeBeforeRescue > 0)
        {
            vars.timeBeforeRescue--;
        }
        else if (vars.timeBeforeRescue == 0)
        {
            //Constant
            int percentageChanceOfDoubleCross = 20;
            int pictureUsedForRescue = 25;
            int pictureUsedForBetrayal = 26;

            vars.timeBeforeRescue--;

            if (Random.Range(0, 100) <= percentageChanceOfDoubleCross)
            {
                manager.addTimeTransition();

                manager.Kill(vars.kidnappedMember, "You leave a pack of bills in the place and wait for about forty five minutes.", pictureUsedForBetrayal);
                manager.enqueuePopUp("#n Then two hours. ", pictureUsedForBetrayal);
                manager.enqueuePopUp("<color=red> You never see " + manager.Family[vars.kidnappedMember].firstName + " again. </color>", pictureUsedForBetrayal);

                vars.kidnappedMember = -1;
            }
            else
            {
                manager.addTimeTransition();

                manager.toggleGone(vars.kidnappedMember, "You leave a pack of bills in the place and wait in the car for about forty five minutes. ", false, pictureUsedForRescue);
                manager.enqueuePopUp(manager.Family[vars.kidnappedMember].firstName + " shows up running, crying, skinny and covered in bruises.", pictureUsedForRescue);
                manager.enqueuePopUp("You take " + manager.Family[vars.kidnappedMember].sex.ToString() + " back home.", pictureUsedForRescue);

                vars.kidnappedMember = -1;
            }

        }
    }



    //Robbery Events
    bool pickpocketing()
    {
        //Constant vars
        int moraleEffect = -5;
        int minMoneyLost = 10;
        int maxMoneyLost = 500;
        int pictureUsed = 11;
        int minTimePassed = 7;

        float currentDistrictCrime = 0;

        switch ((int)manager.currentTime)
        {
            case 0:
                currentDistrictCrime = manager.City.districts[(int)manager.tempActivity.area].currentCRMorning;
                break;
            case 1:
                currentDistrictCrime = manager.City.districts[(int)manager.tempActivity.area].currentCRNoon;
                break;
            case 2:
                currentDistrictCrime = manager.City.districts[(int)manager.tempActivity.area].currentCREvening;
                break;
            default:
                currentDistrictCrime = 0;
                break;
        }
        float triggerMax = (currentDistrictCrime * manager.City.currentCrime / 2);

        Debug.Log("pickpocketing being tested: \nTrigger: " + triggerMax + "/100   " + minTimePassed + "<?" + vars.daysSinceLastRobbery + "()" + (minTimePassed < vars.daysSinceLastRobbery));

        if (Random.Range(0, 100) < triggerMax && minTimePassed < vars.daysSinceLastRobbery)
        {
            string descriptionText = "";

            //Effects on money
            int a = manager.houseStats.getMoney();
            manager.houseStats.modMoney(-Random.Range(minMoneyLost, maxMoneyLost));
            int b = manager.houseStats.getMoney();

            manager.Family[0].moraleChange(moraleEffect);
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

            vars.daysSinceLastRobbery = -1;

            manager.enqueuePopUp(descriptionText, pictureUsed);

            return true;
        }
        else
        {
            return false;
        }
    }

    bool discreteMugging()
    {
        //Constant vars
        int moraleEffect = -10;
        int minMoneyLost = 10;
        int maxMoneyLost = 500;
        int pictureUsed = 12;
        int minTimePassed = 7;

        //Semi Constant vars
        bool isSpecificSector = (manager.tempActivity.area == ActivityClass.sector.B || manager.tempActivity.area == ActivityClass.sector.C || manager.tempActivity.area == ActivityClass.sector.E || manager.tempActivity.area == ActivityClass.sector.F);
        bool isSpecificTime = (manager.currentTime == timeOfDay.evening);
        float triggerMax = (manager.City.districts[(int)manager.tempActivity.area].baseCrimeRateEvening) * manager.City.currentCrime;

        Debug.Log("discreteMugging being tested: \nTrigger: " + triggerMax + "/100 \n " + minTimePassed + "/" + vars.daysSinceLastRobbery + "  " + isSpecificTime/* + "  " + isSpecificSector*/);

        if (minTimePassed < vars.daysSinceLastRobbery && isSpecificTime && /*isSpecificSector &&*/ Random.Range(0, 100) < triggerMax)
        {
            string descriptionText = "";

            //Effects on money
            int a = manager.houseStats.getMoney();
            manager.houseStats.modMoney(-Random.Range(minMoneyLost, maxMoneyLost));
            int b = manager.houseStats.getMoney();

            //Morale effect and text variants
            if (manager.tempActivity.activityCategory == ActivityClass.category.Family)
            {
                foreach (FamilyMembers f in manager.Family)
                {
                    f.moraleChange(moraleEffect);
                }

                descriptionText += "On the way to the car, a guy with a gun jumps on you and your family and assaults you. ";
            }
            else
            {
                manager.Family[0].moraleChange(moraleEffect);
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

            vars.daysSinceLastRobbery = -1;
            manager.enqueuePopUp(descriptionText, pictureUsed);

            return true;
        }
        else
        {
            return false;
        }
    }

    bool violentMugging()
    {
        //Constant vars
        int moraleEffect = -15;
        int minMoneyLost = 10;
        int maxMoneyLost = 500;
        int pictureUsed = 13;
        int pictureUsedForDeath = 26;
        int minTimePassed = 7;
        float exMinChaos = 4;

        //Semi Constant vars
        float currentDistrictCrime = 0;

        switch ((int)manager.currentTime)
        {
            case 0:
                currentDistrictCrime = manager.City.districts[(int)manager.tempActivity.area].currentCRMorning;
                break;
            case 1:
                currentDistrictCrime = manager.City.districts[(int)manager.tempActivity.area].currentCRNoon;
                break;
            case 2:
                currentDistrictCrime = manager.City.districts[(int)manager.tempActivity.area].currentCREvening;
                break;
            default:
                currentDistrictCrime = 0;
                break;
        }
        float triggerMax = (currentDistrictCrime * manager.City.currentCrime);

        Debug.Log("violentMug being tested: \nTrigger: " + triggerMax + "/100");

        if (manager.City.currentChaos > exMinChaos && minTimePassed < vars.daysSinceLastRobbery && Random.Range(0, 100) < triggerMax)
        {
            string descriptionText = "";

            //Effects on money
            int a = manager.houseStats.getMoney();
            manager.houseStats.modMoney(-Random.Range(minMoneyLost, maxMoneyLost));
            int b = manager.houseStats.getMoney();

            //Morale effect and text variants
            if (manager.tempActivity.activityCategory == ActivityClass.category.Family)
            {
                foreach (FamilyMembers f in manager.Family)
                {
                    f.moraleChange(moraleEffect);
                }

                descriptionText += "On the way to the car, a guy with a gun jumps on you and your family and assaults you. ";
            }
            else
            {
                manager.Family[0].moraleChange(moraleEffect);
                descriptionText += "On the way to the car, a guy with a gun jumps on you and assaults you. ";
            }

            //Money text variants
            if (a - b <= (maxMoneyLost - minMoneyLost) / 3)
            {
                if (manager.tempActivity.activityCategory == ActivityClass.category.Family)
                {
                    if (manager.membersAlive > 1 && manager.membersInHouse > 1)
                    {
                        for (int n = 1; n < 4; n++)
                        {
                            if (!manager.Family[n].dead && !manager.Family[n].gone)
                            {
                                descriptionText += "Because you had so little money, the mugger gets angry and shot " + manager.Family[n].firstName + " in the chest. ";

                                if (Random.Range(0, 4) > 2)
                                {
                                    descriptionText += "Fortunately, you manage to take " + manager.Family[n].sex.ToString() + " to the emergency room of the hospital and they manage to save " + manager.Family[n].firstName + "'s life.";

                                    foreach (FamilyMembers f in manager.Family)
                                        f.moraleChange(moraleEffect);
                                }
                                else
                                {
                                    manager.Kill(n, "The bullet pierces the lungs. " + manager.Family[n].firstName + " dies in your arms drowned in " + ((manager.Family[n].sex == FamilyMembers.gender.him) ? "his" : "her") + " own blood.", pictureUsedForDeath);

                                    foreach (FamilyMembers f in manager.Family)
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

            vars.daysSinceLastRobbery = -1;
            manager.enqueuePopUp(descriptionText, pictureUsed);

            return true;
        }
        else
        {
            return false;
        }

    }

    bool lethalMug()
    {
        //Constant vars
        int moraleEffect = -15;
        int minMoneyLost = 10;
        int maxMoneyLost = 500;
        int pictureUsed = 13;
        int pictureUsedForDeath = 14;
        int minTimePassed = 7;
        float exMinChaos = 7;
        float exMinCrime = 6;
        int maxSpareMoney = 100;

        //Semi Constant vars
        float currentDistrictCrime = 0;

        switch ((int)manager.currentTime)
        {
            case 0:
                currentDistrictCrime = manager.City.districts[(int)manager.tempActivity.area].currentCRMorning;
                break;
            case 1:
                currentDistrictCrime = manager.City.districts[(int)manager.tempActivity.area].currentCRNoon;
                break;
            case 2:
                currentDistrictCrime = manager.City.districts[(int)manager.tempActivity.area].currentCREvening;
                break;
            default:
                currentDistrictCrime = 0;
                break;
        }
        float triggerMax = (currentDistrictCrime);

        Debug.Log("lethalMug being tested: \nTrigger: " + triggerMax + "/100");

        if (manager.City.currentChaos > exMinChaos && manager.City.currentCrime > exMinCrime && minTimePassed < vars.daysSinceLastRobbery && Random.Range(0, 100) < triggerMax)
        {
            string descriptionText = "";

            //Effects on money
            int a = manager.houseStats.getMoney();
            manager.houseStats.modMoney(-Random.Range(minMoneyLost, maxMoneyLost));
            int b = manager.houseStats.getMoney();

            //Morale effect and text variants
            manager.Family[0].moraleChange(moraleEffect);
            descriptionText += "On the way to the car, a guy with a gun jumps on you and assaults you. ";

            //Money text variants
            if (a - b < maxSpareMoney)
            {
                descriptionText += "The mugger takes your wallet and notices you don't even have " + maxSpareMoney + " on you.";
                manager.Kill(0, "You see a flash of light and fall dead on the floor.", pictureUsedForDeath);

            }
            else
            {
                descriptionText += "You are robbed from this week's earnings.";
                manager.enqueuePopUp(descriptionText, pictureUsed);
            }
            vars.daysSinceLastRobbery = -1;

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
        int moodChange = -15;
        int pictureUsed = 15;

        //Semi constant
        float triggerMax = manager.City.districts[3].currentCREvening;

        Debug.Log("expKid being tested: \nTrigger: " + triggerMax + "/100");

        if (!vars.kidnappedAlready && manager.membersInHouse > 1 && manager.City.currentCrime > exMinCrime && manager.City.currentChaos > exMinChaos && Random.Range(0, 100) < triggerMax)
        {
            manager.addTimeTransition();

            for (int n = 3; n > 0; n++)
            {
                if (!manager.Family[n].dead && !manager.Family[n].gone)
                {
                    manager.Family[n].moraleChange(moodChange);
                    manager.houseStats.modMoney(-vars.priceOfKidnapping);

                    vars.kidnappedAlready = true;

                    manager.enqueuePopUp("When you get home you receive a strange phone call that tells you " + manager.Family[n].firstName + " has been kidnapped.", pictureUsed);

                    manager.enqueuePopUp("You pay the rescue and a couple hours later " + ((manager.Family[n].sex == FamilyMembers.gender.him) ? "he" : "she") + " is delivered on a public park with little more than a couple bruises.", pictureUsed);

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
        int daysCaptive = 7;
        int moraleDropForEverybody = -15;
        int moraleDropForKidnapped = -30;
        int pictureUsed = 16;

        //Semi constant
        float triggerMax = manager.City.districts[3].currentCREvening;

        Debug.Log("desperateKid being tested: \nTrigger: " + triggerMax + "/100");

        if (!vars.kidnappedAlready && manager.membersInHouse > 1 && manager.City.currentCrime > exMinCrime && manager.City.currentChaos > exMinChaos && Random.Range(0, 100) < triggerMax)
        {
            manager.addTimeTransition();

            for (int n = 3; n > 0; n++)
            {
                if (!manager.Family[n].dead && !manager.Family[n].gone)
                {
                    manager.addTimeTransition();

                    manager.toggleGone(n, "When you get home you receive a strange phone call that tells you " + manager.Family[n].firstName + " has been kidnapped.", true, pictureUsed);

                    manager.enqueuePopUp("The kidnapper tells a date and place where you need to drop the money " + daysCaptive + " days later.", pictureUsed);

                    vars.timeBeforeRescue = daysCaptive;

                    vars.kidnappedMember = n;

                    manager.houseStats.modMoney(-vars.priceOfKidnapping);

                    foreach (FamilyMembers f in manager.Family)
                        f.moraleChange(moraleDropForEverybody);

                    manager.Family[vars.kidnappedMember].moraleChange(moraleDropForKidnapped);

                    return true;
                }
            }

            return false;
        }
        else
            return false;
    }


    //Traffic events
    bool blockedRoad()
    {
        //Constants
        float exMinChaos = 2;
        int minTimePassed = 7;
        int moralEffect = -2;
        int pictureUsed = 17;

        //Dinamic constants
        float triggerMax = ((manager.City.currentChaos + manager.City.districts[(int)manager.tempActivity.area].traffic) * 10) / 2;

        Debug.Log("blockedRoad being tested: \nTrigger: " + triggerMax + "/100");

        if (manager.currentTime != timeOfDay.evening && manager.City.currentChaos > exMinChaos && vars.daysSinceLastTraffic > minTimePassed && Random.Range(0, 100) < triggerMax)
        {
            manager.addTimeTransition();

            manager.Family[0].moraleChange(moralEffect);

            vars.daysSinceLastTraffic = -1;

            manager.enqueuePopUp("Apparently there was a crash in the main avenue. You'll be here for a while.", pictureUsed);

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
        int pictureUsed = 18;

        //Dinamic constants
        float triggerMax = ((manager.City.currentChaos + manager.City.districts[(int)manager.tempActivity.area].traffic) * 5) / 3;

        Debug.Log("smallCarAccident being tested: \nTrigger: " + triggerMax + "/100");

        if (manager.currentTime != timeOfDay.evening && manager.City.currentChaos > exMinChaos && vars.daysSinceLastTraffic > minTimePassed && Random.Range(0, 100) < triggerMax)
        {
            manager.addTimeTransition();

            manager.Family[0].moraleChange(moralEffect);

            vars.daysSinceLastTraffic = -1;

            manager.enqueuePopUp("A jerk t-boned you in the main avenue. You'll be stuck here for a while.", pictureUsed);

            manager.houseStats.modMoney(moneyLost);

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
        int duration = 1;
        timeOfDay requiredTime = timeOfDay.morning;
        int pictureUsed = 19;

        float triggerMax = manager.City.currentChaos * 5;

        Debug.Log("disservice being tested: \nTrigger: " + triggerMax + "/100");

        if (manager.currentTime == requiredTime && manager.City.currentChaos > exMinChaos && minTimeForDisservice < vars.daysSinceNegligence && Random.Range(0, 100) < triggerMax)
        {
            manager.enqueuePopUp("Due to incompetence, you are left a day without water or electricity.", pictureUsed);

            vars.daysForServiceFix = duration;

            return true;
        }
        else
            return false;
    }

    bool uselessness()
    {
        //Constants
        int minTimeForDisservice = 30;
        int exMinChaos = 5;
        int duration = 7;
        timeOfDay requiredTime = timeOfDay.morning;
        int pictureUsed = 19;

        float triggerMax = manager.City.currentChaos * 3;

        Debug.Log("uselessness being tested: \nTrigger: " + triggerMax + "/100");

        if (manager.currentTime == requiredTime && manager.City.currentChaos > exMinChaos && minTimeForDisservice < vars.daysSinceNegligence && Random.Range(0, 100) < triggerMax)
        {
            manager.enqueuePopUp("You wake up with no water or electricity. You don't think it will be solved soon.", pictureUsed);

            vars.daysForServiceFix = duration;

            return true;
        }
        else
            return false;
    }


    //Accident events
    bool fireworksAccident()
    {
        DayClass temp = manager.currentDay;

        //semi-constant
        bool correctDayAndTime = (temp.month == 12 && (temp.day == 24 || temp.day == 31) && manager.currentTime == timeOfDay.evening);
        bool correctActivity = (manager.tempActivity.activityCategory != ActivityClass.category.Family);
        float randomNum = Random.Range(0, 50);
        int indexA = 10, indexB = 35;
        int pictureUsed = 20;

        if (manager.membersAlive > 1 && manager.membersInHouse > 1 && correctDayAndTime && correctActivity && randomNum >= indexA && randomNum <= indexB)
        {
            manager.enqueuePopUp("You decide to spend the holiday on your own. You hear the fireworks in the distance as the night comes to an end.", pictureUsed);
            manager.enqueuePopUp("When you get back home you find yourself in front of a house burning to the ground with your family inside. The firefighters never arrived.", pictureUsed);

            for (int n = 1; n < 4; n++)
                manager.Kill(n, manager.Family[n].firstName + " died in the fire.", pictureUsed);

            return true;
        }
        else
            return false;
        
    }

    bool flood()
    {
        //Constants
        int exMinChaos = 6;
        int minTimeSinceLastEvent = 20;
        float triggerMax = 9;
        int moneyLost = -100;
        int moraleDrop = -10;
        int pictureUsed = 21;

        Debug.Log("flood being tested: \nTrigger: " + triggerMax + "/100");

        if (exMinChaos > manager.City.currentChaos && vars.daysSinceLastEvent > minTimeSinceLastEvent && Random.Range(0, 100) < triggerMax)
        {
            manager.houseStats.modMoney(moneyLost);

            foreach (FamilyMembers f in manager.Family)
            {
                f.moraleChange(moraleDrop);
            }

            manager.enqueuePopUp("On the way back it suddenly starts raining hard. The whole house is flooded after a nearby drain got clogged.", pictureUsed);

            return true;
        }
        else
            return false;
    }

    bool cousinDeath()
    {
        //Constants
        float exMinCrime = 6;
        int minMorale = 60;
        int moraleChange = -5;
        float triggerMax = 50;
        int pictureUsed = 22;

        //Semi constant
        bool correctTime = (manager.currentTime == timeOfDay.morning);

        Debug.Log("cousinDeath being tested: \nTrigger: " + triggerMax + "/100\n " + correctTime + " " + (manager.City.currentCrime > exMinCrime));

        if (correctTime && manager.City.currentCrime > exMinCrime && Random.Range(0, 100) < triggerMax)
        {
            foreach (FamilyMembers f in manager.Family)
            {
                if (f.morale < minMorale)
                    return false;
                else
                    f.moraleChange(moraleChange);
            }

            manager.addTimeTransition();
            manager.addTimeTransition();

            manager.enqueuePopUp("You receive a call from one of your multiple uncles as soon as you wake up.", pictureUsed);
            manager.enqueuePopUp("Apparently one of your cousins was shot dead after someone tried to mug him. You tell #f of the funeral.", pictureUsed);

            return true;
        }
        else
            return false;
    }

    bool lostBullet()
    {
        //Constants
        float exMinChaos = 5;
        float exMinCrime = 5;
        int triggerMax = 3;
        bool correctCat = manager.tempActivity.activityCategory == ActivityClass.category.Family;
        int pictureUsed = 6;

        Debug.Log("lostBullet being tested: \nTrigger: " + triggerMax + "/100");

        if (correctCat && manager.City.currentChaos > exMinChaos && manager.City.currentCrime > exMinCrime && Random.Range(0, 100) < triggerMax)
        {
            int randomNum = Random.Range(1, 3);

            if (!manager.Family[randomNum].dead && !manager.Family[randomNum].gone)
            {
                manager.Kill(randomNum, "During your regular activities, a gang war breaks loose nearby. ", pictureUsed);
                manager.enqueuePopUp(manager.Family[randomNum].firstName + " is hit by a lost bullet. " + ((manager.Family[randomNum].sex == FamilyMembers.gender.him) ? "He" : "She") + " dies shortly after in the hospital.", pictureUsed);

                return true;
            }
            else
                return false;
        }
        else
            return false;
    }


    //Fortune events
    bool jerkUncleDeath()
    {
        //Constants
        float minFilth = 5;
        int maxMoney = 500;
        int maxTimeSinceLastEvent = 6;
        int triggerMax = 30;
        int gainedMoney = 400;
        int pictureUsed = 24;

        Debug.Log("jerkUncleDeath being tested: \nTrigger: " + triggerMax + "/100");

        if (manager.City.currentFilth > minFilth && manager.houseStats.getMoney() < maxMoney &&
            vars.daysSinceLastEvent < maxTimeSinceLastEvent && Random.Range(0, 100) < triggerMax)
        {
            manager.houseStats.modMoney(gainedMoney);

            manager.enqueuePopUp("One of your scummiest relatives has suddenly passed away. ", pictureUsed);
            manager.enqueuePopUp("Due to some legal loop hole, you end up inheriting a small part of his will.", pictureUsed);

            return true;
        }
        else
            return false;

    }

    bool kidnappedEscapes()
    {
        //Constants
        int maxFatherMood = 20;
        int triggerMax = 80;
        int pictureUsed = 25;

        Debug.Log("kidnappedEscape being tested: \nTrigger: " + triggerMax + "/100");

        if (vars.kidnappedMember > 0 && manager.Family[0].morale < maxFatherMood && Random.Range(0, 100) < triggerMax)
        {
            manager.houseStats.modMoney(vars.priceOfKidnapping);

            manager.toggleGone(vars.kidnappedMember, "You hear a knock on your door. You open and see " + manager.Family[vars.kidnappedMember].firstName + " covered in bruises, tears and dirt.", false, pictureUsed);
            manager.enqueuePopUp("You embrace " + manager.Family[vars.kidnappedMember].sex.ToString() + " as " + ((manager.Family[vars.kidnappedMember].sex == FamilyMembers.gender.him) ? "he" : "she") + " tells you about the escape.vYou don't have to worry about the captors anymore.", pictureUsed);

            vars.kidnappedMember = -1;

            return true;
        }
        else
            return false;

    }



    //Get functions
    public int getRobDays()
    {
        return vars.daysSinceLastRobbery;
    }

    public int getDaysRescue()
    {
        return vars.timeBeforeRescue;
    }

    public int getKidnappedMember()
    {
        return vars.kidnappedMember;
    }

    public int getDaysSinceTraffic()
    {
        return vars.daysSinceLastTraffic;
    }

    public int getNeglDays()
    {
        return vars.daysSinceNegligence;
    }

    public int getFixTime()
    {
        return vars.daysForServiceFix;
    }
}
