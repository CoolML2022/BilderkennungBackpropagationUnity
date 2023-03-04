using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Visualizer : MonoBehaviour
{
    
    [SerializeField] private Button changeViewNNet;
    [SerializeField] private GameObject PanelUI;
    [SerializeField] private bool ShowWeights = false;
    [SerializeField] private int SizeOfDrawingSpace = 32;
    [SerializeField] private int SizeOfPixel = 20;
    [SerializeField] private int XDisplacement = 100;
    [HideInInspector] public int[,] Drawing;
    private bool Switch = false;
    public float MouseX;
    public float MouseY;
    private float centerOffsetX;
    private float centerOffsetY;
    bool Clicked = false;
    bool onceCalled = false;

    private static Texture2D staticRectTexture;
    private static GUIStyle staticRectStyle;

    private Texture2D DrawingTexture;

    private void Awake()
    {
        Drawing = new int[SizeOfDrawingSpace, SizeOfDrawingSpace];
        staticRectTexture = new Texture2D(1, 1);
        DrawingTexture = new Texture2D(SizeOfDrawingSpace, SizeOfDrawingSpace);
        staticRectStyle = new GUIStyle();
        
        ClearCanvas();
    }
    private void OnGUI()
    {

        centerOffsetX = (Screen.width / 2) - SizeOfDrawingSpace * SizeOfPixel / 2;
        centerOffsetY = (Screen.height / 2) - SizeOfDrawingSpace * SizeOfPixel / 2;


        MouseX = Input.mousePosition.x;
        MouseY = Input.mousePosition.y;
        ShowDrawing();
    }
    void ShowDrawing()
    {
        if (Input.GetMouseButton(0))
        {
            for (int i = 0; i < Drawing.GetLength(0); i++)
            {
                for (int j = 0; j < Drawing.GetLength(1); j++)
                {
                    if (i != Drawing.GetLength(0) && j != Drawing.GetLength(1))
                    {
                        if ((MouseX > centerOffsetX - XDisplacement + SizeOfPixel * i) && (MouseX < centerOffsetX - XDisplacement + SizeOfPixel * (i + 1)))
                        {
                            if(MouseY > centerOffsetY + SizeOfPixel * j && MouseY < centerOffsetY + SizeOfPixel * (j + 1))
                            {
                                Drawing[i, j] = 1;
                                DrawingTexture.SetPixel(i, j, Color.white);
                            }
                        }
                    }
                }
            }
        }
        DrawingTexture.filterMode = FilterMode.Point;
        DrawingTexture.Apply();
        GUIDrawRectNormal(centerOffsetX - XDisplacement - 20, centerOffsetY - 20, SizeOfDrawingSpace * SizeOfPixel + 40, Color.gray);
        GUIDrawRect(centerOffsetX - XDisplacement, centerOffsetY, SizeOfDrawingSpace * SizeOfPixel, DrawingTexture);      
    }

    public void ClearCanvas()
    {
        for (int i = 0; i < Drawing.GetLength(0); i++)
        {
            for (int j = 0; j < Drawing.GetLength(1); j++)
            {
                Drawing[i, j] = 0;
                DrawingTexture.SetPixel(i, j, Color.black);
            }
        }
        DrawingTexture.Apply();
    }
    public static void GUIDrawRect(float xPos, float yPos, int size, Texture2D texture2D)
    {
        staticRectStyle.normal.background = texture2D;
        GUI.Box(new Rect(xPos, yPos, size, size), GUIContent.none, staticRectStyle);
    }
    public static void GUIDrawRectNormal(float xPos, float yPos, int size, Color color)
    {
        staticRectTexture.SetPixel(0, 0, color);
        staticRectTexture.Apply();
        staticRectStyle.normal.background = staticRectTexture;
        GUI.Box(new Rect(xPos, yPos, size, size), GUIContent.none, staticRectStyle);

    }

    public void switchScenes()
    {
        Switch = !Switch;
    }
    
}
