﻿using System.Collections;
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
        if (MenuScript.selectedCategory != Category)
        {
            MenuScript.SendMessage("changeActCat", Category);
            turnOn();
        }
        else
        {
            MenuScript.SendMessage("changeActCat", ActivityClass.category.ALL);
            turnOff();
        }
    }

    public void turnOff()
    {
        animator.SetInteger("Selection", 0);
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
