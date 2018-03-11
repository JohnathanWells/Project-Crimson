using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonScript : MonoBehaviour {

    public enum clickType { rightClick, middleClick, leftClick };

    public string functionName;
    public int valueToSend;
    public Transform scriptLocation;
    public clickType type;

    public bool highlightOnHover = false;
    public Color defaultColor = Color.white;
    public Color highlitColor = Color.white;
    public Text textToHighlight;

    void OnMouseDown()
    {
        if ((type == clickType.leftClick && Input.GetMouseButtonDown(0))
            || (type == clickType.rightClick && Input.GetMouseButtonDown(1))
            || (type == clickType.middleClick && Input.GetMouseButtonDown(2)))
        {
            scriptLocation.SendMessage(functionName, valueToSend);
        }

    }

    void OnMouseOver()
    {
        if (highlightOnHover)
            textToHighlight.color = highlitColor;
    }

    void OnMouseExit()
    {
        if (highlightOnHover)
            textToHighlight.color = defaultColor;
    }

}
