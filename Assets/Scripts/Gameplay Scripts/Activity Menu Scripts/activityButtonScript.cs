using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activityButtonScript : MonoBehaviour {

    public Transform statDisplay;
    public ActivityMenuScript menuManager;
    public TextMesh textDisplay;
    public int maxCharLength = 14;
    public Color enabledColor = new Color(1, 1, 1, 1);
    public Color disabledColor = new Color(1, 1, 1, 1);
    public Color highlitButtonColor = new Color(1, 1, 1, 1);
    public bool isAlsoPointer = false;
    public Transform correspondingPointer;

    bool highlit = false;
    bool active = false;
    bool isAvailable = true;
    ActivityClass assignedActivity;

    void Start()
    {
        if (!isAlsoPointer)
            textDisplay = gameObject.GetComponentInChildren<TextMesh>();
    }

    void OnMouseOver()
    {
        if (active && !menuManager.Manager.phoneOpen && !menuManager.Manager.houseOpen)
        {
            statDisplay.gameObject.SetActive(true);
            statDisplay.SendMessage("setActivity", assignedActivity, SendMessageOptions.DontRequireReceiver);
            highLightPointer(true);
        }
    }

    void OnMouseDown()
    {
        if (active && isAvailable && !menuManager.Manager.phoneOpen && !menuManager.Manager.houseOpen)
        {
            menuManager.toggleFilm(true);
            menuManager.SendMessage("executeActivity", assignedActivity);
            statDisplay.gameObject.SetActive(false);
            highLightPointer(false);

            //menuManager.executeActivity(assignedActivity);
        }
    }

    void OnMouseExit()
    {
        statDisplay.gameObject.SetActive(false);
        highLightPointer(false);
    }

    public void setEmpty()
    {
        if (!isAlsoPointer)
        {
            active = false;
            textDisplay.text = " ";
        }
    }

    public void setActivity(ActivityClass newAssign)
    {
        active = true;
        assignedActivity = newAssign;

        if (!isAlsoPointer)
            highLightPointer(false);

        if (textDisplay != null && !assignedActivity.paysService)
        {
            if (assignedActivity.activityName.Length > maxCharLength)
                textDisplay.text = assignedActivity.activityName.Remove(maxCharLength);
            else
                textDisplay.text = assignedActivity.activityName;
        }

        if (assignedActivity.cost <= menuManager.getHouseMoney())
        {
            isAvailable = true;

            if (!isAlsoPointer)
                textDisplay.color = enabledColor;
            else
                gameObject.SetActive(true);
        }
        else
        {
            isAvailable = false;

            if (!isAlsoPointer)
                textDisplay.color = disabledColor;
            else
                gameObject.SetActive(false);
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

    public void highLightPointer(bool on)
    {
        if (on)
        {
            if (!highlit)
                menuManager.SendMessage("playHighlightSound", SendMessageOptions.DontRequireReceiver);

            if (isAlsoPointer)
            {
                GetComponent<SpriteRenderer>().color = enabledColor;
            }
            else
            {
                correspondingPointer.SendMessage("highLightPointer", true, SendMessageOptions.DontRequireReceiver);
                textDisplay.color = highlitButtonColor;
            }

            highlit = true;
        }
        else
        {
            if (isAlsoPointer)
            {
                GetComponent<SpriteRenderer>().color = disabledColor;
            }
            else
            {
                correspondingPointer.SendMessage("highLightPointer", false, SendMessageOptions.DontRequireReceiver);
                textDisplay.color = enabledColor;
            }

            highlit = false;
        }
    }
}
