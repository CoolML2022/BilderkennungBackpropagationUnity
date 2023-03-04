using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawSurface : MonoBehaviour
{

    public int drawWidth;

    private RectTransform drawSurfaceRectTransform;
    private Texture2D drawSurfaceTexture;
    private float drawSurfaceWidth;
    private float drawSurfaceHeight;

    private Vector2 localPointerPosition;

    // Use this for initialization
    void Start()
    {
        drawSurfaceRectTransform = this.gameObject.GetComponent<RectTransform>();
        drawSurfaceWidth = drawSurfaceRectTransform.rect.width;
        drawSurfaceHeight = drawSurfaceRectTransform.rect.height;
        drawSurfaceTexture = new Texture2D((int)drawSurfaceWidth, (int)drawSurfaceHeight);
        this.gameObject.GetComponent<Image>().material.mainTexture = drawSurfaceTexture;

        // Reset all pixels color to transparent
        Color32 resetColor = new Color32(255, 255, 255, 255);
        Color32[] resetColorArray = drawSurfaceTexture.GetPixels32();

        for (int i = 0; i < resetColorArray.Length; i++)
        {
            resetColorArray[i] = resetColor;
        }

        drawSurfaceTexture.SetPixels32(resetColorArray);
        drawSurfaceTexture.Apply();

        //Debug.Log(GetInstanceID() + " - Started");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            Color drawColor = Color.red;
            if (Input.GetMouseButton(1))
            {
                drawColor = Color.red;
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(drawSurfaceRectTransform, Input.mousePosition, null))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(drawSurfaceRectTransform, Input.mousePosition, null, out localPointerPosition);
                for (int i = -(drawWidth / 2); i < (drawWidth / 2); i++)
                {
                    for (int j = -(drawWidth / 2); j < (drawWidth / 2); j++)
                    {
                        drawSurfaceTexture.SetPixel((int)(localPointerPosition.x + (drawSurfaceWidth / 2) + i), (int)(localPointerPosition.y + (drawSurfaceHeight / 2) + j), drawColor);
                    }
                }
                drawSurfaceTexture.Apply();
                Debug.Log(drawSurfaceTexture.GetInstanceID() + " - Drawn");
                //Debug.Log(GetInstanceID() + " - Drawn");
            }
            this.gameObject.GetComponent<Image>().material.mainTexture = drawSurfaceTexture;
        }
    }
}

