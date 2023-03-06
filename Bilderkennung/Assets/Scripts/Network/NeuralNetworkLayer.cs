using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static UnityEngine.Random;

public class NeuralNetworkLayer : MonoBehaviour
{
    int[] layerSizes;
    Layer[] layers;
    public NeuralNetworkLayer(int[] layerSizes, double learinigRate, double momentumRate)
    {
        this.layerSizes = new int[layerSizes.Length];
        for (int i = 0; i < layerSizes.Length; i++)
            this.layerSizes[i] = layerSizes[i];

        layers = new Layer[layerSizes.Length - 1];

        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = new Layer(layerSizes[i], layerSizes[i + 1], learinigRate, momentumRate);
        }
    }
    public double[] FeedForward(double[] inputs)
    {
        layers[0].CalculateForward(inputs);
        for (int i = 1; i < layers.Length; i++)
        {
            layers[i].CalculateForward(layers[i - 1].outputs);
        }
        return layers[layers.Length - 1].outputs;
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
        int numOfInputs;
        int numOfOutputs;
        double learinigRate;
        double momentumRate;
        public double[] outputs;
        public double[] inputs;
        public double[,] weights;
        public double[] biases;
        public double[,] weightsDelta;
        public double[,] prevweightsDelta;
        public double[] biasesDelta;
        public double[] prevbiasesDelta;
        public double[] gamma;
        public double[] error;
        NetworkSettings network = new NetworkSettings();
        //Initialize
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
                outputs[i] = network.Activation(outputs[i]);
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
                gamma[i] = error[i] * network.Derivative(outputs[i]);
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
                gamma[i] *= network.Derivative(outputs[i]);
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
    }
}


