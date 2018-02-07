using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScaler : MonoBehaviour {

    public float targetAspectX = 4;
    public float targetAspectY = 3;

    public Vector2[] resolutions;
    int selectedRes = 0;
    float size;

    // Use this for initialization
    void Start()
    {

        Screen.SetResolution(Mathf.RoundToInt(resolutions[selectedRes].x), Mathf.RoundToInt(resolutions[selectedRes].y), false);
        size = 5.4f;

        //// set the desired aspect ratio (the values in this example are
        //// hard-coded for 16:9, but you could make them into public
        //// variables instead so you can set them at design time)
        //float targetaspect = targetAspectX / targetAspectY;

        //// determine the game window's current aspect ratio

        //float windowaspect = (float)Screen.width / (float)Screen.height;

        //// current viewport height should be scaled by this amount

        //float scaleheight = windowaspect / targetaspect;

        //// obtain camera component so we can modify its viewport

        //Camera camera = GetComponent<Camera>();

        //// if scaled height is less than current height, add letterbox

        //if (scaleheight < 1.0f)
        //{
        //    Rect rect = camera.rect;

        //    rect.width = 1.0f;

        //    rect.height = scaleheight;
        //    rect.x = 0;
        //    rect.y = (1.0f - scaleheight) / 2.0f;

        //    camera.rect = rect;

        //}
        //else // add pillarbox
        //{
        //    float scalewidth = 1.0f / scaleheight;

        //    Rect rect = camera.rect;

        //    rect.width = scalewidth;

        //    rect.height = 1.0f;
        //    rect.x = (1.0f - scalewidth) / 2.0f;
        //    rect.y = 0;

        //    camera.rect = rect;

        //}
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            selectedRes = (selectedRes + 1) % resolutions.Length;
            Screen.SetResolution(Mathf.RoundToInt(resolutions[selectedRes].x), Mathf.RoundToInt(resolutions[selectedRes].y), false);
            //updateSize();
        }
    }

    void updateSize()
    {
        size = Screen.width / resolutions[0].x * 5.4f;
        Camera.main.orthographicSize = size;
    }
}
