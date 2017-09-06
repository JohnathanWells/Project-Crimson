using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phoneScript : MonoBehaviour {

    //The phone screen references
    public Transform phoneScreen;
    //The animations for the phone
    public Animator animator;
    public AnimationClip openingAnimation;
    public AnimationClip closingAnimation;
    private string openingAnimName;
    private bool phoneZoomed = false;

    void Start()
    {
        openingAnimName = openingAnimation.name;
        phoneScreen.gameObject.SetActive(false);
    }

    //Update phone information
    public void updatePhone()
    {

    }

    //Window Phone Stuff
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!phoneZoomed)
            {
                phoneScreen.gameObject.SetActive(true);
                animator.Play(openingAnimName);
                phoneZoomed = true;
            }
        }
    }

    public void closePhone()
    {
        StartCoroutine(closeWindow());
    }

    IEnumerator closeWindow()
    {
        animator.Play(closingAnimation.name);
        yield return new WaitForSeconds(closingAnimation.length);
        phoneZoomed = false;
        phoneScreen.gameObject.SetActive(false);
    }
}
