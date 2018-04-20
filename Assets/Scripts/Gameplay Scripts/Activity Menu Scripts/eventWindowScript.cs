using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class eventWindowScript : MonoBehaviour {
    public GameManager manager;
    public Transform[] optionButtons;
    public Text[] optionTexts;
    public Text textDisplay;

    decisionEventClass selectedEvent;

    void Awake()
    {
        if (optionButtons.Length > 0)
        {
            optionTexts = new Text[optionButtons.Length];
            for (int i = 0; i < optionButtons.Length; i++)
            {
                optionTexts[i] = optionButtons[i].GetComponentInChildren<Text>();
            }
        }
    }

    public void displayDecision(decisionEventClass selected)
    {
        selectedEvent = selected;

        //We display the situation
        textDisplay.text = selected.situationText;

        int n;

        for (n = 0; n < Mathf.Min(optionButtons.Length, selected.optionTexts.Count); n++)
        {
            optionButtons[n].gameObject.SetActive(true);
            //Debug.Log(selected.optionTexts.Count + " " + optionButtons.Length + " : " + n);
            
            optionTexts[n].gameObject.name = n.ToString();
            optionTexts[n].text = selected.optionTexts[n];
        }

        for (; n < optionButtons.Length; n++)
        {
            optionButtons[n].gameObject.SetActive(false);
        }
    }

    public void choseOption(int number)
    {
        executeCommand(selectedEvent.optionEffects[number]);
        manager.closePopUp();
        manager.concludeActivity();
    }

    public void executeCommand(string[] instructions)
    {
        foreach (string s in instructions)
            translateInstruction(s);
    }

    public void translateInstruction(string instruction)
    {
        string[] parts = instruction.Split('_');
        //Debug.Log(parts.Length);

        if (parts.Length > 0)
        {
            switch (parts[0])
            {
                case "addFood":
                    manager.houseStats.modFood(int.Parse(parts[1]));
                    break;
                case "addMoney":
                    manager.houseStats.modMoney(int.Parse(parts[1]));
                    break;
                case "addMorale":
                    manager.Family[int.Parse(parts[1])].moraleChange(int.Parse(parts[2]));
                    break;
                case "addHealth":
                    manager.Family[int.Parse(parts[1])].healthChange(int.Parse(parts[2]));
                    break;
                case "addRabies":
                    manager.Family[int.Parse(parts[1])].getsSick(manager.sicknesses[4]);
                    break;
                case "addMedicine":
                    manager.houseStats.modMed(int.Parse(parts[1]));
                    break;
                case "addHygiene":
                    manager.houseStats.modHygiene(int.Parse(parts[1]));
                    break;
                case "addCleaning":
                    manager.houseStats.modCleaning(int.Parse(parts[1]));
                    break;
                case "passTime":
                    manager.addTimeTransition();
                    break;
                case "addMessage":
                    manager.enqueuePopUp(parts[1], int.Parse(parts[2]));
                    break;
                case "Kill":
                    if (parts.Length > 4)
                        manager.Kill(int.Parse(parts[1]), parts[2], int.Parse(parts[3]), parts[4]);
                    else
                        manager.Kill(int.Parse(parts[1]), parts[2], int.Parse(parts[3]));
                    break;
                case "GameOver":
                    manager.gameOver();
                    break;
                default:
                    break;
            }
        }
    }
}
