using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class optionsScript : MonoBehaviour {

    public GameManager manager;

    public void saveAndExit()
    {
        manager.SendMessage("saveData");
        manager.SendMessage("toMainMenu");
    }
}
