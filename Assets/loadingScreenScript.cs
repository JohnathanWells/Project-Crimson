using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loadingScreenScript : MonoBehaviour {
    public string levelToLoad;
    public float refreshRate;
    public bool showProgress = false;
    public Text progressTeller;

    void Start()
    {
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelToLoad);

        //Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;

            if (showProgress)
                progressTeller.text = (asyncLoad.progress * 100).ToString();
        }
    }

}
