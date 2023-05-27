using static System.Math;
public class ActivationFunktion
{
    public static class Sigmoid
    {
        public static double Activate(double inputs)
        {
            return 1.0 / (1 + Exp(-inputs));
        }
        public static double Derivative(double inputs)
        {
            double a = Activate(inputs);
            return a * (1 - a);
        }
    }
    public static class TanH
    {
        public static double Activate(double inputs)
        {
            double e2 = Exp(2 * inputs);
            return (e2 - 1) / (e2 + 1);
        }
        public static double Derivative(double inputs)
        {
            double e2 = Exp(2 * inputs);
            double t = (e2 - 1) / (e2 + 1);
            return 1 - t * t;
        }
    }
    public static class ReLU
    {
        public static double Activate(double inputs)
        {
            return Max(0, inputs);
        }
        public static double Derivative(double inputs)
        {
            return (inputs > 0) ? 1 : 0;
        }
    }
    public static class SiLU 
    {
        public static double Activate(double inputs)
        {
            return inputs / (1 + Exp(-inputs));
        }
        public static double Derivative(double inputs)
        {
            double sig = 1 / (1 + Exp(-inputs));
            return inputs * sig * (1 - sig) + sig;
        }
    }
}
