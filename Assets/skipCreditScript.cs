using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class skipCreditScript : MonoBehaviour {

    public KeyCode buttonForSkip = KeyCode.Escape;
    public int mainMenuID = 0;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(buttonForSkip))
            SceneManager.LoadScene(mainMenuID);
	}
}
