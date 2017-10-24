using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScript : MonoBehaviour {



    [Header("References")]
    public GameManager manager;

    [Header("Icons")]
    public Transform notificationIcon;

    [Header("Drawer")]
    public Transform familyDrawer;
    private Animator drawerAnimations;
    private bool openDrawer = false;
    public Transform[] familyIcons = new Transform[4];
    public Transform[] sicknessIcons = new Transform[4];
    public Transform[] medicationIcons = new Transform[4];
    
    [Header("House Menu")]
    //Opening Window
    public Transform window;
    public AnimationClip windowOpenClip;
    public AnimationClip windowCloseClip;
    private bool openWindow = false;
    private Animator windowAnimations;
    //Menu
    public Transform MenuManagementScript; 




	void Awake () {
        //This hides the notification and the status symbols
        notificationIcon.gameObject.SetActive(false);

        //The animators related to the house icon
        window.gameObject.SetActive(false);
        drawerAnimations = familyDrawer.gameObject.GetComponent<Animator>();
        windowAnimations = window.gameObject.GetComponent<Animator>();

        for (int a = 0; a < 4; a++)
        {
            //Dad is 0, Mom is 1, Son is 2, Dau is 3

            //Sickness is checked
            if (manager.Family[a].status.ID != 0)
                sicknessIcons[a].gameObject.SetActive(true);
            else
                sicknessIcons[a].gameObject.SetActive(false);


            //Medicine is checked
            if (manager.Family[a].medicine == 0)
                medicationIcons[a].gameObject.SetActive(false);
            else
                medicationIcons[a].gameObject.SetActive(true);
        }
	}
	
    void OnMouseOver()
    {
        //If you click on the house, it will open the quick summary of the family. If you click on it again it will hide it.
        if (Input.GetMouseButtonDown(1))
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
        else if (Input.GetMouseButtonDown(0))
        {
            if (!openWindow)
            {
                openHouseMenu();
            }
            else
            {
                closeHouseMenu();
            }
        }
    }

    //This function is called to update the family icons with the current information
    public void familyUpdate(FamilyMembers[] familyArray)
    {
        for (int a = 0; a < 4; a++)
        {
            familyIcons[a].gameObject.SetActive(true);

            if (!familyArray[a].dead)
            {//If the member is sick, display the thermometer
                if (familyArray[a].status.ID != 0)
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
            }
            else
            {
                familyIcons[a].gameObject.SetActive(false);
            }

            //Change the color of the figure depending on the health
            //TODO
        }
    }

    //This function open the window
    public void openHouseMenu()
    {
        window.gameObject.SetActive(true);
        windowAnimations.Play(windowOpenClip.name);
        openWindow = true;
        MenuManagementScript.SendMessage("openMenu", manager.Family);
        MenuManagementScript.SendMessage("updateHouseInfo", manager);
    }

    //This functions manipulate resource distribution
    public void manipulateFoodConsumption(arrowButtonClass information)
    {
        if (manager.houseStats.getFoodQ() - information.change >= 0)
        {
            manager.Family[information.id].food += information.change;
            manager.houseStats.modFood(-information.change);

            MenuManagementScript.SendMessage("updateFamilyInfo", manager.Family);
            MenuManagementScript.SendMessage("updateHouseInfo", manager);
        }
    }

    public void manipulateMedicineReserve(arrowButtonClass information)
    {
        if (manager.houseStats.getMedicines() - information.change >= 0)
        {
            manager.Family[information.id].medicine += information.change;
            manager.houseStats.modMed(-information.change);

            MenuManagementScript.SendMessage("updateFamilyInfo", manager.Family);
            MenuManagementScript.SendMessage("updateHouseInfo", manager);
        }
    }

    //This function and coroutine close the window
    public void closeHouseMenu()
    {

        StartCoroutine(closeWindow());
    }

    IEnumerator closeWindow()
    {
        windowAnimations.Play(windowCloseClip.name);
        yield return new WaitForSeconds(windowCloseClip.length);
        openWindow = false;
        manager.saveData();
        window.gameObject.SetActive(false);
    }
}
