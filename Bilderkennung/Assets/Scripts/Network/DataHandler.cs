using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using UnityEngine.UI;
using System;
using System.IO;
using NumSharp;
using System.Linq;
using System.Runtime.InteropServices;
public class DataHandler : MonoBehaviour
{
    public UnityEngine.UI.RawImage dispaly;
    private string[] path;
    byte[] image;
    private static GUIStyle Style1;
    public int numOfLearningIterations;
    public int NumOfDrawings;
    Texture2D ImageTexture;
    void Start()
    {

        path = new string[12] {"/Data/Data-npy/full_numpy_bitmap_anvil.npy",
                               "/Data/Data-npy/full_numpy_bitmap_banana.npy",
                               "/Data/Data-npy/full_numpy_bitmap_car.npy",
                               "/Data/Data-npy/full_numpy_bitmap_chair.npy",
                               "/Data/Data-npy/full_numpy_bitmap_clock.npy",
                               "/Data/Data-npy/full_numpy_bitmap_fence.npy",
                               "/Data/Data-npy/full_numpy_bitmap_fish.npy",
                               "/Data/Data-npy/full_numpy_bitmap_helicopter.npy",
                               "/Data/Data-npy/full_numpy_bitmap_house.npy",
                               "/Data/Data-npy/full_numpy_bitmap_mountain.npy",
                               "/Data/Data-npy/full_numpy_bitmap_octopus.npy",
                               "/Data/Data-npy/full_numpy_bitmap_stairs.npy"};
        Style1 = new GUIStyle();
        image = SelectImage(UnityEngine.Random.Range(0, 3));
        ImageTexture = ConvertToTexture2d(image);
    }

    public int GetFlatIndex(int x, int y)
    {
        return y * 28 + x;
    }
    public static byte[] ImageToByte(Bitmap img)
    {
        ImageConverter converter = new ImageConverter();
        return (byte[])converter.ConvertTo(img, typeof(byte[]));
    }
    private void OnGUI()
    {

        //GUIDrawRect(300, 200, 300, Drawing);
        GUIDrawRect(300, 200, 300, ImageTexture, Style1);

        //GUIDrawRect(700, 200, 300, Drawing1, Style2);
    }
    public Texture2D ConvertToTexture2d(byte[] pixelGrayScale)
    {
        Texture2D texture = new Texture2D(28, 28);
        Color32[] colors = new Color32[pixelGrayScale.Length];
        for (int i = pixelGrayScale.Length - 1; i >=0 ; i--)
        {
            byte v = pixelGrayScale[i];
            colors[pixelGrayScale.Length - i - 1] = new Color32(v, v, v, 255);
        }
        texture.SetPixels32(colors);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        return texture;
    }
    public static byte[] SelectImage(int index)
    {
        string[] path = new string[12] {"/Data/Data-npy/full_numpy_bitmap_banana.npy",
                                        "/Data/Data-npy/full_numpy_bitmap_house.npy",
                                        "/Data/Data-npy/full_numpy_bitmap_fish.npy",
                                        "/Data/Data-npy/full_numpy_bitmap_anvil.npy",                               
                                        "/Data/Data-npy/full_numpy_bitmap_car.npy",
                                        "/Data/Data-npy/full_numpy_bitmap_chair.npy",
                                        "/Data/Data-npy/full_numpy_bitmap_clock.npy",
                                        "/Data/Data-npy/full_numpy_bitmap_fence.npy",                              
                                        "/Data/Data-npy/full_numpy_bitmap_helicopter.npy",                               
                                        "/Data/Data-npy/full_numpy_bitmap_mountain.npy",
                                        "/Data/Data-npy/full_numpy_bitmap_octopus.npy",
                                        "/Data/Data-npy/full_numpy_bitmap_stairs.npy"};
        byte[] image;
        byte[,] Arry1 = np.Load<byte[,]>(Application.dataPath + path[index]);
        image = Arry1.GetRow<byte>((int)UnityEngine.Random.Range(0, Arry1.GetLength(0)));
        return image;
    }
    public static void GUIDrawRect(float xPos, float yPos, int size, Texture2D texture2D, GUIStyle style)
    {
        style.normal.background = texture2D;
        GUI.Box(new Rect(xPos, yPos, size, size), GUIContent.none, style);
    }
}
public static class ArrayExt
{
    public static T[] GetRow<T>(this T[,] array, int row)
    {
        if (!typeof(T).IsPrimitive)
            throw new InvalidOperationException("Not supported for managed types.");

        if (array == null)
            throw new ArgumentNullException("array");

        int cols = array.GetUpperBound(1) + 1;
        T[] result = new T[cols];

        int size;

        if (typeof(T) == typeof(bool))
            size = 1;
        else if (typeof(T) == typeof(char))
            size = 2;
        else
            size = Marshal.SizeOf<T>();

        Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);

        return result;
    }
}

public class DataSet
{
    public List<double[,]> trainValues = new List<double[,]>();
    public List<double[,]> testValues = new List<double[,]>();
    private string[] path = new string[12] {
        "/Data/Data-npy/full_numpy_bitmap_banana.npy",
        "/Data/Data-npy/full_numpy_bitmap_house.npy",
        "/Data/Data-npy/full_numpy_bitmap_fish.npy",
        "/Data/Data-npy/full_numpy_bitmap_anvil.npy",
        "/Data/Data-npy/full_numpy_bitmap_car.npy",
        "/Data/Data-npy/full_numpy_bitmap_chair.npy",
        "/Data/Data-npy/full_numpy_bitmap_clock.npy",
        "/Data/Data-npy/full_numpy_bitmap_fence.npy",
        "/Data/Data-npy/full_numpy_bitmap_helicopter.npy",
        "/Data/Data-npy/full_numpy_bitmap_mountain.npy",
        "/Data/Data-npy/full_numpy_bitmap_octopus.npy",
        "/Data/Data-npy/full_numpy_bitmap_stairs.npy"};

    public DataSet(int numOfImages, float percentageOfData)
    {
        for(int i = 0; i< numOfImages; i++)
        {
            byte[,] ByteImage = np.Load<byte[,]>(Application.dataPath + path[i]);
            int numOfRest = (int)(ByteImage.GetLength(0) * percentageOfData);
            double[,] pixelValues = new double[numOfRest, ByteImage.GetLength(1)];
            for (int j = 0; j < (int)(ByteImage.GetLength(0) * percentageOfData); j++)
            {               
                for(int y = 0; y < ByteImage.GetLength(1); y++)
                {
                    pixelValues[j, y] = ByteImage[j, y] / 255;
                    
                    if(UnityEngine.Random.Range(0f, 1f) <= 0.09 && pixelValues[j, y] == 0)
                    {
                        pixelValues[i, y] = UnityEngine.Random.Range(0f, .2f);
                    }
                    
                }
            }
            trainValues.Add(pixelValues);
            int index = 0;
            Debug.Log(ByteImage.GetLength(1));
            pixelValues = new double[ByteImage.GetLength(0) - numOfRest, ByteImage.GetLength(1)];
            for(int j = (int)(ByteImage.GetLength(0) * percentageOfData); j < ByteImage.GetLength(0); j++)
            {
                for(int y = 0; y < ByteImage.GetLength(1); y++)
                {
                    pixelValues[index, y] = ByteImage[j, y] / 255;
                    /*
                    if (UnityEngine.Random.Range(0f, 1f) <= 0.09 && pixelValues[index, y] == 0)
                    {
                        pixelValues[i, y] = UnityEngine.Random.Range(0f, 1f);
                    }
                    */
                }
                index++;
            }
            testValues.Add(pixelValues);
            
        }       
    }
}



