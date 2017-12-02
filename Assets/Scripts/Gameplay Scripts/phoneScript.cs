using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phoneScript : MonoBehaviour {

    //References to other scripts
    public GameManager manager;
    //The phone screen references
    public Transform phoneScreen;
    public SpriteRenderer phoneScreenBackground;
    public Color[] timeColors = new Color[3];
    public SpriteRenderer[] DayDigits = new SpriteRenderer[2];
    public SpriteRenderer[] MonthDigits = new SpriteRenderer[2];
    public Sprite[] numbersDay = new Sprite[10];
    public Sprite[] numbersMonth = new Sprite[10];
    public Transform[] timeIndicators = new Transform[3];
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
        setDate();
        setTime();
    }

    public void setDate()
    {
        DayClass date = manager.currentDay;

        DayDigits[0].sprite = numbersDay[Mathf.FloorToInt(date.day / 10)];
        DayDigits[1].sprite = numbersDay[Mathf.FloorToInt(date.day % 10)];

        MonthDigits[0].sprite = numbersMonth[Mathf.FloorToInt(date.month / 10)];
        MonthDigits[1].sprite = numbersMonth[Mathf.FloorToInt(date.month % 10)];
    }

    public void setTime()
    {
        if (manager.currentTime == timeOfDay.morning)
        {
            phoneScreenBackground.color = timeColors[0];
            timeIndicators[0].gameObject.SetActive(true);
            timeIndicators[1].gameObject.SetActive(false);
            timeIndicators[2].gameObject.SetActive(false);
        }
        else if (manager.currentTime == timeOfDay.afternoon)
        {
            phoneScreenBackground.color = timeColors[1];
            timeIndicators[0].gameObject.SetActive(false);
            timeIndicators[1].gameObject.SetActive(true);
            timeIndicators[2].gameObject.SetActive(false);
        }
        else
        {
            phoneScreenBackground.color = timeColors[2];
            timeIndicators[0].gameObject.SetActive(false);
            timeIndicators[1].gameObject.SetActive(false);
            timeIndicators[2].gameObject.SetActive(true);
        } 
    }

    //Window Phone Stuff
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            openPhone();
        }
    }

    public void openPhone()
    {
        if (!phoneZoomed)
        {
            phoneScreen.gameObject.SetActive(true);
            animator.Play(openingAnimName);
            phoneZoomed = true;
            phoneScreen.SendMessage("openMenu");
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
