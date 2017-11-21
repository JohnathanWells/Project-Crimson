using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonScript : MonoBehaviour {

    public enum clickType { rightClick, middleClick, leftClick };

    public string functionName;
    public int valueToSend;
    public Transform scriptLocation;
    public clickType type;

    void OnMouseDown()
    {
        if ((type == clickType.leftClick && Input.GetMouseButtonDown(0))
            || (type == clickType.rightClick && Input.GetMouseButtonDown(1))
            || (type == clickType.middleClick && Input.GetMouseButtonDown(2)))
        {
            scriptLocation.SendMessage(functionName, valueToSend);
        }

    }

}
