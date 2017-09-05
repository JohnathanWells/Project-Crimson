using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScript : MonoBehaviour {

    //References to other Scripts

    //Icons and such
    public Transform notificationIcon;
        //Family Drawer
    public Transform familyDrawer;
    private Animator drawerAnimations;
    private bool openDrawer = false;
    public Transform[] familyIcons = new Transform[4];
    public Transform[] sicknessIcons = new Transform[4];
    public Transform[] medicationIcons = new Transform[4];
        //House Menu
    public Transform window;
    private GameObject windowObject;
    private bool openWindow = false;
    private Animator windowAnimations;


	void Start () {
        //This hides the notification and the status symbols
        notificationIcon.gameObject.SetActive(false);
        windowObject = window.gameObject;
        windowObject.SetActive(false);
        drawerAnimations = familyDrawer.gameObject.GetComponent<Animator>();
        windowAnimations = window.gameObject.GetComponent<Animator>();
        for (int a = 0; a < 4; a++)
        {
            sicknessIcons[a].gameObject.SetActive(false);
            medicationIcons[a].gameObject.SetActive(false);
        }
	}
	
    void OnMouseOver()
    {
        //If you click on the house, it will open the quick summary of the family. If you click on it again it will hide it.
        if (Input.GetMouseButtonDown(0))
        {
            if (!openDrawer)
            {
                drawerAnimations.Play("OpenDrawer");
                openDrawer = true;
            }
            else
            {
                drawerAnimations.Play("CloseDrawer");
                openDrawer = false;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (!openWindow)
            {
                window.gameObject.SetActive(true);
                windowAnimations.Play("OpenHouseMenu");
                openWindow = true;
            }
            else
            {
                windowAnimations.Play("CloseHouseMenu");
                openWindow = false;
                //window.gameObject.SetActive(false);
            }
        }
    }

    //This function is called to update the family icons with the current information
    public void updateFamilyIcons(FamilyMembers[] familyArray)
    {
        for (int a = 0; a < 4; a++)
        {
            //If the member is sick, display the thermometer
            if (familyArray[a].status != FamilyMembers.sickness.Healthy)
            {
                sicknessIcons[a].gameObject.SetActive(true);
                notificationIcon.gameObject.SetActive(true);
                            
                //If the member is being medicated, display the pill
                if (familyArray[a].medicine > 0)
                {
                    medicationIcons[a].gameObject.SetActive(true);
                }
                else
                    medicationIcons[a].gameObject.SetActive(false);
            }
            else
            {
                sicknessIcons[a].gameObject.SetActive(false);
                medicationIcons[a].gameObject.SetActive(false);
            }

            //Change the color of the figure depending on the health
            //TODO
        }
    }

    public void closeHouseMenu()
    {
        openWindow = false;
    }
}
