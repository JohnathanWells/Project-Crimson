using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phoneScript : MonoBehaviour {

    //References to other scripts
    public GameManager manager;
    public newsScript newsScript;
    //Reference to transforma
    public Transform notificationIcon;
    //The phone screen references
    public Transform phoneScreen;
    public SpriteRenderer phoneScreenBackground;
    public Color[] timeColors = new Color[3];
    public SpriteRenderer[] DayDigits = new SpriteRenderer[2];
    public SpriteRenderer[] MonthDigits = new SpriteRenderer[2];
    public SpriteRenderer dayOfWeekIndicator;
    public Sprite[] numbersDay = new Sprite[10];
    public Sprite[] numbersMonth = new Sprite[10];
    public Transform[] timeIndicators = new Transform[3];
    public Sprite[] dayOfWeekSprites = new Sprite[7];
    //The animations for the phone
    public Animator animator;
    public AnimationClip openingAnimation;
    public AnimationClip closingAnimation;
    private string openingAnimName;
    private bool phoneZoomed = false;

    public bool showTimeOfDayOnPhone = false;

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

        if (newsScript.checkIfTheresNews(manager.currentDay))
            showNotificationIcon(true);
    }

    public void setDate()
    {
        DayClass date = manager.currentDay;

        DayDigits[0].sprite = numbersDay[Mathf.FloorToInt(date.day / 10)];
        DayDigits[1].sprite = numbersDay[Mathf.FloorToInt(date.day % 10)];

        MonthDigits[0].sprite = numbersMonth[Mathf.FloorToInt(date.month / 10)];
        MonthDigits[1].sprite = numbersMonth[Mathf.FloorToInt(date.month % 10)];

        if (dayOfWeekIndicator != null)
            dayOfWeekIndicator.sprite = dayOfWeekSprites[(int)date.calculateDayOfWeek()];
    }

    public void setTime()
    {
        if (manager.currentTime == timeOfDay.morning)
        {
            phoneScreenBackground.color = timeColors[0];
            if (showTimeOfDayOnPhone && timeIndicators[0] != null && timeIndicators[1] != null && timeIndicators[2] != null)
            {
                timeIndicators[0].gameObject.SetActive(true);
                timeIndicators[1].gameObject.SetActive(false);
                timeIndicators[2].gameObject.SetActive(false);
            }
        }
        else if (manager.currentTime == timeOfDay.afternoon)
        {
            phoneScreenBackground.color = timeColors[1];
            if (showTimeOfDayOnPhone && timeIndicators[0] != null && timeIndicators[1] != null && timeIndicators[2] != null)
            {
                timeIndicators[0].gameObject.SetActive(false);
                timeIndicators[1].gameObject.SetActive(true);
                timeIndicators[2].gameObject.SetActive(false);
            }
        }
        else
        {
            phoneScreenBackground.color = timeColors[2];
            if (showTimeOfDayOnPhone && timeIndicators[0] != null && timeIndicators[1] != null && timeIndicators[2] != null)
            {
                timeIndicators[0].gameObject.SetActive(false);
                timeIndicators[1].gameObject.SetActive(false);
                timeIndicators[2].gameObject.SetActive(true);
            }
        } 
    }

    string displayDayOfWeekAbreviation(daysOfWeek day)
    {
        switch ((int)day)
        {
            case 0:
                return "SUN";
            case 1:
                return "MON";
            case 2:
                return "TUE";
            case 3:
                return "WED";
            case 4:
                return "THU";
            case 5:
                return "FRI";
            case 6:
                return "SAT";
            default:
                return "MISS";
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
            notificationIcon.gameObject.SetActive(false);
            phoneScreen.gameObject.SetActive(true);
            animator.Play(openingAnimName);
            phoneZoomed = true;
            newsScript.updateInfo(manager);
            phoneScreen.SendMessage("openMenu");
            manager.phoneOpen = true;
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
        manager.phoneOpen = false;
    }

    public void showNotificationIcon(bool enabled)
    {
        notificationIcon.gameObject.SetActive(enabled);
    }
}
