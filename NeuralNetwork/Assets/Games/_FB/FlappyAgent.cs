using DumbML;

namespace Flappy {
    public class FlappyAgent : RLAgent {
        public Operation a, v;

        public FlappyAgent(int expBufferSize = 10000) : base(expBufferSize) {
            Operation op = new InputLayer(4).Build();
            op = new FullyConnected(10, ActivationFunction.Relu).Build(op);
            op = Attention(op);
            op = new FullyConnected(10, ActivationFunction.Relu).Build(op);
            op = Attention(op);

            a = new FullyConnected(10, ActivationFunction.Relu).Build(op);
            a = Attention(a);
            a = new FullyConnected(2,null).Build(op);

            v = new FullyConnected(10, ActivationFunction.Relu).Build(op);
            v = Attention(v);
            v = new FullyConnected(1,null).Build(op);

            op = v - new Sum(a) / 2;
            op = new Append(op, op);

            op += a;
            Build(op);
            SetOptimizer(new Adam(), Loss.MSE);            
        }

        Operation GetInner(Operation input) {
            var op = new FullyConnected(10, ActivationFunction.Sigmoid).Build(input);
            op = new FullyConnected(10, ActivationFunction.Sigmoid).Build(op);

            return op;
        }

        Operation Attention(Operation input) {
            var atn = new FullyConnected(input.shape[0], ActivationFunction.Sigmoid, false).Build(input);

            return input * atn;
        }
    }

}