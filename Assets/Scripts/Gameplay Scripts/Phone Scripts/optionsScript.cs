using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class optionsScript : MonoBehaviour {

    public GameManager manager;
    public string surveyLink;

    public void saveAndExit()
    {
        manager.SendMessage("saveData");
        manager.SendMessage("toMainMenu");
    }

    public void restartScene()
    {
        SceneManager.LoadScene(this.gameObject.scene.name);
    }

    public void openSurvey()
    {
        Application.OpenURL(surveyLink);
    }
}
