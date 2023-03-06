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
        network = new NeuralNetworkLayer(LayerSizes, LearningRate, momentumRate);
        dataSet = new DataSet(NumOfImages, TrainingSplit);
        tester = new Tester(LayerSizes, LearningRate, momentumRate,dataSet,network );
        activationH = ActivationFunction;   
        //tester.XOR_Train(50);
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
    ActivationFunktion.Sigmoid Sigmoid = new ActivationFunktion.Sigmoid();
    ActivationFunktion.TanH TanH = new ActivationFunktion.TanH();
    ActivationFunktion.ReLU ReLU = new ActivationFunktion.ReLU();
    ActivationFunktion.SiLU SiLU = new ActivationFunktion.SiLU();
    public double Activation(double input)
    {
        switch (activationH)
        {
            case ActivationFunctionEnum.Sigmoid:
                return Sigmoid.Activate(input);
            case ActivationFunctionEnum.ReLU:
                return ReLU.Activate(input);
            case ActivationFunctionEnum.SiLU:
                return SiLU.Activate(input);
            case ActivationFunctionEnum.TanH:
                return TanH.Activate(input);
            default:
                return TanH.Activate(input);
        }
    }
    public double Derivative(double input)
    {
        switch (activationH)
        {
            case ActivationFunctionEnum.Sigmoid:
                return Sigmoid.Derivative(input);
            case ActivationFunctionEnum.ReLU:
                return ReLU.Derivative(input);
            case ActivationFunctionEnum.SiLU:
                return SiLU.Derivative(input);
            case ActivationFunctionEnum.TanH:
                return TanH.Derivative(input);
            default:
                return TanH.Derivative(input);
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



