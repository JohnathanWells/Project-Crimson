using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activityButtonScript : MonoBehaviour {

    public Transform statDisplay;

    public TextMesh textDisplay;
    bool active = false;
    ActivityClass assignedActivity;

    void Start()
    {
        textDisplay = gameObject.GetComponentInChildren<TextMesh>();
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

        textDisplay.text = assignedActivity.activityName;
        //statDisplay.SendMessage("setActivity", assignedActivity);
    }

    void OnMouseOver()
    {
        if (active)
        {
            statDisplay.gameObject.SetActive(true);
            statDisplay.SendMessage("setActivity", assignedActivity);
        }
    }

    void OnMouseExit()
    {
        statDisplay.gameObject.SetActive(false);
    }
}
