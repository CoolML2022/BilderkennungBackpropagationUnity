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
    NeuralNetworkLayer network;
    //0 0 0 -> 0
    //0 0 1 -> 1
    //0 1 0 -> 1
    //1 0 0 -> 1
    //1 1 0 -> 0
    //0 1 1 -> 0
    //1 0 1 -> 0
    //1 1 1 -> 1
    public Tester(int[] layerSizes, double learningRate, double momentumRate, DataSet dataSet, NeuralNetworkLayer network)
    {
        this.layerSizes = layerSizes;
        this.learningRate = learningRate;
        this.momentumRate = momentumRate;
        this.data = dataSet;
        this.network = network;
    }
    public void Train(int NumOfIteration)
    {
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
        for (int i = 0; i < NumOfIterationBeforeTest; i++)
        {       
            randomImage = UnityEngine.Random.Range(0, numOfImages);
            randomImageIndex = UnityEngine.Random.Range(0, data.trainValues[randomImage].GetLength(0));
            expectedOutput = new double[numOfImages];
            expectedOutput[randomImage] = 1;
            network.FeedForward(data.trainValues[randomImage].GetRow<double>(randomImageIndex));
            network.BackProp(expectedOutput);         
        }
        //Debug.Log("Time-Learn: " + (Time.realtimeSinceStartup - startTime) * 1000f + "ms");
        //
        //
        //RunRandomTest
        //
        //
        randomImage = UnityEngine.Random.Range(0, numOfImages);
        randomImageIndex = UnityEngine.Random.Range(0, data.testValues[randomImage].GetLength(0));
        double[] predictedOutput = network.FeedForward(data.testValues[randomImage].GetRow<double>(randomImageIndex));
        string predicted = FindName(FindMaxValueIndex(predictedOutput));
        //Debug.Log("Expected: " + FindName(ImageIndexTest) + "Networks prediction: " + predicted + " Time: "+ (Time.realtimeSinceStartup - startTime) * 1000f + "ms" + " Iteration: " + Counter * NumOfIterationBeforeTest);
        if (FindMaxValueIndex(predictedOutput) == randomImage)
        {
            correctCounter++;
        }
        Counter++;
        Debug.Log("Accuracy: " + (correctCounter / Counter) * 100 + "%");

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
}




