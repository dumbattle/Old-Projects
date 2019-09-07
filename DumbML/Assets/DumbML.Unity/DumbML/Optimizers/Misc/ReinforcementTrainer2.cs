using System;
using System.Collections.Generic;
using System.Linq;

namespace DumbML {
    [Serializable]
    public class ReinforcementTrainer2 {
        public readonly int bufferSize = 100000;
        NeuralNetwork network;
        NeuralNetwork targetNetwork;

        public Optimizer optimizer;
        public int actionSize;

        //Buffer<(Tensor input, Tensor target)> replayBuffer;

        Buffer<(Tensor input, int action, float reward, Tensor nextState, float priority)> experienceRelay;

        Random rng = new Random();

        public float exploreProb = 1;
        public float exploreStop = .01f;
        public float exploreDecay = .000_01f;

        //public float curiosity = 1;
        //public float curiosityStop = .25f;
        //public float curiosityDecay = .000_01f;

        public float step = 0;

        public float discount = .9f;

        //public float explorationRate, curiosityRate, exploitationRate;

        public ActivationLayer smoothRelu = new ActivationLayer(ActivationFunction.SmoothRelu);

        public ReinforcementTrainer2(NeuralNetwork network, int actionSize) {
            this.network = network;
            UpdateTargetNetwork();
            this.actionSize = actionSize;
            optimizer = new RMSProp(network, lr: .0001f, lossFunction: new ReinforcementLoss());

            experienceRelay = new PrioritizedBuffer<(Tensor input, int action, float reward, Tensor nextState, float priority)>
                (bufferSize, (e) => e.priority);
            //experienceRelay = new Buffer<(Tensor input, int action, float reward, Tensor nextState, float priority)>(bufferSize);
            smoothRelu.Build(network);
        }


        public int Next(Tensor input) {
            if (exploreProb > exploreStop) {
                exploreProb *= 1 - exploreDecay;
            }

            step++;
            if (step% 1000 == 0) {
                UpdateTargetNetwork();
            }
            int action = -1;
            Tensor output = network.Compute(input);

            action = 0;

            //UnityEngine.Debug.Log(output);


            if (rng.NextDouble() < exploreProb) {
                    //Explore
                    action = rng.Next(0, actionSize);
            }
            else {
                //if (curiosity > curiosityStop) {
                //    curiosity *= (1 - curiosityDecay);
                //}

                //if (rng.NextDouble() < curiosity) {
                //    //curious
                //    output = smoothRelu.Compute(output);

                //    float sum = 0;

                //    foreach (float f in output) {
                //        sum += f;
                //    }

                //    float dart = (float)rng.NextDouble() * sum;

                //    for (int i = 0; i < actionSize; i++) {
                //        dart -= output[i];
                //        if (dart <= 0) {
                //            action = i;
                //            break;
                //        }
                //    }
                //}
                //else {
                    //exploit
                    for (int i = 0; i < actionSize; i++) {
                        if (output[i] > output[action]) {
                            action = i;
                        }
                    }
                //}
            }

            //explorationRate = exploreProb;
            //curiosityRate = (1 - exploreProb) * curiosity;
            //exploitationRate = (1 - exploreProb) * (1 - curiosity);

            return action;
        }

        public void Add(Tensor state, int action, float reward, Tensor nextState, int count = 1) {
            Tensor input = state;

            float dis = discount /** (1 - exploreProb)*/;

            //dis = 0;
            //float futureReward = nextState != null ? network.Compute(nextState).Max() : 0;


            //double DQN
            int nextAction = 0;
            if (nextState != null) {
                Tensor nextOuput = network.Compute(nextState);

                for (int j = 1; j < actionSize; j++) {
                    if (nextOuput[j] > nextOuput[nextAction]) {
                        nextAction = j;
                    }
                }
            }

            float futureReward = nextState != null ? targetNetwork.Compute(nextState)[nextAction] : 0;



            Tensor target = new Tensor(() => float.NaN, actionSize);
            target[action] = reward + futureReward * dis;
            Tensor output = network.Compute(state);

            float loss = Math.Abs(target[action] - output[action]) + .000001f;
            for (int i = 0; i < count; i++) {
                experienceRelay.Add((input, action, reward, nextState, loss));
                //optimizer.Train(input, target);
                //UnityEngine.Debug.Log(reward);
            }
        }

        public float Train(int count = 32,int batchSize = 32, int epochs = 1) {
            //(Tensor input, Tensor target)[] data = replayBuffer.GetRandom(BATCH_SIZE);

            (Tensor input, int action, float reward, Tensor nextState, float)[] data = experienceRelay.GetRandom(count);
            Tensor[] inputs = new Tensor[count];

            Tensor[] targets = new Tensor[count];

            float dis = discount /** (1 - exploreProb)*/;
            dis = 0;
            float e = 0;


            for (int i = 0; i < count; i++) {
                inputs[i] = data[i].input;
                if (inputs[i] == null) {
                    continue;
                }
                targets[i] = new Tensor(() => float.NaN, actionSize);



                //float futureReward = data[i].nextState != null ? network.Compute(data[i].nextState).Max() : 0;
                //targets[i][data[i].action] = data[i].reward + futureReward * dis;




                //double DQN
                if (data[i].nextState == null) {

                    targets[i][data[i].action] = data[i].reward;
                }
                else {
                    Tensor nextOuput = network.Compute(data[i].nextState);
                    int nextAction = 0;

                    for (int j = 1; j < actionSize; j++) {
                        if (nextOuput[j] > nextOuput[nextAction]) {
                            nextAction = j;
                        }
                    }

                    float futureReward = data[i].nextState != null ? targetNetwork.Compute(data[i].nextState)[nextAction] : 0;

                    targets[i][data[i].action] = data[i].reward + futureReward * dis;
                }
            }

            e = optimizer.Train(inputs, targets, batchSize, epochs);

            return e;
        }

        void UpdateTargetNetwork () {
            targetNetwork = (NeuralNetwork)NeuralNetwork.FromJson(network.ToJSON());
        }
    }

}