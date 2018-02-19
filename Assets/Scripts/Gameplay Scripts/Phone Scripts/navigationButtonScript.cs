using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class navigationButtonScript : MonoBehaviour {


    public int buttonNumber;
    public Text text;
    public Transform expansionSymbol;
    public Transform wikiMnager;
    wikiClass linkedArticle;


    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //List<int> instructions = new List<int>();
            //Debug.Log("click on " + text.text);
            wikiMnager.SendMessage("addToInstructions", buttonNumber);
            wikiMnager.SendMessage("updateArticle", linkedArticle);
        }
    }

    public void setButton(wikiClass article)
    {
        linkedArticle = article;

        text.text = article.title;

        if (article.subordinates.Count > 0)
            expansionSymbol.gameObject.SetActive(true);
        else
            expansionSymbol.gameObject.SetActive(false);
    }

    public void emptyButton()
    {
        text.text = "";
        expansionSymbol.gameObject.SetActive(false);
    }
}
