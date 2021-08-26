using System;

namespace DumbML {
    public class InputLayer : Layer {
        public Placeholder ph;

        public InputLayer(params int[] shape) {
            ph = new Placeholder("Input", shape);
        }
        public InputLayer(string name, params int[] shape) {
            ph = new Placeholder(name, shape);
        }
        public override Operation Build(Operation input) {
            return ph;
        }
        public Placeholder Build() {
            return ph;
        }
    }
    public class FullyConnected : Layer {
        public Variable weights, bias;
        bool useBias;
        int outputSize;
        ActivationFunction af;

        public FullyConnected(int outputSize, ActivationFunction af = null, bool bias = true) {
            this.outputSize = outputSize;
            useBias = bias;
            this.af = af ?? ActivationFunction.None;
        }

        public override Operation Build(Operation input) {
            //weights = Tensor.Random(input.shape[0], outputSize);

            weights = new Tensor(() => LPE.RNG.Normal(),input.shape[0], outputSize);
            weights.SetName($"{weights.shape.ContentString()} FullyConnected weight");
            if (useBias) {
                bias = new Tensor(outputSize);
                bias.SetName($"{bias.shape.ContentString()} FullyConnected bias");
                return forward = af.Activate(new MatrixMult(input, weights) + bias);
            }
            else {
                return forward = af.Activate(new MatrixMult(input, weights));
            }
        }
    }
}

namespace LPE {    
    public static class RNG {
        static Random rng = new Random();

        public static float Normal() { return Normal(0, 1); }
        public static float Normal(float mean, float stdDev) {
            double u1 = 1.0 - rng.NextDouble(); // uniform(0,1] random doubles
            double u2 = 1.0 - rng.NextDouble();
            double randStdNormal =
                Math.Sqrt(-2.0 * Math.Log(u1)) *
                Math.Sin(2.0 * Math.PI * u2); // random normal(0,1)

            double result = mean + stdDev * randStdNormal; // random normal(mean,stdDev^2)
            return (float)result;
        }
    }
}