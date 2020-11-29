using DumbML;

namespace Flappy {
    public class FlappyPG : Model {
        public RingBuffer<RLExperience> trajectory = new RingBuffer<RLExperience>(1000);
        public RingBuffer<RLExperience> replayBuffer = new RingBuffer<RLExperience>(1000);

        float discount = .9f;

        Operation logProb;
        Placeholder actionMask;
        Optimizer o;
        Gradients g;

        public FlappyPG() {
            Operation op = new InputLayer(4).Build();
            op = new FullyConnected(30, ActivationFunction.Sigmoid).Build(op);
            op = new FullyConnected(30, ActivationFunction.Sigmoid).Build(op);
            op = new FullyConnected(2).Build(op);

            op = op.Softmax();

            actionMask = new Placeholder("Action Mask", op.shape);
            logProb = new Log(op) * actionMask * new BroadcastScalar(-1, actionMask.shape);
            Build(op);

            g = new Gradients(op.GetOperations<Variable>());
            o = new RMSProp(g);
        }

        public RLExperience SampleAction(Tensor state) {
            Tensor output = Compute(state);
            int action = output.Sample();
            RLExperience result = new RLExperience(state, output, action);
            return result;
        }

        public void EndTrajectory() {
            float score = 0;
            float avg = 0;

            replayBuffer.Clear();

            for (int i = trajectory.Count - 1; i >= 0; i--) {
                var exp = trajectory[i];

                score *= discount;
                score += exp.reward;
                exp.reward = score;

                avg += exp.reward;

                replayBuffer.Add(exp);
            }

            avg /= trajectory.Count;

            foreach (var exp in trajectory) {
                exp.reward -= avg;
            }

            trajectory.Clear();
        }

        public void Train(int batchSize = 32) {
            if (replayBuffer.Count < batchSize) {
                return;
            }

            var sample = replayBuffer.GetRandom(batchSize);

            var inputs = new Tensor[batchSize];
            var masks = new Tensor[batchSize];

            for (int i = 0; i < batchSize; i++) {
                inputs[i] = sample[i].state;

                var r = sample[i].reward;

                masks[i] = new Tensor(outputShape);
                masks[i][sample[i].action] = r;
            }

            for (int i = 0; i < batchSize; i++) {
                SetInputs(inputs[i]);

                actionMask.SetVal(masks[i]);

                logProb.Eval();
                logProb.Backwards(o);
            }

            o.Update();
            o.ZeroGrad();
        }

        public void TrainAll() {
            int batchSize = replayBuffer.Count;

            var inputs = new Tensor[batchSize];
            var masks = new Tensor[batchSize];

            for (int i = 0; i < batchSize; i++) {
                inputs[i] = replayBuffer[i].state;

                var r = replayBuffer[i].reward;

                masks[i] = new Tensor(outputShape);
                masks[i][replayBuffer[i].action] = r;
            }

            for (int i = 0; i < batchSize; i++) {
                SetInputs(inputs[i]);
                actionMask.SetVal(masks[i]);
                logProb.Eval();
                logProb.Backwards(o);
            }

            o.Update();
            o.ZeroGrad();
        }
    }
}