using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

public class Tester : MonoBehaviour
{
    // Start is called before the first frame update
    private int[] layerSizes;
    private double learningRate;
    private double momentumRate;
    private DataSet data;
    private float MinAccuracy;
    NeuralNetworkLayer network;
    Visualizer visualizer;
    public TransformationSettings settings;
    public Tester(int[] layerSizes, double learningRate, double momentumRate, DataSet dataSet, NeuralNetworkLayer network, float minAccuracy)
    {
        this.layerSizes = layerSizes;
        this.learningRate = learningRate;
        this.momentumRate = momentumRate;
        this.data = dataSet;
        this.network = network;
        this.MinAccuracy = minAccuracy;
    }
    
    private void Awake()
    {
        visualizer = new Visualizer();
    }
    public void XOR_Train(int NumOfIteration)
    {

        //0 0 0 -> 0
        //0 0 1 -> 1
        //0 1 0 -> 1
        //1 0 0 -> 1
        //1 1 0 -> 0
        //0 1 1 -> 0
        //1 0 1 -> 0
        //1 1 1 -> 1
        /*
        NeuralNetworkLayer net = new NeuralNetworkLayer(layerSizes, learningRate, momentumRate);

        for (int i = 0; i < NumOfIteration; i++)
        {
            net.FeedForward(new double[] { 0, 0, 0 });
            net.BackProp(new double[] { 0 });

            net.FeedForward(new double[] { 0, 1, 0 });
            net.BackProp(new double[] { 1 });

            net.FeedForward(new double[] { 1, 0, 0 });
            net.BackProp(new double[] { 1 });

            net.FeedForward(new double[] { 0, 0, 1 });
            net.BackProp(new double[] { 1 });

            net.FeedForward(new double[] { 1, 1, 1 });
            net.BackProp(new double[] { 1 });

            net.FeedForward(new double[] { 0, 1, 1 });
            net.BackProp(new double[] { 0 });

            net.FeedForward(new double[] { 1, 0, 1 });
            net.BackProp(new double[] { 0 });

            net.FeedForward(new double[] { 1, 1, 0 });
            net.BackProp(new double[] { 0 });

            net.FeedForward(new double[] { 1, 1, 1 });
            net.BackProp(new double[] { 1 });
        }
        Debug.Log("0 0 0: " + net.FeedForward(new double[] { 0, 0, 0 })[0]);
        Debug.Log("0 1 0: " + net.FeedForward(new double[] { 0, 1, 0 })[0]);
        Debug.Log("1 0 0: " + net.FeedForward(new double[] { 1, 0, 0 })[0]);
        Debug.Log("0 0 1: " + net.FeedForward(new double[] { 0, 0, 1 })[0]);
        Debug.Log("1 1 1: " + net.FeedForward(new double[] { 1, 1, 1 })[0]);
        Debug.Log("0 1 1: " + net.FeedForward(new double[] { 0, 1, 1 })[0]);
        */
    }
    public float Counter = 1;
    public float correctCounter = 0;
    public void TrainWithData(int NumOfIterationBeforeTest)
    {
        
        float startTime = Time.realtimeSinceStartup;
        int numOfImages = data.trainValues.Count();
        int randomImage;
        int randomImageIndex;
        double[] expectedOutput;
        for (int j = 0; j < NumOfIterationBeforeTest; j++)
        {
            for (int i = 0; i < numOfImages; i++)
            {
                
                randomImageIndex = Random.Range(0, data.trainValues[i].GetLength(0));
                expectedOutput = new double[numOfImages];
                double[] inputValues = new double[28 * 28];
                expectedOutput[i] = 1;                
                inputValues = data.trainValues[i].GetRow<double>(randomImageIndex);
                //RandomizeSettings(inputValues);
                //inputValues = TransformImage(inputValues, settings, 28);            
                network.FeedForward(inputValues);
                network.BackProp(expectedOutput);
            }
        }
        //Debug.Log("Time-Learn: " + (Time.realtimeSinceStartup - startTime) * 1000f + "ms");
        //
        //
        //RunRandomTest
        //
        //
        randomImage = Random.Range(0, numOfImages);
        randomImageIndex = Random.Range(0, data.testValues[randomImage].GetLength(0));
        double[] predictedOutput = network.FeedForward(data.testValues[randomImage].GetRow<double>(randomImageIndex));
        //Checks if the predicted Output equals the correct Output
        if (FindMaxValueIndex(predictedOutput) == randomImage)
        {
            correctCounter++;
        }
        Counter++;
        float accuracy = correctCounter / Counter;
        Debug.Log("Accuracy: " + accuracy * 100 + "%");
        if(accuracy >= MinAccuracy)
        {
            network.SaveNetwork();
            print("Saved");
        }
            
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
                transformedImage[GetFlatIndex(x, y, 28)] = System.Math.Clamp(pixelValue + noiseValue * 255, 0, 255);// * 255;
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
    public void RandomizeSettings(double[] values)
    {
        settings = CreateRandomSettings(new System.Random(), values);
    }
    private int FindMaxValueIndex(double[] inp)
    {
        double maxValue = inp.Max();
        int maxIndex = inp.ToList().IndexOf(maxValue);
        return maxIndex;
    }
    private string FindName(int index)
    {
        string name = "";
        if (index == 0)
            name = "BANANA ";
        else if (index == 1)
            name = "HOUSE ";
        else if (index == 2)
            name = "FISH ";
        return name;
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




