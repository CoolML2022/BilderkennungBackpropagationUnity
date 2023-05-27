using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSettings : MonoBehaviour
{
    [Header("Network Settings")]
    [SerializeField] public int[] LayerSizes;
    [SerializeField] public double LearningRate = 0.1;
    [SerializeField] public double momentumRate = 0.9;
    [SerializeField] public ActivationFunctionEnum ActivationFunction;
    [SerializeField] public ActivationFunctionEnumOutput ActivationFunctionOutput;
    [Range(0, 1)] public float MinAccuracy = .6f;
    [Header("Train Settings")]
    [SerializeField] public int NumOfTrainingBevoreTest = 10;
    [SerializeField] private int NumOfImages;
    [Range(0, 1)] public float TrainingSplit;


    private static ActivationFunctionEnum activationH;
    Tester tester;
    NeuralNetworkLayer network;
    DataSet dataSet;
    private bool stopTraining = false;
    //public bool StopTraining = false;
    private void Awake()
    {
        network = new NeuralNetworkLayer(LayerSizes, LearningRate, momentumRate, ActivationFunction, ActivationFunctionOutput);
        dataSet = new DataSet(NumOfImages, TrainingSplit);
        tester = new Tester(LayerSizes, LearningRate, momentumRate, dataSet, network, MinAccuracy);
        //tester = new Tester(LayerSizes, LearningRate, momentumRate);
        activationH = ActivationFunction;
        //tester.XOR_Train(5000);
        //tester.TrainWithData(NumOfTrainingBevoreTest);
        //tester.LoadTest();
    }
    private void OnGUI()
    {
        
        if (!stopTraining)
            tester.TrainWithData(NumOfTrainingBevoreTest);
        
    }
    public void StopTraining()
    {
        stopTraining = !stopTraining;
    }
    public double Activation(double input)
    {
        switch (activationH)
        {
            case ActivationFunctionEnum.Sigmoid:
                return ActivationFunktion.Sigmoid.Activate(input);
            case ActivationFunctionEnum.ReLU:
                return ActivationFunktion.ReLU.Activate(input);
            case ActivationFunctionEnum.SiLU:
                return ActivationFunktion.SiLU.Activate(input);
            case ActivationFunctionEnum.TanH:
                return ActivationFunktion.TanH.Activate(input);
            default:
                return ActivationFunktion.TanH.Activate(input);
        }
    }
    public double Derivative(double input)
    {
        switch (activationH)
        {
            case ActivationFunctionEnum.Sigmoid:
                return ActivationFunktion.Sigmoid.Derivative(input);
            case ActivationFunctionEnum.ReLU:
                return ActivationFunktion.ReLU.Derivative(input);
            case ActivationFunctionEnum.SiLU:
                return ActivationFunktion.SiLU.Derivative(input);
            case ActivationFunctionEnum.TanH:
                return ActivationFunktion.TanH.Derivative(input);
            default:
                return ActivationFunktion.TanH.Derivative(input);
        }
    }
}
public enum ActivationFunctionEnum
{
    TanH,
    ReLU,
    SiLU,
    Sigmoid
};
public enum ActivationFunctionEnumOutput
{
    TanH,
    ReLU,
    SiLU,
    Sigmoid
};



