using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class musicMenuScript : MonoBehaviour {

    public Text musicDisplayText;
    public soundScript audioManager;

    public void DisplayCurrentSong()
    {
        musicDisplayText.text = audioManager.currentSong + " - " + audioManager.availableSongs[audioManager.currentSong].name;
    }

}
