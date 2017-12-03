using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class wikiClass{

    //public Queue<int> positionOfParents;
    public string title;
    public string content;
    public Sprite imageAttached;
    public List<wikiClass> subordinates = new List<wikiClass>();

    public wikiClass()
    {
        title = "Missing";
        content = "Empty";
        imageAttached = null;
    }

    public wikiClass(string tit, string cont)
    {
        title = tit;
        content = cont;
    }

    public wikiClass(int posInList, string tit, string cont, Sprite image)
    {
        title = tit;
        content = cont;
        imageAttached = image;
    }

    public void addSubordinate(wikiClass article, Queue<int> parentPosition)
    {
        if (parentPosition.Count <= 0)
        {
            subordinates.Add(article);
            Debug.Log(article.title + " added as a subordinate of " + title + ". Now counting " + subordinates.Count + " subs");
        }
        else
        {
            //Debug.Log("There are " + parentPosition.Count + " levels ahead");
            int subsubordinate = parentPosition.Dequeue();
            //Debug.Log(subsubordinate + " location out of " + subordinates.Count);

            if (subsubordinate < subordinates.Count && subsubordinate >= 0)
            {
                //Debug.Log("Going deeper");
                subordinates[subsubordinate].addSubordinate(article, parentPosition);
            }
            else
            {
                subordinates.Add(article);
                //Debug.Log(article.title + " added as a subordinate of " + title + ". Now counting " + subordinates.Count + " subs");
            }
        }
    }

    public void addSubordinate(wikiClass article)
    {
        subordinates.Add(article);
        //Debug.Log(article.title + " added as a subordinate of " + title + ". Now counting " + subordinates.Count + " subs");
    }
}
