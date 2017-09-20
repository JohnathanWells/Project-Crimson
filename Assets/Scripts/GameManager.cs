using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public enum timeOfDay { Morning, Afternoon, Evening };


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

    [Header("Files")]
    public TextAsset activityFile;
    public TextAsset statChangesFile;
    public TextAsset districtStatsFile;
    
    [Header("Day Stuff")]
    public DayClass currentDay;
    public timeOfDay currentTime;
    private int currentDayNum;

    [Header("Permanent Stuff")]
    public DayClass[] sessionCalendar;
    public List<ActivityClass> mornActivities = new List<ActivityClass>();
    public List<ActivityClass> noonActivities = new List<ActivityClass>();
    public List<ActivityClass> evenActivities = new List<ActivityClass>();




    void Start()
    {
        newSession();

        //TODO
        //Remember to change this with a load thing once I get to that

        transitionDay();
    }

    void transitionDay()
    {
        //TODO
        //Everything else

        Phone.SendMessage("updatePhone");
        House.SendMessage("familyUpdate", Family);
        Activities.SendMessage("updateMenu");
    }

    void newSession()
    {
        City = new cityClass(districtStatsFile, statChangesFile);
        houseStats = new houseClass();

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

        Family[0] = new FamilyMembers(FamilyMembers.famMember.Dad);
        Family[0].setName("Pedro Perez");
        Family[1] = new FamilyMembers(FamilyMembers.famMember.Mom);
        Family[1].setName("Maria Perez");
        Family[2] = new FamilyMembers(FamilyMembers.famMember.Son);
        Family[2].setName("Jose Perez");
        Family[3] = new FamilyMembers(FamilyMembers.famMember.Dau);
        Family[3].setName("Juana Perez");
    }

    int calculateArrayPos(int month, int day)
    {
        int result = 0;
        int n = 10;

        while (true)
        {
            if (n == month)
            {
                return result + day;
            }
            else
            {
                if (month == 10 || month == 12 || month == 1 || month == 3)
                {
                    result += 31;
                }
                else if (month == 2)
                {
                    result += 28;
                }
                else
                {
                    result += 30;
                }
            }

            //Add 1 to n to change month, if it is 0 then we turn it into 1 to start counting from there
            n = (n + 1) % 12;

            if (n == 0)
            {
                n++;
            }
        }
    }
}
