using DumbML;

namespace Chess {
    public class ChessModel {
        public Trainer trainer;
        public NeuralNetwork main;

        public ChessModel(int evalCount) {
            main = new NeuralNetwork(8 * 8 * 12 + 5);
            //main.Add(new FullyConnected(300, ActivationFunction.LeakyRelu, false));
            //main.Add(new FullyConnected(100, ActivationFunction.LeakyRelu, false));
            //main.Add(new FullyConnected(32, ActivationFunction.LeakyRelu, false));
            main.Add(new ParallelNetwork(GetInner(), GetInner(), GetInner()));
            main.Add(new FullyConnected(1, ActivationFunction.Tanh, false));
            main.Build();
            //main.SetTrainer(new Adam(), Loss.MSE);
            trainer = new Trainer(main, new Adam(), Loss.MSE);

            NeuralNetwork GetInner() {
                var result = new NeuralNetwork(8 * 8 * 12 + 5);
                result.Add(new FullyConnected(100, ActivationFunction.Sigmoid, true));
                result.Add(new FullyConnected(33, ActivationFunction.Sigmoid, false));
                result.Add(new FullyConnected(10, ActivationFunction.Sigmoid, false));
                return result;
            }
        }

        public float Compute(Tensor position) {
            var output = main.Compute(position);

            return output[0];
        }
    }
}