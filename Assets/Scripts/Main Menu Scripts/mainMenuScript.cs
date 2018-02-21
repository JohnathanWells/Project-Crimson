using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

//Expand
enum maleNameList { John, Joe, David, Markus, Daniel, Chris, Nathan };
enum femaleNameList { Mary, Sarah, Cristina, Erica, Danielle, Max, Sam};

//Remember to always leave main at the end
enum menus {newGame, main};

public class mainMenuScript : MonoBehaviour {

    [Header("Continue")]
    public Transform continueButton;

    [Header("Files and Text")]
    public TextAsset maleNamesFile;
    public TextAsset femaleNamesFile;
    private string[] maleNameList;
    private string[] femaleNameList;

    [Header("New Game")]
    //Data loading
    newGameClass gameStartup = new newGameClass();
    public InputField[] names = new InputField[4];
    public InputField lastName;
    public int mainGameScene = 1;
    public bool dataLoaded = false;
    public Transform warning;

    //Menu Management
    [Tooltip("[0] newGameMenu\n[1] mainMenu")]
    public Transform[] menuParents = new Transform[(int)menus.main + 1];
    menus currentMenu = menus.main;

    void Start()
    {
        maleNameList = maleNamesFile.text.Split(' ');
        femaleNameList = femaleNamesFile.text.Split(' ');

        SaveLoad.Load();

        if (SaveLoad.savedGame == null || SaveLoad.savedGame.empty || SaveLoad.savedGame.isDadDead())
            continueButton.gameObject.SetActive(false);
    }

    //Used in starting a new game or loading one
    public void startNewGame()
    {

        if (!isStringEmpty(lastName.text))
        {
            randomizeEmptyNames();

            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
            string[] list = new string[4];

            for (int n = 0; n < 4; n++)
                list[n] = names[n].text;

            gameStartup.setNewGame(list, lastName.text);
            SceneManager.LoadScene(mainGameScene);
        }
        else
        {
            warning.gameObject.SetActive(true);
        }
    }

    bool isStringEmpty(string str)
    {
        if (str == "")
            return true;

        foreach (char c in str)
        {
            if (c != ' ')
                return false;
        }

        return true;
    }

    void randomizeEmptyNames()
    {
        for (int n = 0; n < 4; n++)
        {
            if (isStringEmpty(names[n].text))
            {
                generateRandomName(n, (n == (int)(FamilyMembers.famMember.Dad) || n == (int)(FamilyMembers.famMember.Son)));
            }
        }
    }

    public void generateRandomName(int member, bool isItMale)
    {
        if (isItMale)
        {
            if (maleNameList.Length > 0)
                names[member].text = maleNameList[UnityEngine.Random.Range(0, maleNameList.Length)];
            else
                names[member].text = ((maleNameList)(UnityEngine.Random.Range(0, Enum.GetNames(typeof(maleNameList)).Length))).ToString();
        }
        else
        {
            if (femaleNameList.Length > 0)
                names[member].text = femaleNameList[UnityEngine.Random.Range(0, femaleNameList.Length)];
            else 
                names[member].text = ((femaleNameList)(UnityEngine.Random.Range(0, Enum.GetNames(typeof(femaleNameList)).Length))).ToString();
        }
    }

    public void continueGame()
    {
        UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene(mainGameScene);
    }

    public void deleteData()
    {
        SaveLoad.Delete();
    }

    //Used in exiting the game
    public void exitGame()
    {
        Application.Quit();
    }

    //Menu management
    public void changeMenu(int to)
    {
        warning.gameObject.SetActive(false);

        if (to != (int)currentMenu && to <= (Enum.GetNames(typeof(menus))).Length)
        {

            foreach (Transform t in menuParents)
                t.gameObject.SetActive(false);

            menuParents[to].gameObject.SetActive(true);

            currentMenu = (menus)(to);
        }
    }


    //Used for loading class and stuff
    public void getClass(newGameClass data)
    {
        data.newGame = gameStartup.newGame;

        for (int n = 0; n < 4; n++ )
        {
            data.family[n] = gameStartup.family[n];
        }

        StartCoroutine(killObject());
    }

    public void signalLoaded()
    {
        dataLoaded = true;
        Destroy(gameObject);
    }

    IEnumerator killObject()
    {
        while (!dataLoaded)
        {
            yield return new WaitForSeconds(1f);

            if (dataLoaded)
                Destroy(this.gameObject);
        }
    }
}
