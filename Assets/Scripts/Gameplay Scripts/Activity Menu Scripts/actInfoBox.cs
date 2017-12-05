using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actInfoBox : MonoBehaviour {

    [Header("Visuals")]
    public Vector3 Offset = new Vector3(0, 0, 5);

    [Header("Colors")]
    public Color goodColor = new Color(0, 1, 0, 1);
    public Color badColor = new Color(1, 0, 0, 1);
    public Color neutralColor = new Color(1, 1, 1, 1);

    [Header("Stats")]
    ActivityClass highlightedActivity;
    public TextMesh name;
    public TextMesh[] moraleChanges = new TextMesh[4];
    public TextMesh shopStatus;
    public TextMesh cost;

    Vector2 mousePos;

	// Update is called once per frame
    void Update () {
        mousePos = Input.mousePosition;
        transform.position = (Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x + Offset.x, mousePos.y + Offset.y, Offset.z)));
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

        cost.text = -highlightedActivity.cost + "$";

        if (highlightedActivity.cost > 0)
        {
            cost.color = badColor;
        }
        else if (highlightedActivity.cost < 0)
        {
            cost.color = goodColor;
        }
        else
        {
            cost.color = neutralColor;
        }
    }
}
