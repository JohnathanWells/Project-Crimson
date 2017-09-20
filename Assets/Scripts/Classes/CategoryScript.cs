using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryScript : MonoBehaviour {

    public ActivityClass.category Category;
    public ActivityMenuScript MenuScript;
    public CategoryScript[] otherCategories;
    public Animator animator;
    public int catNum;

    void OnMouseDown()
    {
        MenuScript.SendMessage("changeActCat", Category);
        turnOn();
    }

    public void turnOff()
    {
        animator.SetBool(0, false);
    }

    public void turnOn()
    {
        //for (int n = 0; n < otherCategories.Length; n++)
        //{
        //    otherCategories[n].SendMessage("turnOff");
        //}

        animator.SetInteger("Selection", catNum);
    }
}
