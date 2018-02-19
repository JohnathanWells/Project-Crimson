using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wikiScript : MonoBehaviour {

    //Manager
    GameManager manager;

    //Wiki Buttons
    public Transform[] navigationButtons;
    public Transform backtrackButton;

    //Main article info
    public Text newsHeader;
    public Text articleText;

    //Data regarding articles
    wikiClass masterArticle = new wikiClass();
    public List<int> navigationIndexes = new List<int>();

    bool hasBeenOpened = false;

    //void Start()
    //{
    //    masterArticle.title = "Information";
    //    updateArticle(masterArticle);
    //}

    public void updateInfo(GameManager mngr)
    {
        StartCoroutine(refreshText());
        manager = mngr;
        masterArticle = manager.wikiDatabase;

        if (!hasBeenOpened)
        {
            updateArticle(masterArticle);
            hasBeenOpened = true;
        }
        //else
        //{
        //    updateArticle
        //}

        //if (currentNews + 1 < newsList.Count)
        //{
        //    if (newsList[currentNews + 1].date > manager.currentDay)
        //    {
        //        currentNews++;
        //        return;
        //    }
        //}
    }

    public void addToInstructions(int buttonNumber)
    {
        navigationIndexes.Add(buttonNumber);
    }

    public void openArticle(List<int> instructions)
    {
        //Debug.Log("opening article");
        if (instructions.Count > 0)
        {
            wikiClass temp = masterArticle;

            StartCoroutine(refreshText());

            for (int n = 0; n < instructions.Count; n++)
            {
                temp = temp.subordinates[instructions[n]];
            }

            updateArticle(temp);
        }
        else
            updateArticle(masterArticle);
    }

    public void backtrack()
    {
        if (navigationIndexes.Count > 0)
        {
            navigationIndexes.RemoveAt(navigationIndexes.Count - 1);
            openArticle(navigationIndexes);
        }
    }

    public void mainWikiMenu()
    {
        navigationIndexes.Clear();
        openArticle(navigationIndexes);
    }

    public void updateArticle(wikiClass article)
    {
        newsHeader.text = article.title;

        //Debug.Log("Instructions: ");
        //foreach (int n in navigationIndexes)
        //    Debug.Log("[" + n + "]");

        if (navigationIndexes.Count > 0)
            backtrackButton.gameObject.SetActive(true);
        else
            backtrackButton.gameObject.SetActive(false);

        if (article.subordinates.Count == 0)
        {
            foreach (Transform t in navigationButtons)
            {
                t.gameObject.SetActive(false);
                //t.SendMessage("emptyButton");
            }
            //if (!article.featuresImage)
            //{
                articleText.text = transormStringKeywords(article.content, manager);
            //}
            //else
            //{
            //    picture.sprite = temp.imageAttached;
            //    newsText[1].text = transormStringKeywords(article.content, manager);
            //}
        }
        else
        {
            articleText.text = "";
            int displayedButtons = Mathf.Min(navigationButtons.Length, article.subordinates.Count);
            for (int n = 0; n < displayedButtons; n++)
            {
                navigationButtons[n].gameObject.SetActive(true);
                navigationButtons[n].SendMessage("setButton", article.subordinates[n]);
            }

            for (int n = displayedButtons; n < navigationButtons.Length; n++)
            {
                //navigationButtons[n].SendMessage("emptyButton");
                navigationButtons[n].gameObject.SetActive(false);
            }
        }
    }

    public string transormStringKeywords(string str, GameManager manager)
    {
        string[] words = str.Split(' ');
        string result = "";
        string temp = "";

        foreach (string word in words)
        {
            if (word[0] == '#')
            {
                switch (word)
                {
                    case "#dn":
                        temp = manager.currentDay.day.ToString();
                        break;
                    case "#mn":
                        temp = manager.currentDay.month.ToString();
                        break;
                    case "#d":
                        temp = manager.currentDay.calculateDayOfWeek().ToString();
                        break;
                    case "#m":
                        temp = manager.currentDay.calculateMonth();
                        break;
                    case "#c":
                        temp = manager.City.stateName;
                        break;
                    case "#d1":
                        if (manager.City.districts.Length > 0)
                            temp = manager.City.districts[0].districtName;
                        break;
                    case "#d2":
                        if (manager.City.districts.Length > 1)
                            temp = manager.City.districts[1].districtName;
                        break;
                    case "#d3":
                        if (manager.City.districts.Length > 2)
                            temp = manager.City.districts[2].districtName;
                        break;
                    case "#d4":
                        if (manager.City.districts.Length > 3)
                            temp = manager.City.districts[3].districtName;
                        break;
                    case "#d5":
                        if (manager.City.districts.Length > 4)
                            temp = manager.City.districts[4].districtName;
                        break;
                    case "#d6":
                        if (manager.City.districts.Length > 5)
                            temp = manager.City.districts[5].districtName;
                        break;
                    case "#cprght":
                        temp = "©";
                        break;
                    default:
                        if (word[0] == '#' && word[1] == 'd' && word.Length > 2)
                        {
                            int distNum = int.Parse(word[2].ToString()) - 1;

                            if (distNum < manager.City.districts.Length && word.Length > 3)
                            {
                                if (word[3] == 'c' && word.Length > 4)
                                {
                                    switch (word[4])
                                    {
                                        case 'm':
                                            temp = manager.City.districts[distNum].currentCRMorning.ToString();
                                            break;
                                        case 'n':
                                            temp = manager.City.districts[distNum].currentCRNoon.ToString();
                                            break;
                                        case 'e':
                                            temp = manager.City.districts[distNum].currentCREvening.ToString();
                                            break;
                                        default:
                                            temp = word;
                                            break;
                                    }
                                }
                                else if (word[4] == 't')
                                    temp = manager.City.districts[distNum].traffic.ToString();
                                else
                                    temp = word;
                            }
                            else
                                temp = word;
                        }
                        else
                            temp = word;

                        break;
                }

                result += " " + temp;
            }
            else
            {
                result += " " + word;
            }
        }

        return result;

    }

    IEnumerator refreshText()
    {
        Vector2 displacement = new Vector2(0.0001f, 0);
        //Debug.Log("doin it");
        newsHeader.transform.Translate(displacement);
        articleText.transform.Translate(displacement);
        foreach (Transform t in navigationButtons)
            t.Translate(displacement);

        yield return new WaitForSeconds(0.001f);

        newsHeader.transform.Translate(-displacement);
        articleText.transform.Translate(-displacement);
        foreach (Transform t in navigationButtons)
            t.Translate(-displacement);

        //Debug.Log("finished");
    }
}
