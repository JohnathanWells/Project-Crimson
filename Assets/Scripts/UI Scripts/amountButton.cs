using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class amountButton : MonoBehaviour {

    public int ID;
    public int quantityModified;
    private int currentNumber;
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
    }

    void OnMouseDown()
    {
        if (currentNumber + quantityModified <= MAX && currentNumber + quantityModified >= MIN)
        {
            functionReceiver.SendMessage(functionSent, information);
        }
    }

    public void updateNumber(int newDigit)
    {
        currentNumber = newDigit;
        counterpart.currentNumber = newDigit;
        display.text = "" + currentNumber;
    }
}
