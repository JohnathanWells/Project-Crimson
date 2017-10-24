using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sickAlertScript : MonoBehaviour {

    public Transform symptomsDisplayWindow;
    sicknessClass currentSickness;

    void OnMouseOver()
    {
        symptomsDisplayWindow.gameObject.SetActive(true);
        symptomsDisplayWindow.SendMessage("setSickness", currentSickness);
    }

    void OnMouseExit()
    {
        symptomsDisplayWindow.gameObject.SetActive(false);
    }

    public void setSickness(sicknessClass illness)
    {
        currentSickness = illness;
    }
}
