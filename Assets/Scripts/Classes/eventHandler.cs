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
        public int priceOfKidnapping = 1000;

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

            Debug.Log("Event Type: " + RN + "/" + eventIndex.Count + "/" + rollingChance + "\tSpecific: " + specificEvent + "/" + (eventIndex[RN].Length - 1) + " (" + eventIndex[RN][specificEvent] + ")");

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

            vars.timeBeforeRescue--;

            if (Random.Range(0, 100) <= percentageChanceOfDoubleCross)
            {
                manager.addTimeTransition();

                manager.Kill(vars.kidnappedMember, "You leave a pack of bills inside the trash can of a public park and wait in the car for about forty five minutes. #n Then two hours. #n You never see " + manager.Family[vars.kidnappedMember].firstName + " again.");

                vars.kidnappedMember = -1;
            }
            else
            {
                manager.addTimeTransition();

                manager.toggleGone(vars.kidnappedMember, "You leave a pack of bills inside the trash can of a public park and wait in the car for about forty five minutes. " + manager.Family[vars.kidnappedMember].firstName + " shows up running to the car, crying, skinny and covered in bruises. You take them back home.", false);

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
        int pictureUsed = 0;
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
        int pictureUsed = 0;
        int minTimePassed = 7;

        //Semi Constant vars
        bool isSpecificSector = (manager.tempActivity.area == ActivityClass.sector.B || manager.tempActivity.area == ActivityClass.sector.C || manager.tempActivity.area == ActivityClass.sector.E || manager.tempActivity.area == ActivityClass.sector.F);
        bool isSpecificTime = (manager.currentTime == timeOfDay.evening);
        float triggerMax = (manager.City.districts[(int)manager.tempActivity.area].baseCrimeRateEvening) * manager.City.currentCrime;

        if (minTimePassed < vars.daysSinceLastRobbery && isSpecificTime && isSpecificSector && Random.Range(0, 100) < triggerMax)
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
        int pictureUsed = 0;
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
                                    descriptionText += "Fortunately, you manage to take them to the emergency room of the hospital and they manage to save their life.";

                                    foreach (FamilyMembers f in manager.Family)
                                        f.moraleChange(moraleEffect);
                                }
                                else
                                {
                                    manager.Kill(n, "The bullet pierces the lungs. " + manager.Family[n].firstName + " dies in your arms drowned in their own blood.");

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
        int pictureUsed = 0;
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
                descriptionText += "The mugger takes your wallet and notices you don't even have " + maxSpareMoney + " on you. He gets angry and starts shouting at you.";
                manager.Kill(0, "You see a flash of light and fall dead on the floor.");
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


    //Kidnapping Events
    bool expressKidnapping()
    {
        //Constants
        float exMinChaos = 6;
        float exMinCrime = 4;
        int moodChange = -15;
        int pictureUsed = 0;

        //Semi constant
        float triggerMax = manager.City.districts[3].currentCREvening;

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

                    manager.enqueuePopUp("When you get home you receive a strange phone call that tells you " + manager.Family[n].firstName + " has been kidnapped. You pay the rescue and a couple hours later they are delivered on a public park with little more than a couple bruises.", pictureUsed);

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
        int moraleDropForKidnapped = -15;

        //Semi constant
        float triggerMax = manager.City.districts[3].currentCREvening;

        if (!vars.kidnappedAlready && manager.membersInHouse > 1 && manager.City.currentCrime > exMinCrime && manager.City.currentChaos > exMinChaos && Random.Range(0, 100) < triggerMax)
        {
            manager.addTimeTransition();

            for (int n = 3; n > 0; n++)
            {
                if (!manager.Family[n].dead && !manager.Family[n].gone)
                {
                    manager.addTimeTransition();

                    manager.toggleGone(n, "When you get home you receive a strange phone call that tells you " + manager.Family[n].firstName + " has been kidnapped. The kidnapper tells a date and place " + daysCaptive + " days later where you need to leave the money.", true);

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

        //Dinamic constants
        float triggerMax = ((manager.City.currentChaos + manager.City.districts[(int)manager.tempActivity.area].traffic) * 10) / 2;

        if (manager.currentTime != timeOfDay.evening && manager.City.currentChaos > exMinChaos && vars.daysSinceLastTraffic > minTimePassed && Random.Range(0, 100) < triggerMax)
        {
            manager.addTimeTransition();

            manager.Family[0].moraleChange(moralEffect);

            vars.daysSinceLastTraffic = -1;

            manager.enqueuePopUp("Apparently a jerk t-boned another car in the main avenue. You'll be stuck in traffic for a while.", 0);

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
        float triggerMax = ((manager.City.currentChaos + manager.City.districts[(int)manager.tempActivity.area].traffic) * 5) / 3;

        if (manager.currentTime != timeOfDay.evening && manager.City.currentChaos > exMinChaos && vars.daysSinceLastTraffic > minTimePassed && Random.Range(0, 100) < triggerMax)
        {
            manager.addTimeTransition();

            manager.Family[0].moraleChange(moralEffect);

            vars.daysSinceLastTraffic = -1;

            manager.enqueuePopUp("A jerk t-boned you in the main avenue. You'll be stuck here for a while until someone tows you away.", 0);

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
        float triggerMax = manager.City.currentChaos * 5;
        int duration = 1;
        timeOfDay requiredTime = timeOfDay.morning;
        int pictureUsed = 0;

        if (manager.currentTime == requiredTime && manager.City.currentChaos > exMinChaos && minTimeForDisservice < vars.daysSinceNegligence && Random.Range(0, 100) < triggerMax)
        {
            manager.enqueuePopUp("That morning you wake up with no water or electricity. You call the providers who insist the issue will be today.", pictureUsed);

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
        float triggerMax = manager.City.currentChaos;
        int duration = 7;
        timeOfDay requiredTime = timeOfDay.morning;
        int pictureUsed = 0;

        if (manager.currentTime == requiredTime && manager.City.currentChaos > exMinChaos && minTimeForDisservice < vars.daysSinceNegligence && Random.Range(0, 100) < triggerMax)
        {
            manager.enqueuePopUp("That morning you wake up with no water or electricity. You call the providers who insist the issue will be solved soon. You have a hunch they are full of it.", pictureUsed);

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
        int indexA = 30, indexB = 35;
        int pictureUsed = 0;

        if (manager.membersAlive > 1 && manager.membersInHouse > 1 && correctDayAndTime && correctActivity && randomNum >= indexA && randomNum <= indexB)
        {
            manager.enqueuePopUp("You decide to spend the holiday on your own. You hear the fireworks in the distance as the night comes to an end. When you get back home you find yourself in front of a house burning to the ground with your family inside. The firefighters never arrived.", pictureUsed);

            for (int n = 1; n < 4; n++)
                manager.Kill(n, manager.Family[n].firstName + " died in the fire.");

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
        int pictureUsed = 0;

        if (exMinChaos > manager.City.currentChaos && vars.daysSinceLastEvent > minTimeSinceLastEvent && Random.Range(0, 100) < triggerMax)
        {
            manager.houseStats.modMoney(moneyLost);

            foreach (FamilyMembers f in manager.Family)
            {
                f.moraleChange(moraleDrop);
            }

            manager.enqueuePopUp("On the way back it suddenly starts raining hard. By the time you home the whole house is flooded after a nearby storm drain got clogged.", pictureUsed);

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
        int pictureUsed = 0;

        //Semi constant
        bool correctTime = (manager.currentTime == timeOfDay.morning);

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

            manager.enqueuePopUp("You receive a call from one of your multiple uncles as soon as you wake up. Apparently one of your cousins was shot dead after someone tried to mug him. You tell #f of the funeral.", pictureUsed);

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

        if (correctCat && manager.City.currentChaos > exMinChaos && manager.City.currentCrime > exMinCrime && Random.Range(0, 100) < triggerMax)
        {
            int randomNum = Random.Range(1, 3);

            if (!manager.Family[randomNum].dead && !manager.Family[randomNum].gone)
            {
                manager.Kill(randomNum, "During your regular activities, a gang war breaks loose nearby. " + manager.Family[randomNum].firstName + " is hit by a lost bullet. They die shortly after in the hospital.");

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
        int pictureUsed = 0;

        if (manager.City.currentFilth > minFilth && manager.houseStats.getMoney() < maxMoney &&
            vars.daysSinceLastEvent < maxTimeSinceLastEvent && Random.Range(0, 100) < triggerMax)
        {
            manager.houseStats.modMoney(gainedMoney);

            manager.enqueuePopUp("One of your relatives, an awful human being liked by no one in your family, has suddenly passed away from one of the pandemics of " + manager.City.stateName + ". Due to some legal shenanigans, you end up inheriting a small part of his will.", pictureUsed);

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


        if (vars.kidnappedMember > 0 && manager.Family[0].morale < maxFatherMood && Random.Range(0, 100) < triggerMax)
        {
            manager.houseStats.modMoney(vars.priceOfKidnapping);

            manager.toggleGone(vars.kidnappedMember, "You hear a knock on your door. When you answer it " + manager.Family[vars.kidnappedMember].firstName + " covered in bruises, tears and dirt. You embrace them as they tell you how they escaped. You don't have to worry about the captors anymore.", false);

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
