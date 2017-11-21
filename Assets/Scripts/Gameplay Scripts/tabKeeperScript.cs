using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tabKeeperScript : MonoBehaviour {
    
    public SpriteRenderer renderer;
    public Color offColor;
    public Color onColor;
    public int tabNumber;
    public Transform windowManager;
    bool active = false;


    public void setTabActive(bool value)
    {
        if (value)
        {
            renderer.color = onColor;
            active = true;
        }
        else
        {
            renderer.color = offColor;
            active = false;
        }
    }

    void OnMouseDown()
    {
        if (!active)
        {
            windowManager.SendMessage("updateTab", tabNumber);
        }
    }
}
