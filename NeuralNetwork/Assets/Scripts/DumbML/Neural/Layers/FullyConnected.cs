using System;

namespace DumbML {
    public class InputLayer : Layer {
        public Placeholder ph;

        public InputLayer(params int[] shape) {
            ph = new Placeholder(shape);            
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
            var n = new Normal(-1, 1);

            weights = new Tensor(() => n.Next(),input.shape[0], outputSize);
            weights.SetName("FC weight");
            if (useBias) {
                bias = new Tensor(outputSize);
                return forward = af.Activate(new MatrixMult(input, weights) + bias);
            }
            else {
                return forward = af.Activate(new MatrixMult(input, weights));
            }
        }
    }
    public class Normal {
        static Random rng = new Random();

        float stdDev;
        float mean;
        public Normal(float min, float max) {
            mean = (max + min) / 2;
            stdDev = (max - min) / 4;
        }

        public float Next() {
            double u1 = 1.0 - rng.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rng.NextDouble();
            double randStdNormal =
                Math.Sqrt(-2.0 * Math.Log(u1)) *
                Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)

            double result = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return (float)result;
        }
    }
    public class TestLayer : Layer {
        FullyConnected a, b, c;
        bool useBias;
        int outputSize;
        ActivationFunction af;


        public TestLayer(int outputSize, ActivationFunction af = null, bool bias = true) {
            this.outputSize = outputSize;
            useBias = bias;
            this.af = af ?? ActivationFunction.None;
        }


        public override Operation Build(Operation input) {
            a = new FullyConnected(outputSize, af, useBias);
            b = new FullyConnected(outputSize, af, useBias);
            c = new FullyConnected(outputSize, af, useBias);

            var ab = a.Build(input);
            var bb = b.Build(input);
            var cb = c.Build(input);

            return new Append(ab, bb, cb);
        }
    }
}