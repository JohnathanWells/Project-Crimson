using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class decisionEventClass{

    public string name;
    public string situationText;
    public string triggerMax;
    public List<string> optionTexts;
    public List<string[]> optionEffects;

    public decisionEventClass(string line)
    {
        optionTexts = new List<string>();
        optionEffects = new List<string[]>();
        string[] temp = line.Split('\t');

        if (temp.Length >= 5)
        {
            name = temp[0];
            situationText = temp[2];
            triggerMax = temp[1];

            for (int n = 3; n < temp.Length; n+=2)
            {
                optionTexts.Add(temp[n]);
                optionEffects.Add(commandToStringArray(temp[n + 1]));
            }
        }
    }

    string [] commandToStringArray(string command)
    {
        return command.Split('~');
    }

}
