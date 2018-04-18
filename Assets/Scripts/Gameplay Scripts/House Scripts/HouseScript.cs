using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseScript : MonoBehaviour {

    [Header("General")]
    public Color[] healthColors = new Color[5];
    public Color[] moraleColors = new Color[5];

    [Header("References")]
    public GameManager manager;

    [Header("Icons")]
    public Transform notificationIcon;
    public Animator buttonAnimator;

    [Header("Drawer")]
    public Transform familyDrawer;
    private Animator drawerAnimations;
    private bool openDrawer = false;
    public SpriteRenderer[] familyIcons = new SpriteRenderer[4];
    public Transform[] sicknessIcons = new Transform[4];
    public Transform[] medicationIcons = new Transform[4];
    public Text moneyIndicator;
    
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
            toggleDrawer();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            buttonAnimator.SetTrigger("ButtonClicked");
            //buttonAnimator.ResetTrigger("ButtonClicked");

            if (!openWindow)
            {
                openHouseMenu();
            }
            else
            {
                closeHouseMenu();
            }
        }
        else if (Input.GetMouseButtonDown(2))
        {
            buttonAnimator.SetTrigger("ButtonClicked");
            //buttonAnimator.ResetTrigger("ButtonClicked");

            if (!openWindow)
            {
                openHouseMenu();
                MenuManagementScript.SendMessage("updateTab", 1);
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
        int tempInt;

        moneyIndicator.text = "<b>Money:</b>\n" + manager.houseStats.getMoney().ToString() + "$";

        for (int a = 0; a < 4; a++)
        {
            if (!familyArray[a].dead && !familyArray[a].gone)
            {
                familyIcons[a].gameObject.SetActive(true);

                //We display the member health with colors
                #region healthStatus
                tempInt = familyArray[a].health;

                if (tempInt > 75 && tempInt <= 100)
                {
                    familyIcons[a].color = healthColors[4];
                }
                else if (tempInt > 50 && tempInt <= 75)
                {
                    familyIcons[a].color = healthColors[3];
                }
                else if (tempInt > 25 && tempInt <= 50)
                {
                    familyIcons[a].color = healthColors[2];
                }
                else if (tempInt > 10 && tempInt <= 25)
                {
                    familyIcons[a].color = healthColors[1];
                }
                else if (tempInt <= 10)
                {
                    familyIcons[a].color = healthColors[0];
                }
                else
                {
                    familyIcons[a].color = healthColors[0];
                }
                #endregion


                #region moraleStatus (Commented out)
                //tempInt = FamilyMembers[a].morale;

                //if (tempInt > 75 && tempInt <= 100)
                //{
                //    moraleStatus[a].text = "Joyful";
                //    moraleStatus[a].color = moraleColors[4];
                //}
                //else if (tempInt > 50 && tempInt <= 75)
                //{
                //    moraleStatus[a].text = "Happy";
                //    moraleStatus[a].color = moraleColors[3];
                //}
                //else if (tempInt > 25 && tempInt <= 50)
                //{
                //    moraleStatus[a].text = "OK";
                //    moraleStatus[a].color = moraleColors[2];
                //}
                //else if (tempInt > 10 && tempInt <= 25)
                //{
                //    moraleStatus[a].text = "Depressed";
                //    moraleStatus[a].color = moraleColors[1];
                //}
                //else if (tempInt <= 10)
                //{
                //    moraleStatus[a].text = "Unstable";
                //    moraleStatus[a].color = moraleColors[0];
                //}
                //else
                //{
                //    moraleStatus[a].text = "ERROR";
                //    moraleStatus[a].color = moraleColors[0];
                //}
                #endregion
                
                //If the member is sick, display the thermometer
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
        if (!manager.currentlyExecutingActivity)
        {
            manager.houseOpen = true;
            window.gameObject.SetActive(true);
            windowAnimations.Play(windowOpenClip.name);
            openWindow = true;
            MenuManagementScript.SendMessage("openMenu", manager.Family);
            MenuManagementScript.SendMessage("updateHouseInfo", manager);
            notificationIcon.gameObject.SetActive(false);
        }
    }

    //This functions manipulate resource distribution
    public void manipulateFoodConsumption(arrowButtonClass information)
    {
        if (manager.houseStats.getFoodQ() - information.change >= 0)
        {
            manager.Family[information.id].food += information.change;
            //manager.houseStats.modFood(-information.change);

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
        if (windowAnimations.runtimeAnimatorController != null)
            StartCoroutine(closeWindow());
    }

    IEnumerator closeWindow()
    {
        windowAnimations.Play(windowCloseClip.name);
        yield return new WaitForSeconds(windowCloseClip.length);
        openWindow = false;
        manager.saveData();
        window.gameObject.SetActive(false);
        manager.houseOpen = false;
    }

    public void toggleDrawer()
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

    public void toggleDrawer(bool close)
    {
        if (close)
        {
            drawerAnimations.Play("Close");
            openDrawer = false;
        }
        else
        {
            drawerAnimations.Play("Open");
            openDrawer = true;
        }
    }
}
