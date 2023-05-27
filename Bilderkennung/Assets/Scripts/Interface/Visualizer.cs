using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Visualizer : MonoBehaviour
{

    [SerializeField] private Button changeViewNNet;
    [SerializeField] private Text BananaPosa;
    [SerializeField] private Text ChairPosa;
    [SerializeField] private GameObject PanelUI;
    [SerializeField] private int SizeOfDrawingSpace = 32;
    [SerializeField] private int SizeOfPixel = 20;
    [SerializeField] private int XDisplacement = 100;
    [SerializeField] private float displacementColor = .3f;
    [HideInInspector] public double[,] Drawing;
    private bool Switch = false;
    [HideInInspector] public float MouseX;
    [HideInInspector] public float MouseY;
    private float centerOffsetX;
    private float centerOffsetY;
    private static Texture2D staticRectTexture;
    private static GUIStyle staticRectStyle;

    private Texture2D DrawingTexture;

    private void Awake()
    {
        Drawing = new double[SizeOfDrawingSpace, SizeOfDrawingSpace];
        staticRectTexture = new Texture2D(1, 1);
        DrawingTexture = new Texture2D(SizeOfDrawingSpace, SizeOfDrawingSpace);
        staticRectStyle = new GUIStyle();
        ClearCanvas();
        network = new NeuralNetworkLayer(Application.dataPath + "/NetworkSaveFolder");
    }
    private void OnGUI()
    {

        centerOffsetX = (Screen.width / 2) - SizeOfDrawingSpace * SizeOfPixel / 2;
        centerOffsetY = (Screen.height / 2) - SizeOfDrawingSpace * SizeOfPixel / 2;

        MouseX = Input.mousePosition.x;
        MouseY = Input.mousePosition.y;
        ShowDrawing();
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearCanvas();
        }
    }
    public void ShowDrawing()
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
                            if (MouseY > centerOffsetY + SizeOfPixel * j && MouseY < centerOffsetY + SizeOfPixel * (j + 1))
                            {

                                float v = displacementColor;
                                if (i - 1 >= 0 && Drawing[i - 1, j] == 0 && Random.Range(0f, 1f) > 0.002f)
                                {
                                    print(Drawing[i - 1, j]);
                                    Drawing[i - 1, j] = v;
                                    DrawingTexture.SetPixel(i - 1, j, new Color(v, v, v));
                                }
                                if (i + 1 < Drawing.GetLength(0) && Drawing[i + 1, j] == 0 && Random.Range(0f, 1f) > 0.002f)
                                {
                                    Drawing[i + 1, j] = v;
                                    DrawingTexture.SetPixel(i + 1, j, new Color(v, v, v));
                                }
                                if (j - 1 >= 0 && Drawing[i, j - 1] == 0 && Random.Range(0f, 1f) > 0.002f)
                                {
                                    Drawing[i, j - 1] = v;
                                    DrawingTexture.SetPixel(i, j - 1, new Color(v, v, v));
                                }
                                if (j + 1 < Drawing.GetLength(0) && Drawing[i, j + 1] == 0 && Random.Range(0f, 1f) > 0.002f)
                                {
                                    Drawing[i, j + 1] = v;
                                    DrawingTexture.SetPixel(i, j + 1, new Color(v, v, v));
                                }

                                Drawing[i, j] = 1;
                                DrawingTexture.SetPixel(i, j, Color.white);
                            }
                        }

                    }
                }
            }
            GuessDrawing();
        }
        DrawingTexture.filterMode = FilterMode.Point;
        DrawingTexture.Apply();
        GUIDrawRectNormal(centerOffsetX - XDisplacement - 20, centerOffsetY - 20, SizeOfDrawingSpace * SizeOfPixel + 40, Color.gray);
        GUIDrawRect(centerOffsetX - XDisplacement, centerOffsetY, SizeOfDrawingSpace * SizeOfPixel, DrawingTexture);
    }
    NeuralNetworkLayer network;
    [SerializeField] public int[] LayerSizes;
    [SerializeField] public double LearningRate = 0.1;
    [SerializeField] public double momentumRate = 0.9;
    public void GuessDrawing()
    {
        double[] predicted;
        predicted = network.FeedForward(ArrayFlattener.Flatten<double>(Drawing));
        BananaPosa.text = predicted[0].ToString();
        ChairPosa.text = predicted[1].ToString();
        print(NameFinder.FindName(predicted));
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
