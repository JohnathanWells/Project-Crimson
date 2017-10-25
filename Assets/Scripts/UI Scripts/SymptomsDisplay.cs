using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymptomsDisplay : MonoBehaviour {

    [Header("Stats")]
    sicknessClass selectedSickness;
    public TextMesh symptomsText;

    Vector2 mousePos;

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        transform.position = (Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, +5)));
    }

    public void setSickness(sicknessClass newHighlight)
    {
        selectedSickness = newHighlight;
        updateText();
    }

    void updateText()
    {
        string result = "";
        string[] temp = selectedSickness.symptoms.Split(',');
        int count = 0;

        for (int n = 0; n < temp.Length; n++ )
        {
            count += temp[n].Length;
            if (count > 16)
            {
                result += "\n" + temp[n];
                count = 0;
            }
            else
            {
                if (n > 0)
                    result += " ";

                result += temp[n];
            }

            if (n < temp.Length - 1)
                result += ",";
        }
        
        symptomsText.text = result;
    }
}
