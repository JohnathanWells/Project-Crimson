using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneMenuScript : MonoBehaviour {

    public GameManager manager;
    public Transform[] tabs;
    public Transform[] tabKeepers;

    int currentTab = -1;

    //void Start()
    //{
    //    updateTab(0);
    //}

    public void refreshInfo(GameManager mngr)
    {
        manager = mngr;
        //Debug.Log(manager.wikiDatabase.title + " " + manager.wikiDatabase.subordinates.Count);
        tabs[currentTab].SendMessage("updateInfo", manager);
    }

    public void openMenu()
    {
        updateTab(0);

        refreshInfo(manager);
    }

    //These functions deal with changing tabs
    public void changeTab(int direction)
    {
        currentTab = (currentTab + direction) % tabs.Length;


    }

    public void updateTab(int tabNum)
    {
        if (tabNum != currentTab)
        {
            currentTab = tabNum;

            for (int n = 0; n < tabs.Length; n++)
            {
                if (n == tabNum)
                {
                    tabs[n].gameObject.SetActive(true);
                    tabKeepers[n].SendMessage("setTabActive", true);
                    refreshInfo(manager);
                }
                else
                {
                    tabs[n].gameObject.SetActive(false);
                    tabKeepers[n].SendMessage("setTabActive", false);
                }
            }

        }
    }
}
