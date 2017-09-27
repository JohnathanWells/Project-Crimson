﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actInfoBox : MonoBehaviour {

    [Header("Colors")]
    public Color goodColor = new Color(0, 1, 0, 1);
    public Color badColor = new Color(1, 0, 0, 1);
    public Color neutralColor = new Color(1, 1, 1, 1);

    [Header("Stats")]
    ActivityClass highlightedActivity;
    public TextMesh name;
    public TextMesh[] moraleChanges = new TextMesh[4];
    public TextMesh shopStatus;

    Vector2 mousePos;

	// Update is called once per frame
    void Update () {
        mousePos = Input.mousePosition;
        transform.position = (Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, +5)));
    }

    public void setActivity(ActivityClass newHighlight)
    {
        highlightedActivity = newHighlight;
        updateText();
    }

    void updateText()
    {
        string tempString;
        int tempInt;

        name.text = highlightedActivity.activityName;

        for (int a = 0; a < 4; a++)
        {
            tempInt = highlightedActivity.moraleChange[a];
            
            if (tempInt > 0)
            {
                tempString = "+" + tempInt;
                moraleChanges[a].color = goodColor;
            }
            else if (tempInt < 0)
            {
                tempString = "" + tempInt;
                moraleChanges[a].color = badColor;
            }
            else
            {
                tempString = " " + tempInt;
                moraleChanges[a].color = neutralColor;
            }

            moraleChanges[a].text = tempString;
        }

        if (highlightedActivity.isItShop)
            shopStatus.text = "SHOP";
        else
            shopStatus.text = " ";
    }
}
