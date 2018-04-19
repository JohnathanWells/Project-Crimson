using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameOverWindowScript : MonoBehaviour {

    public Text deathMessage;
    public TextMesh finalDayDisplay;
    public TextMesh[] familyStatuses;
    public TextMesh[] names;
    public int mainMenuID = 0;
    public int creditID = 3;
    public Color goodColor = Color.green;
    public Color badColor = Color.red;
    public Color couldBeWorseColor = Color.yellow;
    public int maxNameLength = 8;

    bool Victory = false;

    DayClass finalDay;
    FamilyMembers[] family;

    public void setFamily(FamilyMembers[] fam)
    {
        family = fam;
    }

    public void setDay(DayClass day)
    {
        finalDay = day;
    }

    public void displayGameOver()
    {
        deathMessage.text = trimString(family[0].firstName.Split(' ')[0]) + " " + trimString(family[0].lastName) + " is dead!";
        finalDayDisplay.text = "You died on " + finalDay.calculateMonth() + " " + finalDay.day + "th\nYou lasted " + finalDay.dayCount + " days.";

        for (int n = 0; n < 4; n++)
        {
            names[n].text = trimString(family[n].firstName);

            if (family[n].dead)
            {
                familyStatuses[n].text = "DEAD\n[" + family[n].deathCause + "]";
                familyStatuses[n].color = badColor;
            }
            else if (family[n].gone && !family[n].dead)
            {
                familyStatuses[n].text = "GONE";
                familyStatuses[n].color = couldBeWorseColor;
            }
            else
            {
                familyStatuses[n].text = "ALIVE";
                familyStatuses[n].color = goodColor;
            }
        }
    }

    public void displayVictory()
    {
        Victory = true;

        deathMessage.color = goodColor;
        deathMessage.text = trimString(family[0].firstName.Split(' ')[0]) + " " + trimString(family[0].lastName) + " survived to " + finalDay.calculateMonth() + " " + finalDay.day + "th";
        finalDayDisplay.text = "You managed to survive " + finalDay.dayCount + " days.";

        for (int n = 0; n < 4; n++)
        {
            names[n].text = trimString(family[n].firstName);

            if (family[n].dead)
            {
                familyStatuses[n].text = "DEAD\n[" + family[n].deathCause + "]";
                familyStatuses[n].color = badColor;
            }
            else if (family[n].gone && !family[n].dead)
            {
                familyStatuses[n].text = "GONE";
                familyStatuses[n].color = couldBeWorseColor;
            }
            else
            {
                familyStatuses[n].text = "ALIVE";
                familyStatuses[n].color = goodColor;
            }
        }
    }

    public void resetGame()
    {
        if (Victory)
        {
            SaveLoad.Delete();
            SceneManager.LoadScene(creditID);
        }
        else
        {
            SaveLoad.Delete();
            SceneManager.LoadScene(mainMenuID);
        }
    }

    string trimString(string str)
    {
        if (str.Length > maxNameLength)
        {
            return str.Substring(0, maxNameLength - 1) + ".";
        }
        else
            return str;
    }
}
