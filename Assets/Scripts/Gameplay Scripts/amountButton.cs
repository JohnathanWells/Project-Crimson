using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class amountButton : MonoBehaviour {

    public int ID;
    public int quantityModified;
    private int currentNumber;
    float timeHeld = 0f;
    float timeBeforeActivate;
    public Transform functionReceiver;
    public amountButton counterpart;
    public string functionSent;

    public int MAX;
    public int MIN;

    public TextMesh display;
    private arrowButtonClass information;

    void Start()
    {
        information = new arrowButtonClass(ID, quantityModified);
        timeBeforeActivate = Constants.mouseHoldSecondsPerItem;
    }

    void OnMouseDown()
    {
        activate();

    }

    void OnMouseDrag()
    {
        timeHeld += Time.deltaTime;

        if (timeHeld >= Constants.mouseHoldSecondsBeforeAutoActivate)
        {
            timeBeforeActivate -= Time.deltaTime;

            if (timeBeforeActivate <= 0)
            {
                activate();
                timeBeforeActivate = Constants.mouseHoldSecondsPerItem;
            }
        }
    }

    void OnMouseUp()
    {
        timeHeld = 0;
        timeBeforeActivate = Constants.mouseHoldSecondsPerItem;
    }

    public void updateNumber(int newDigit)
    {
        currentNumber = newDigit;
        counterpart.currentNumber = newDigit;
        display.text = "" + currentNumber;
    }

    public void changeColor(Color to)
    {
        display.color = to;
    }

    void activate()
    {
        if (currentNumber + quantityModified <= MAX && currentNumber + quantityModified >= MIN)
        {
            functionReceiver.SendMessage(functionSent, information);
        }
    }
}
