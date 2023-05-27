using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static System.Math;
using static UnityEngine.Random;

public class NeuralNetworkLayer : MonoBehaviour
{
    int[] layerSizes;
    Layer[] layers;
    MetaData metaData;

    public NeuralNetworkLayer(int[] layerSizes, double learinigRate, double momentumRate, ActivationFunctionEnum activationH, ActivationFunctionEnumOutput activationO)
    {
        metaData = new MetaData();
        this.layerSizes = new int[layerSizes.Length];
        for (int i = 0; i < layerSizes.Length; i++)
            this.layerSizes[i] = layerSizes[i];

        layers = new Layer[layerSizes.Length - 1];

        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = new Layer(layerSizes[i], layerSizes[i + 1], learinigRate, momentumRate, activationH, activationO);
        }
        metaData.layerSizes = layerSizes;
        metaData.learinigRate = learinigRate;
        metaData.momentumRate = momentumRate;
        metaData.activationH = activationH;
        metaData.activationO = activationO;

    }
    public NeuralNetworkLayer(string Path)
    {
        string metaDataString = File.ReadAllText(Path + $"/MetaData.txt");
        MetaData metaData = JsonUtility.FromJson<MetaData>(metaDataString);
        this.layerSizes = metaData.layerSizes;

        this.layerSizes = new int[metaData.layerSizes.Length];
        for (int i = 0; i < metaData.layerSizes.Length; i++)
            this.layerSizes[i] = metaData.layerSizes[i];

        layers = new Layer[layerSizes.Length - 1];

        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = new Layer(layerSizes[i], layerSizes[i + 1], metaData.learinigRate, metaData.momentumRate, metaData.activationH, metaData.activationO);
        }
        LoadNetwork(Path);
    }
    public double[] FeedForward(double[] inputs)
    {
        layers[0].CalculateForward(inputs);
        for (int i = 1; i < layers.Length - 1; i++)
        {
            layers[i].CalculateForward(layers[i - 1].outputs);
        }
        layers[layers.Length - 1].CalculateOutputLayer(layers[layers.Length - 2].outputs);
        return layers[layers.Length - 1].outputs;
    }
    public void SaveNetwork()
    { 
        for (int i = 0; i < layers.Length; i++)
        {
            string LayerString = JsonUtility.ToJson(layers[i]);
            File.WriteAllText(Application.dataPath + $"/NetworkSaveFolder/Layer-{i}.txt", LayerString);
        }
        string metaDataString = JsonUtility.ToJson(metaData);
        File.WriteAllText(Application.dataPath + $"/NetworkSaveFolder/MetaData.txt", metaDataString);
    }
    public void LoadNetwork(string Path)
    {
        for(int i = 0; i < layerSizes.Length -1; i++)
        {
            string LayerString = File.ReadAllText(Path + $"/Layer-{i}.txt");
            Layer layer = JsonUtility.FromJson<Layer>(LayerString);
            layers[i].biases = layer.biases;
            layers[i].flattendWeights = layer.flattendWeights;
            layers[i].numOfInputs = layer.numOfInputs;
            layers[i].numOfOutputs = layer.numOfOutputs;
            layers[i].UnflattenWeights();
        }
    }

    public void BackProp(double[] expeted)
    {

        for (int i = layers.Length - 1; i >= 0; i--)
        {
            if (i == layers.Length - 1)
            {
                layers[i].BackPropOutputLayer(expeted);
            }
            else
            {
                layers[i].BackPropHiddenLayer(layers[i + 1].gamma, layers[i + 1].weights);
            }
        }
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].UpdateWeightsBiases();
        }
    }
    public class Layer
    {
        public int numOfInputs;
        public int numOfOutputs;
        public double[] biases;
        public double[] flattendWeights;
        [System.NonSerialized] ActivationFunctionEnum activationH;
        [System.NonSerialized] ActivationFunctionEnumOutput activationO;
        [System.NonSerialized] public double learinigRate;
        [System.NonSerialized] public double momentumRate;
        [System.NonSerialized] public double[] inputs;
        [System.NonSerialized] public double[] outputs;
        [System.NonSerialized] public double[,] weights;
        [System.NonSerialized] public double[,] weightsDelta;
        [System.NonSerialized] public double[,] prevweightsDelta;
        [System.NonSerialized] public double[] biasesDelta;
        [System.NonSerialized] public double[] prevbiasesDelta;
        [System.NonSerialized] public double[] gamma;
        [System.NonSerialized] public double[] error;
        //Initialize
        public Layer(int numOfInputs, int numOfOutputs, double learinigRate, double momentumRate, ActivationFunctionEnum activationH, ActivationFunctionEnumOutput activationO)
        {
            this.activationH = activationH;
            this.activationO = activationO;
            this.numOfInputs = numOfInputs;
            this.numOfOutputs = numOfOutputs;
            this.learinigRate = learinigRate;
            this.momentumRate = momentumRate;
            outputs = new double[numOfOutputs];
            inputs = new double[numOfInputs];
            weights = new double[numOfOutputs, numOfInputs];
            biases = new double[numOfOutputs];
            weightsDelta = new double[numOfOutputs, numOfInputs];
            biasesDelta = new double[numOfOutputs];
            prevweightsDelta = new double[numOfOutputs, numOfInputs];
            prevbiasesDelta = new double[numOfOutputs];
            gamma = new double[numOfOutputs];
            error = new double[numOfOutputs];
            InitilizeWeights();
        }
        public Layer(int numOfInputs, int numOfOutputs, double learinigRate, double momentumRate)
        {
            this.numOfInputs = numOfInputs;
            this.numOfOutputs = numOfOutputs;
            this.learinigRate = learinigRate;
            this.momentumRate = momentumRate;
            outputs = new double[numOfOutputs];
            inputs = new double[numOfInputs];
            weights = new double[numOfOutputs, numOfInputs];
            biases = new double[numOfOutputs];
            weightsDelta = new double[numOfOutputs, numOfInputs];
            biasesDelta = new double[numOfOutputs];
            prevweightsDelta = new double[numOfOutputs, numOfInputs];
            prevbiasesDelta = new double[numOfOutputs];
            gamma = new double[numOfOutputs];
            error = new double[numOfOutputs];
            InitilizeWeights();
        }
        //Calculates the normal path of the Network -> Forward
        public double[] CalculateForward(double[] inputs)
        {
            this.inputs = inputs;
            for (int i = 0; i < numOfOutputs; i++)
            {
                outputs[i] = biases[i];
                for (int j = 0; j < numOfInputs; j++)
                {
                    outputs[i] += inputs[j] * weights[i, j];
                }
                outputs[i] = ActivationHidden(outputs[i]);
            }
            return outputs;
        }
        public double[] CalculateOutputLayer(double[] inputs)
        {
            this.inputs = inputs;
            for (int i = 0; i < numOfOutputs; i++)
            {
                outputs[i] = biases[i];
                for (int j = 0; j < numOfInputs; j++)
                {
                    outputs[i] += inputs[j] * weights[i, j];
                }
                outputs[i] = ActivationOutput(outputs[i]);
            }
            return outputs;
        }
        //randomizes the weights and sets Biases to zero
        public void InitilizeWeights()
        {
            for (int i = 0; i < numOfOutputs; i++)
            {
                for (int j = 0; j < numOfInputs; j++)
                {
                    weights[i, j] = Random.Range(-1f, 1f);
                }
                biases[i] = 0;
            }
        }
        public void UnflattenWeights()
        {
            weights = ArrayFlattener.Unflatten<double>(flattendWeights, numOfOutputs, numOfInputs);
        }
        //Assing new Weight/Bias Values to the Neural Network
        //Applying Momentum to the delta and saving it to the "prev" (prevoius) weights/biases
        public void UpdateWeightsBiases()
        {
            for (int i = 0; i < numOfOutputs; i++)
            {
                for (int j = 0; j < numOfInputs; j++)
                {
                    double weightDelta = momentumRate * prevweightsDelta[i, j] + (1 - momentumRate) * weightsDelta[i, j];
                    weights[i, j] -= weightDelta * learinigRate;
                    prevweightsDelta[i, j] = weightDelta;
                }
                double biasDelta = momentumRate * prevbiasesDelta[i] + (1 - momentumRate) * biasesDelta[i];
                biases[i] -= biasDelta * learinigRate;
                prevbiasesDelta[i] = biasDelta;
            }
            flattendWeights = ArrayFlattener.Flatten<double>(weights);
        }
        //Calculates the deltas from the OutputLayer
        public void BackPropOutputLayer(double[] expected)
        {
            for (int i = 0; i < numOfOutputs; i++)
            {
                error[i] = outputs[i] - expected[i];
            }
            for (int i = 0; i < numOfOutputs; i++)
            {
                gamma[i] = error[i] * DerivativeOutput(outputs[i]);
            }
            for (int i = 0; i < numOfOutputs; i++)
            {
                for (int j = 0; j < numOfInputs; j++)
                {
                    weightsDelta[i, j] = gamma[i] * inputs[i] ;
                }
                biasesDelta[i] = gamma[i];
            }
        }
        //Calculates the deltas from the hidden Layers
        public void BackPropHiddenLayer(double[] gammaForward, double[,] weightsForward)
        {
            for (int i = 0; i < numOfOutputs; i++)
            {
                gamma[i] = 0;
                for (int j = 0; j < gammaForward.Length; j++)
                {
                    gamma[i] += gammaForward[j] * weightsForward[j, i];
                }
                gamma[i] *= DerivativeHidden(outputs[i]);
            }
            for (int i = 0; i < numOfOutputs; i++)
            {
                for (int j = 0; j < numOfInputs; j++)
                {
                    weightsDelta[i, j] = gamma[i] * inputs[j];
                }
                biasesDelta[i] = gamma[i];
            }
        }
        public double ActivationHidden(double input)
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
        public double ActivationOutput(double input)
        {
            switch (activationO)
            {
                case ActivationFunctionEnumOutput.Sigmoid:
                    return ActivationFunktion.Sigmoid.Activate(input);
                case ActivationFunctionEnumOutput.ReLU:
                    return ActivationFunktion.ReLU.Activate(input);
                case ActivationFunctionEnumOutput.SiLU:
                    return ActivationFunktion.SiLU.Activate(input);
                case ActivationFunctionEnumOutput.TanH:
                    return ActivationFunktion.TanH.Activate(input);
                default:
                    return ActivationFunktion.TanH.Activate(input);
            }
        }
        public double DerivativeHidden(double input)
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
        public double DerivativeOutput(double input)
        {
            switch (activationO)
            {
                case ActivationFunctionEnumOutput.Sigmoid:
                    return ActivationFunktion.Sigmoid.Derivative(input);
                case ActivationFunctionEnumOutput.ReLU:
                    return ActivationFunktion.ReLU.Derivative(input);
                case ActivationFunctionEnumOutput.SiLU:
                    return ActivationFunktion.SiLU.Derivative(input);
                case ActivationFunctionEnumOutput.TanH:
                    return ActivationFunktion.TanH.Derivative(input);
                default:
                    return ActivationFunktion.TanH.Derivative(input);
            }
        }
    }
    public class MetaData
    {
        public int[] layerSizes;
        public double momentumRate;
        public double learinigRate;
        public ActivationFunctionEnum activationH;
        public ActivationFunctionEnumOutput activationO;
    }
}


