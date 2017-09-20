using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonScript : MonoBehaviour {

    public class action
    {
        public enum clickType { rightClick, middleClick, leftClick };

        public string functionName;
        public int valueToSend;
        public Transform scriptLocation;
        public clickType type;
    };

    public action[] tiedActions;

    void OnMouseOver()
    {
        for (int n = 0; n < tiedActions.Length; n++)
        {
            if ((tiedActions[n].type == action.clickType.leftClick && Input.GetMouseButtonDown(0))
                || (tiedActions[n].type == action.clickType.rightClick && Input.GetMouseButtonDown(1))
                || (tiedActions[n].type == action.clickType.middleClick && Input.GetMouseButtonDown(2)))
            {
                tiedActions[n].scriptLocation.SendMessage(tiedActions[n].functionName, tiedActions[n].valueToSend);
            }

        }
    }

}
