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
        for (int i = 0; i < numOfImages; i++)
        {
            byte[,] ByteImage = np.Load<byte[,]>(Application.dataPath + path[i]);
            int numOfRest = (int)(ByteImage.GetLength(0) * percentageOfData);
            double[,] pixelValues = new double[numOfRest, ByteImage.GetLength(1)];
            for (int j = 0; j < (int)(ByteImage.GetLength(0) * percentageOfData); j++)
            {
                for (int y = 0; y < ByteImage.GetLength(1); y++)
                {
                    pixelValues[j, y] = ByteImage[j, y] / 255;
                    //Adds noise to the Image
                    
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
            for (int j = (int)(ByteImage.GetLength(0) * percentageOfData); j < ByteImage.GetLength(0); j++)
            {
                for (int y = 0; y < ByteImage.GetLength(1); y++)
                {
                    pixelValues[index, y] = ByteImage[j, y] / 255;
                    //Adds noise to the Image
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
