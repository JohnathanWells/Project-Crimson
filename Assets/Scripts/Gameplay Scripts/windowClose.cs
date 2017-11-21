using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windowClose : MonoBehaviour {

    public Transform window;
    public Animator animator;
    public AnimationClip closeAnimation;
    private string closeAnimationName;
    //The script that opens the window needs to keep track of when is it open
    public Transform thisWindowManager;
    //This function changes the state of the window
    public string closingFunction;
    //If on, it just deactivates the window.
    bool simple = false;


    void Start()
    {
        closeAnimationName = closeAnimation.name;
    }

    void OnMouseDown()
    {
        if (!simple)
            thisWindowManager.gameObject.SendMessage(closingFunction);
        else
            window.gameObject.SetActive(false);
    }
}
