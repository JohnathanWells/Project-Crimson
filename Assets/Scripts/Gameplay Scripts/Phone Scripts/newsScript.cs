using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class newsScript : MonoBehaviour {

    //Main article info
    public Text newsHeader;
    public Text newsText;
    public Image picture;


    //Data regarding news
    List<newsClass> newsList;
    List<ObituaryClass> Obituaries = new List<ObituaryClass>();
    int currentNews = 0;

    public void updateInfo(GameManager manager)
    {
        newsList = manager.News;
        //Debug.Log(currentNews + "/" + newsList.Count);

        if (currentNews + 1 < newsList.Count && currentNews + 1>= 0)
        {
            if (newsList[currentNews + 1].date > manager.currentDay)
            {
                currentNews++;
                updateArticle(manager);
                return;
            }
        }
        else if (currentNews >= 0 && newsList.Count > 0)
        {
            //Debug.Log((newsList[currentNews].date <= manager.currentDay) + "\n" + newsList[currentNews].date.day + " vs " + manager.currentDay.day);
            if (newsList[currentNews].date.dayCount <= manager.currentDay.dayCount)
            {
                updateArticle(manager);
                return;
            }
        }
    }

    public void updateArticle(GameManager manager)
    {
        StartCoroutine(refreshText());

        newsClass temp = newsList[currentNews];

        //Debug.Log(temp.title + ": " + temp.content);

        newsHeader.text = temp.title;

        picture.sprite = temp.imageAttached;
        newsText.text = transormStringKeywords(temp.content, manager);
    }

    public string transormStringKeywords(string str, GameManager manager)
    {
        string[] words = str.Split(' ');
        string result = "";
        string tmp = "";

        foreach (string word in words)
        {
            if (word[0] == '#')
            {
                switch (word)
                {
                    case "#dn":
                        tmp = manager.currentDay.day.ToString();
                        break;
                    case "#mn":
                        tmp = manager.currentDay.month.ToString();
                        break;
                    case "#d":
                        tmp = manager.currentDay.calculateDayOfWeek().ToString();
                        break;
                    case "#m":
                        tmp = manager.currentDay.calculateMonth();
                        break;
                    case "#t":
                        tmp = manager.currentTime.ToString();
                        break;
                        //case "#"
                    case "#NL":
                        tmp = "\b\n";
                        break;
                    case "#c":
                        tmp = manager.City.stateName;
                        break;
                    case "#d1":
                        if (manager.City.districts.Length > 0)
                            tmp = manager.City.districts[0].districtName;
                        break;
                    case "#d2":
                        if (manager.City.districts.Length > 1)
                            tmp = manager.City.districts[1].districtName;
                        break;
                    case "#d3":
                        if (manager.City.districts.Length > 2)
                            tmp = manager.City.districts[2].districtName;
                        break;
                    case "#d4":
                        if (manager.City.districts.Length > 3)
                            tmp = manager.City.districts[3].districtName;
                        break;
                    case "#d5":
                        if (manager.City.districts.Length > 4)
                            tmp = manager.City.districts[4].districtName;
                        break;
                    case "#d6":
                        if (manager.City.districts.Length > 5)
                            tmp = manager.City.districts[5].districtName;
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
                                            tmp = manager.City.districts[distNum].currentCRMorning.ToString();
                                            break;
                                        case 'n':
                                            tmp = manager.City.districts[distNum].currentCRNoon.ToString();
                                            break;
                                        case 'e':
                                            tmp = manager.City.districts[distNum].currentCREvening.ToString();
                                            break;
                                        default:
                                            tmp = word;
                                            break;
                                    }
                                }
                                else if (word[4] == 't')
                                    tmp = manager.City.districts[distNum].traffic.ToString();
                                else
                                    tmp = word;
                            }
                            else
                                tmp = word;
                        }
                        else
                            tmp = word;

                        break;
                }

                if (result.Length > 0)
                    result += " " + tmp;
                else
                    result += tmp;
            }
            else
            {
                tmp = word;

                if (result.Length > 0)
                    result += " " + tmp;
                else
                    result += tmp;

            }
        }

        return result;

    }

    IEnumerator refreshText()
    {
        Vector2 displacement = new Vector2(0.0001f, 0);

        newsHeader.transform.Translate(displacement);
        newsText.transform.Translate(displacement);
        picture.transform.Translate(displacement);

        yield return new WaitForSeconds(0.0001f);

        newsHeader.transform.Translate(-displacement);
        newsText.transform.Translate(-displacement);
        picture.transform.Translate(-displacement);

    }
}
