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
            int x = 0;
            if (input.shape.Length == 1) {
                x = input.shape[0];
            }
            else {
                x = input.shape[1];
            }


            weights = new Tensor(() => RNG.Normal(), x, outputSize);
            weights.SetName($"{weights.shape.ContentString()} FullyConnected weight");


            if (useBias) {

                var mm = new MatrixMult(input, weights);
                bias = new Tensor(mm.shape);
                bias.SetName($"{bias.shape.ContentString()} FullyConnected bias");
                return forward = af.Activate(mm + bias);
            }
            else {
                return forward = af.Activate(new MatrixMult(input, weights));
            }
        }
    }
}
