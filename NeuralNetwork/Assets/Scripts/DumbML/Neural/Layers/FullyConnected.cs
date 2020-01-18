
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
            weights = Tensor.Random(input.shape[0], outputSize);
            if (useBias) {
                bias = new Tensor(outputSize);
                return forward = af.Activate(new MatrixMult(input, weights) + bias);
            }
            else {
                return forward = af.Activate(new MatrixMult(input, weights));
            }
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