using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using NumSharp;

public class ImageViewTest : MonoBehaviour
{
    public Text predictedImage;
    private NeuralNetworkLayer network;
    public int NumOfImages;
    private Texture2D currentImage;
    private double[] InputValues;
    private static GUIStyle Style1;
    private float correct;
    private float counter = 1;


    void Start()
    {
        Style1 = new GUIStyle();
        //(currentImage, InputValues) = ConvertToTexture2d(SelectImage(Random.Range(0, NumOfImages)));
        network = new NeuralNetworkLayer(Application.dataPath + "/NetworkSaveFolder");

    }

    private void OnGUI()
    {
        
        //TestAccuracy();
        GUIDrawRect(300, 300, 300, currentImage, Style1);
        //Debug.Log((correct / counter) * 100f + "%");
        //counter++;
       // print("Counter: " + counter);
       // print("Correct: " + correct);
          
    }

    public (Texture2D, double[]) ConvertToTexture2d(byte[] pixelGrayScale)
    {
        Texture2D texture = new Texture2D(28, 28);
        Color32[] colors = new Color32[pixelGrayScale.Length];
        double[] inputvalues = new double[pixelGrayScale.Length];
        for (int i = pixelGrayScale.Length - 1; i >= 0; i--)
        {
            byte v = pixelGrayScale[i];
            colors[pixelGrayScale.Length - i - 1] = new Color32(v, v, v, 255);
            inputvalues[i] = pixelGrayScale[i] / 255;
        }
        texture.SetPixels32(colors);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return (texture, inputvalues);
    }
    public (byte[], int) SelectImage()
    {
        string[] path = new string[12] {
        "/Data/Data-npy/full_numpy_bitmap_banana.npy",
        "/Data/Data-npy/full_numpy_bitmap_chair.npy",
        "/Data/Data-npy/full_numpy_bitmap_house.npy",
        "/Data/Data-npy/full_numpy_bitmap_fish.npy",
        "/Data/Data-npy/full_numpy_bitmap_anvil.npy",
        "/Data/Data-npy/full_numpy_bitmap_car.npy",
        "/Data/Data-npy/full_numpy_bitmap_clock.npy",
        "/Data/Data-npy/full_numpy_bitmap_fence.npy",
        "/Data/Data-npy/full_numpy_bitmap_helicopter.npy",
        "/Data/Data-npy/full_numpy_bitmap_mountain.npy",
        "/Data/Data-npy/full_numpy_bitmap_octopus.npy",
        "/Data/Data-npy/full_numpy_bitmap_stairs.npy"};
        byte[] image;
        int index = Random.Range(0, NumOfImages);
        byte[,] Arry1 = np.Load<byte[,]>(Application.dataPath + path[index]);
        image = Arry1.GetRow<byte>((int)UnityEngine.Random.Range(0, Arry1.GetLength(0)));
        return (image, index);
    }
    public void ButtonClick()
    {      
        double[] outputValue;
        
        int correctIndex;
        byte[] image;
        (image, correctIndex) = SelectImage();
        (currentImage, InputValues) = ConvertToTexture2d(image);
        outputValue = network.FeedForward(InputValues);
        predictedImage.text = NameFinder.FindName(outputValue);
        Debug.Log(PrintArray(outputValue));

    }
    public void TestAccuracy()
    {
        double[] outputValue;
        
        byte[] image;
        int correctIndex;
        (image, correctIndex) = SelectImage();
        (currentImage, InputValues) = ConvertToTexture2d(image);
        outputValue = network.FeedForward(InputValues);
        predictedImage.text = NameFinder.FindName(outputValue);
        //Debug.Log(PrintArray(outputValue));
        if (FindMaxValueIndex(outputValue) == correctIndex)
            correct++;
    }
    public static void GUIDrawRect(float xPos, float yPos, int size, Texture2D texture2D, GUIStyle style)
    {
        style.normal.background = texture2D;
        GUI.Box(new Rect(xPos, yPos, size, size), GUIContent.none, style);
    }

    public static string PrintArray(double[] input)
    {
        string finished = "";
        for (int i = 0; i < input.Length; i++)
        {
            finished += input[i].ToString() + ", ";
        }
        return finished;
    }
    private int FindMaxValueIndex(double[] inp)
    {
        double maxValue = inp.Max();
        int maxIndex = inp.ToList().IndexOf(maxValue);
        return maxIndex;
    }
}
