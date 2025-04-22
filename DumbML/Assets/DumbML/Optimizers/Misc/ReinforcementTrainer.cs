using System.Collections.Generic;
using System.Collections;

namespace DumbML {
    public class ReinforcementTrainer {
        NeuralNetwork network;
        public Optimizer optimizer;
        public int actionSize;
        List<State> memory = new List<State>();

        public ReinforcementTrainer(NeuralNetwork network, int actionSize) {
            this.network = network;
            this.actionSize = actionSize;
            optimizer = new RMSProp(network, lossFunction: new ReinforcementLoss());
        }

        public void AddMemory(Tensor input, int action, float reward, float maxReward) {
            memory.Add(new State(input, action, reward, maxReward));
        }

        public float Train(int epochs = 1) {
            Tensor[] inputs = new Tensor[memory.Count];
            Tensor[] targets = new Tensor[memory.Count];

            for (int i = 0; i < memory.Count; i++) {
                State state = memory[i];

                inputs[i] = state.input;
                targets[i] = new Tensor(() => float.NaN, actionSize);

                //targets[i] = network.Compute(inputs[i]);
                float qVal = i == memory.Count - 1 ? state.reward : state.reward /*+ .5f * current.Value.maxReward*/;
                targets[i][state.action] = qVal;
                //optimizer.Train(inputs[i], targets[i]);
            }

            memory = new List<State>();
            return optimizer.TrainBatch(inputs, targets);
        }
        
    }

}