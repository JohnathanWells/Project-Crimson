using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class newsScript : MonoBehaviour {

    //Main article info
    public Text newsHeader;
    [Tooltip("0 is for simple picture, 1 is for articles with picture")]
    public Text [] newsText = new Text[2];
    public Image picture;


    //Data regarding news
    List<newsClass> newsList;
    List<ObituaryClass> Obituaries = new List<ObituaryClass>();
    int currentNews = 0;

    public void updateInfo(GameManager manager)
    {
        newsList = manager.News;

        if (currentNews + 1 < newsList.Count)
        {
            if (newsList[currentNews + 1].date > manager.currentDay)
            {
                currentNews++;
                return;
            }
        }
    }

    public void updateArticle(GameManager manager)
    {
        newsClass temp = newsList[currentNews];

        newsHeader.text = temp.title;
        
        if (!temp.featuresImage)
        {
            newsText[0].text = transormStringKeywords(temp.content, manager);
        }
        else
        {
            picture.sprite = temp.imageAttached;
            newsText[1].text = transormStringKeywords(temp.content, manager);
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
}
