using DumbML;

namespace Chess {
    public class ChessModel {
        //public NeuralNetwork[] evaluators;
        public NeuralNetwork main;

        public ChessModel(int evalCount) {
            //evaluators = new NeuralNetwork[evalCount];

            //for (int i = 0; i < evalCount; i++) {
            //    var nn = new NeuralNetwork(8 * 8 * 12 + 5);

            //    nn.Add(new FullyConnected(100, ActivationFunction.LeakyRelu));
            //    nn.Add(new FullyConnected(1, ActivationFunction.Sigmoid));

            //    nn.Build();
            //    nn.SetOptimizer(new Adam(), Loss.MSE);
            //    evaluators[i] = nn; 
            //}


            //main = new NeuralNetwork(evalCount);
            main = new NeuralNetwork(8 * 8 * 12 + 5);
            //main.Add(new FullyConnected(300, ActivationFunction.LeakyRelu, false));
            //main.Add(new FullyConnected(100, ActivationFunction.LeakyRelu, false));
            //main.Add(new FullyConnected(32, ActivationFunction.LeakyRelu, false));
            main.Add(new ParallelNetwork(GetInner(), GetInner(), GetInner()));
            main.Add(new FullyConnected(1, ActivationFunction.Tanh, false));
            main.Build();
            main.SetOptimizer(new Adam(), Loss.MSE);

            NeuralNetwork GetInner() {
                var result = new NeuralNetwork(8 * 8 * 12 + 5);
                result.Add(new FullyConnected(100, ActivationFunction.Sigmoid, true));
                result.Add(new FullyConnected(33, ActivationFunction.Sigmoid, false));
                result.Add(new FullyConnected(10, ActivationFunction.Sigmoid, false));
                return result;
            }
        }

        public float Compute(Tensor position) {
            //var sub = GetSubEvaluation(position);

            var output = main.Compute(position);

            return output[0];
        }
        //public float ComputeFromSubEval(Tensor eval) {
        //    return main.Compute(eval)[0];
        //}
        //public Tensor GetSubEvaluation(Tensor position) {
        //    Tensor result = new Tensor(evaluators.Length);

        //    for (int i = 0; i < evaluators.Length; i++) {
        //        result[i] = evaluators[i].Compute(position)[0];
        //    }

        //    return result;
        //}
        //public Tensor[][] GetRandomScores(int count) {
        //    Tensor[][] result = new Tensor[evaluators.Length][];
        //    for (int j = 0; j < evaluators.Length; j++) {
        //        var t = new Tensor[count];
        //        for (int i = 0; i < count; i++) {
        //            t[i] = new Tensor(()=> UnityEngine.Random.value,1);
        //        }
        //        result[j] = t;
        //    }
        //    return result;
        //}

        //public float[] TrainEvaluators(Tensor[] states, Tensor[][] scores) {
        //    float[] result = new float[evaluators.Length];
        //    for (int i = 0; i < evaluators.Length; i++) {
        //        result[i] = evaluators[i].Train(states, scores[i])[0];
        //    }
        //    return result;
        //}
    }
}