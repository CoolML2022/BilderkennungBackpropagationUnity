using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NumSharp;

public class TestTransform : MonoBehaviour
{
    public TransformationSettings settings;
    double[] values;
    double[] changedValues;
    byte[] byteValues;
    Texture2D texture;
    Texture2D texture1;
    private static GUIStyle Style1;
    int index;
    void Start()
    {
        values = new double[28 * 28];
        changedValues = new double[28 * 28];
        byteValues = new byte[28 * 28];
        Style1 = new GUIStyle();
        (byteValues, index) = SelectImage();
        values = ByteToDouble(byteValues);
        foreach (var x in values)
        {
            print(x);
        }

        texture1 = ConvertToTexture2d(values);
        RandomizeSettings();
        values = TransformImage(values, settings, 28);
        foreach (var x in values)
        {
            print(x);
        }
        texture = ConvertToTexture2d(values);
    }


    // Update is called once per frame
    private void OnGUI()
    {
        //RandomizeSettings();

        changedValues = TransformImage(values, settings, 28);
        texture = ConvertToTexture2d(changedValues);

        GUIDrawRect(300, 300, 300, texture, Style1);
        GUIDrawRect(800, 300, 300, texture1, Style1);
        //values = new double[28 * 28];
    }
    public double[] ByteToDouble(byte[] inp)
    {
        double[] outP = new double[inp.Length];
        for (int i = 0; i < inp.Length; i++)
        {
            outP[i] = inp[i];
        }
        return outP;
    }
    public Texture2D ConvertToTexture2d(double[] pixelGrayScale)
    {
        Texture2D texture = new Texture2D(28, 28);
        Color32[] colors = new Color32[pixelGrayScale.Length];
        for (int i = pixelGrayScale.Length - 1; i >= 0; i--)
        {
            byte v = (byte)pixelGrayScale[i];
            colors[pixelGrayScale.Length - i - 1] = new Color32(v, v, v, 255);
        }
        texture.SetPixels32(colors);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return texture;
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
        int index = Random.Range(0, 2);
        byte[,] Arry1 = np.Load<byte[,]>(Application.dataPath + path[index]);
        image = Arry1.GetRow<byte>((int)UnityEngine.Random.Range(0, Arry1.GetLength(0)));
        return (image, index);
    }
    public double[] TransformImage(double[] inputImage, TransformationSettings settings, int size)
    {
        System.Random rng = new System.Random(settings.noiseSeed);
        double[] transformedImage = new double[28 * 28];
        Vector2 iHat = new Vector2(Mathf.Cos(settings.angle), Mathf.Sin(settings.angle)) / settings.scale;
        Vector2 jHat = new Vector2(-iHat.y, iHat.x);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                double u = x / (size - 1.0);
                double v = y / (size - 1.0);

                double uTransformed = iHat.x * (u - 0.5) + jHat.x * (v - 0.5) + 0.5 - settings.offset.x;
                double vTransformed = iHat.y * (u - 0.5) + jHat.y * (v - 0.5) + 0.5 - settings.offset.y;
                double pixelValue = Sample(uTransformed, vTransformed, inputImage);
                double noiseValue = 0;
                if (rng.NextDouble() <= settings.noiseProbability)
                {
                    noiseValue = (rng.NextDouble() - 0.5) * settings.noiseStrength;
                }
                transformedImage[GetFlatIndex(x, y, 28)] = System.Math.Clamp(pixelValue + noiseValue*255, 0, 255);// * 255;
            }
        }
        return transformedImage;
    }
    public double Sample(double u, double v, double[] pixelValues)
    {
        u = System.Math.Max(System.Math.Min(1, u), 0);
        v = System.Math.Max(System.Math.Min(1, v), 0);

        double texX = u * (28 - 1);
        double texY = v * (28 - 1);

        int indexLeft = (int)(texX);
        int indexBottom = (int)(texY);
        int indexRight = System.Math.Min(indexLeft + 1, 28 - 1);
        int indexTop = System.Math.Min(indexBottom + 1, 28 - 1);

        double blendX = texX - indexLeft;
        double blendY = texY - indexBottom;

        double bottomLeft = pixelValues[GetFlatIndex(indexLeft, indexBottom, 28)];
        double bottomRight = pixelValues[GetFlatIndex(indexRight, indexBottom, 28)];
        double topLeft = pixelValues[GetFlatIndex(indexLeft, indexTop, 28)];
        double topRight = pixelValues[GetFlatIndex(indexRight, indexTop, 28)];

        double valueBottom = bottomLeft + (bottomRight - bottomLeft) * blendX;
        double valueTop = topLeft + (topRight - topLeft) * blendX;
        double interpolatedValue = valueBottom + (valueTop - valueBottom) * blendY;
        return interpolatedValue;
    }
    public static void GUIDrawRect(float xPos, float yPos, int size, Texture2D texture2D, GUIStyle style)
    {
        style.normal.background = texture2D;
        GUI.Box(new Rect(xPos, yPos, size, size), GUIContent.none, style);
    }
    public static int GetFlatIndex(int x, int y, int size)
    {
        return y * size + x;
    }
    static double RandomInNormalDistribution(System.Random prng, double mean = 0, double standardDeviation = 1)
    {
        double x1 = 1 - prng.NextDouble();
        double x2 = 1 - prng.NextDouble();

        double y1 = System.Math.Sqrt(-2.0 * System.Math.Log(x1)) * System.Math.Cos(2.0 * System.Math.PI * x2);
        return y1 * standardDeviation + mean;
    }
    public TransformationSettings CreateRandomSettings(System.Random rng, double[] image)
    {
        TransformationSettings settings = new TransformationSettings();
        settings.angle = (float)RandomInNormalDistribution(rng) * 0.15f;
        settings.scale = 1 + (float)RandomInNormalDistribution(rng) * 0.1f;

        settings.noiseSeed = rng.Next();
        settings.noiseProbability = (float)System.Math.Min(rng.NextDouble(), rng.NextDouble()) * 0.05f;
        settings.noiseStrength = (float)System.Math.Min(rng.NextDouble(), rng.NextDouble());


        int boundsMinX = 28;
        int boundsMaxX = 0;
        int boundsMinY = 28;
        int boundsMaxY = 0;

        for (int y = 0; y < 28; y++)
        {
            for (int x = 0; x < 28; x++)
            {
                if (image[GetFlatIndex(x, y, 28)] > 0)
                {
                    boundsMinX = Mathf.Min(boundsMinX, x);
                    boundsMaxX = Mathf.Max(boundsMaxX, x);
                    boundsMinY = Mathf.Min(boundsMinY, y);
                    boundsMaxY = Mathf.Max(boundsMaxY, y);
                }
            }
        }


        float offsetMinX = -boundsMinX / (float)28;
        float offsetMaxX = (28 - boundsMaxX) / (float)28;
        float offsetMinY = -boundsMinY / (float)28;
        float offsetMaxY = (28 - boundsMaxY) / (float)28;

        float offsetX = Mathf.Lerp(offsetMinX, offsetMaxX, (float)rng.NextDouble());
        float offsetY = Mathf.Lerp(offsetMinY, offsetMaxY, (float)rng.NextDouble());
        settings.offset = new Vector2(offsetX, offsetY) * 0.8f;


        return settings;
    }
    public void RandomizeSettings()
    {
        settings = CreateRandomSettings(new System.Random(), values);
    }
    [System.Serializable]
    public struct TransformationSettings
    {
        public float angle;
        public float scale;
        public Vector2 offset;
        public int noiseSeed;
        [Range(0, 1)] public float noiseProbability;
        [Range(0, 1)] public float noiseStrength;
    }
}
