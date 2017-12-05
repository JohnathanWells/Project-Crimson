using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activityButtonScript : MonoBehaviour {

    public Transform statDisplay;
    public ActivityMenuScript menuManager;
    public TextMesh textDisplay;
    public Color enabledColor = new Color(1, 1, 1, 1);
    public Color disabledColor = new Color(1, 1, 1, 1);
    bool active = false;
    bool isAvailable = true;
    ActivityClass assignedActivity;

    void Start()
    {
        textDisplay = gameObject.GetComponentInChildren<TextMesh>();
    }

    void OnMouseOver()
    {
        if (active)
        {
            statDisplay.gameObject.SetActive(true);
            statDisplay.SendMessage("setActivity", assignedActivity);
        }
    }

    void OnMouseDown()
    {
            
        if (active && isAvailable)
        {
            menuManager.SendMessage("executeActivity", assignedActivity);
            statDisplay.gameObject.SetActive(false);
            //menuManager.executeActivity(assignedActivity);
        }
    }

    void OnMouseExit()
    {
        statDisplay.gameObject.SetActive(false);
    }

    public void setEmpty()
    {
        active = false;
        textDisplay.text = " ";
    }

    public void setActivity(ActivityClass newAssign)
    {
        active = true;
        assignedActivity = newAssign;

        if (textDisplay != null && !assignedActivity.paysService)
            textDisplay.text = assignedActivity.activityName;

        if (assignedActivity.cost <= menuManager.getHouseMoney())
        {
            isAvailable = true;

            textDisplay.color = enabledColor;
        }
        else
        {
            isAvailable = false;

            textDisplay.color = disabledColor;
        }
        //statDisplay.SendMessage("setActivity", assignedActivity);
    }

    public void updateActivityStatus(timeOfDay currentTime)
    {
        if (assignedActivity.cost <= menuManager.getHouseMoney() && assignedActivity.timeOfDayAvailability[(int)currentTime])
        {
            textDisplay.color = enabledColor;
            active = true;
        }
        else
        {
            textDisplay.color = disabledColor;
            active = false;
        }
    }
}
