using static System.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ActivationFunktion
{
    public class Sigmoid
    {
        public double Activate(double inputs)
        {
            return 1.0 / (1 + Exp(-inputs));
        }
        public double Derivative(double inputs)
        {
            double a = Activate(inputs);
            return a * (1 - a);
        }
    }

    public class TanH
    {
        public double Activate(double inputs)
        {
            double e2 = Exp(2 * inputs);
            return (e2 - 1) / (e2 + 1);

        }

        public double Derivative(double inputs)
        {
            double e2 = Exp(2 * inputs);
            double t = (e2 - 1) / (e2 + 1);
            return 1 - t * t;
        }
    }
    public class ReLU
    {
        public double Activate(double inputs)
        {
            return Max(0, inputs);
        }
        public double Derivative(double inputs)
        {
            return (inputs > 0) ? 1 : 0;
        }
    }

    public class SiLU 
    {
        public double Activate(double inputs)
        {
            return inputs / (1 + Exp(-inputs));
        }
        public double Derivative(double inputs)
        {
            double sig = 1 / (1 + Exp(-inputs));
            return inputs * sig * (1 - sig) + sig;
        }
    }
}
