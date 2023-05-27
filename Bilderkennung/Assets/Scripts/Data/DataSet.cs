using System.Collections.Generic;
using UnityEngine;
using System;
using NumSharp;
using System.Linq;
using System.Runtime.InteropServices;

public class DataSet
{
    public List<double[,]> trainValues = new List<double[,]>();
    public List<double[,]> testValues = new List<double[,]>();
    private string[] path = new string[12] {
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

    public DataSet(int numOfImages, float trainSplit)
    {
        for (int i = 0; i < numOfImages; i++)
        {
            byte[,] ByteImage = np.Load<byte[,]>(Application.dataPath + path[i]);
            int numOfRest = (int)(ByteImage.GetLength(0) * trainSplit);
            double[,] pixelValues = new double[numOfRest, ByteImage.GetLength(1)];
            for (int j = 0; j < (int)(ByteImage.GetLength(0)* trainSplit); j++)
            {
                for (int y = 0; y < ByteImage.GetLength(1); y++)
                {                    
                    pixelValues[j, y] = ByteImage[j, y] / 255;                  
                }
            }
            trainValues.Add(pixelValues);
            int index = 0;
            pixelValues = new double[ByteImage.GetLength(0) - numOfRest, ByteImage.GetLength(1)];
            Debug.Log(ByteImage.GetLength(0));
            Debug.Log((int)(ByteImage.GetLength(0) * trainSplit));
            for (int j = (int)(ByteImage.GetLength(0)* trainSplit); j < ByteImage.GetLength(0); j++)
            {
                for (int y = 0; y < ByteImage.GetLength(1); y++)
                {                 
                    pixelValues[index, y] = ByteImage[j, y] / 255;
                }
                index++;
            }
            testValues.Add(pixelValues);
        }
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
    public static int GetFlatIndex(int x, int y)
    {
        return y * 28 + x;
    }
}
public static class ArrayFlattener
{

    public static T[] Flatten<T>(T[,] input)
    {
        int width = input.GetLength(0);
        int height = input.GetLength(1);
        T[] flattened = new T[width * height];

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                flattened[j * width + i] = input[i, j];

            }
        }

        return flattened;
    }


    public static T[,] Unflatten<T>(T[] input, int width, int height)
    {
        T[,] unflattened = new T[width, height];

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                unflattened[i, j] = input[j * width + i];

            }
        }

        return unflattened;

    }

}
public static class NameFinder
{
    public static string FindName(double[] input)
    {
        double maxValue = input.Max();
        int maxIndex = input.ToList().IndexOf(maxValue);

        string name = "";
        if (maxIndex == 0)
            name = "Banana";
        else if (maxIndex == 1)
            name = "Chair";
        else if (maxIndex == 2)
            name = "House";
        else if (maxIndex == 3)
            name = "Fish";
        else if (maxIndex == 4)
            name = "Anvil";
        else if (maxIndex == 5)
            name = "Car";
        else if (maxIndex == 6)
            name = "Clock";
        else if (maxIndex == 7)
            name = "Fence";
        else if (maxIndex == 8)
            name = "Helicopter";
        else if (maxIndex == 9)
            name = " Mountain";
        else if (maxIndex == 10)
            name = "Octopus";
        else if (maxIndex == 11)
            name = "Stairs";

        return name;
    }
}
