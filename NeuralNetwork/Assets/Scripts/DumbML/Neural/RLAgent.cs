using System;
using System.Collections.Generic;


namespace DumbML {
    public abstract class RLAgent : Model {
        public RingBuffer<RLExperience> experiences;
        public float exploration = 0.01f;
        public float discout = .9f;
        public int updateInterval = 1000;
        int updateTimer = 0;
        TrainableModel trainModel;
        Model futureModel;

        public RLAgent(int expBufferSize) {
            experiences = new RingBuffer<RLExperience>(expBufferSize);
        }

        public void Build(Operation op) {
            op.Optimize();
            inputs = op.GetOperations<Placeholder>().ToArray();
            forward = op;
            var shape = op.shape;
            if (shape.Length != 1) {
                throw new ArgumentException($"RLAgent must output a 1-D Tensor. Got a {shape.Length}-D Tensor");
            }

            var trainOp = forward;
            futureModel = Copy();
        }



        public RLExperience GetAction(Tensor input) {
            return UnityEngine.Random.value < exploration ? GetRandomAction(input) : GetBestAction(input);
        }

        public RLExperience GetBestAction(Tensor input) {
            var output = Compute(input);
            int result = 0;
            for (int i = 1; i < output.Size; i++) {
                if (output[i] > output[result]) {
                    result = i;
                }
            }

            return new RLExperience(input, output, result);
        }


        public RLExperience GetRandomAction(Tensor input) {
            SetInputs(input);
            var output = forward.Eval();
            return new RLExperience(input, output, UnityEngine.Random.Range(0, output.Size));
        }

        RLExperience GetFutureAction(Tensor input) {
            var output = futureModel.Compute(input);
            int result = 0;

            for (int i = 1; i < output.Size; i++) {
                if (output[i] > output[result]) {
                    result = i;
                }
            }

            return new RLExperience(input, output, result);
        }


        public void AddExperience(RLExperience exp) {
            experiences.Add(exp);
        }


        public void SetOptimizer(Optimizer o, Loss l) {
            var op = forward;

            //addMask
            op *= new Placeholder(op.shape);

            trainModel = new TrainableModel(op);
            trainModel.SetOptimizer(o, l);
        }

        public float Train(int batchSize, int numBatches) {
            if (experiences.Count < batchSize) {
                return -1;
            }
            float loss = 0;


            for (int i = 0; i < numBatches; i++) {
                var batch = experiences.GetRandom(batchSize, (e) => e.weight);
                Tensor[][] inputs = new Tensor[batchSize][];
                Tensor[] labels = new Tensor[batchSize];

                for (int j = 0; j < batchSize; j++) {
                    var e = batch[j];
                    if(e == null) {
                        continue;
                    }
                    Tensor target = new Tensor(e.output.Shape);
                    Tensor mask = new Tensor(e.output.Shape);

                    mask[e.action] = 1;


                    float reward = e.reward;
                    if (e.nextState != null) {
                        var n = GetFutureAction(e.nextState);
                        reward += n.output[n.action] * discout;
                        target[e.action] = reward;
                    }
                    e.weight = (e.output[e.action] - reward).Abs();


                    inputs[j] = new[] { e.state, mask };
                    labels[j] = target;
                }
                loss += trainModel.Train(inputs, labels, batchSize)[0];

                updateTimer++;
                if (updateTimer >= updateInterval) {
                    updateTimer = 0;
                    futureModel.SetWeights(GetWeights());
                }
            }

            return loss / numBatches;
        }
    }

    public class RLExperience {
        public Tensor state;
        public Tensor output;
        public int action;
        public float reward;
        public Tensor nextState;
        public float weight;

        public RLExperience(Tensor state, Tensor output, int action) {
            this.state = state;
            this.output = output;
            this.action = action;
            weight = 1;
            reward = 0;
            nextState = null;
        }
    }


    public static class RandomSelection {
        static Random rng = new Random();

        public static T[] GetRandom<T>(this IEnumerable<T> src, int count) {
            T[] result = new T[count];
            int i = -1;

            float w = (float)Math.Exp(Math.Log(rng.NextDouble()) / count);
            int next = count;

            foreach (var item in src) {
                i++;

                if (i < count) {
                    result[i] = item;
                }
                else {
                    if (next == i) {
                        result[rng.Next(0, count)] = item;
                        w *= (float)Math.Exp(Math.Log(rng.NextDouble()) / count);
                        next = next + (int)(Math.Log(rng.NextDouble()) / Math.Log(1 - w)) + 1;
                    }
                }
            }

            return result;
            //T[] result = new T[count];
            //int i = -1;

            //foreach (var item in src) {
            //    i++;

            //    if (i < count) {
            //        result[i] = item;
            //    }else {
            //        var v = rng.Next(0, i);
            //        if (v < count) {
            //            result[v] = item;
            //        }
            //    }
            //}

            //return result;
        }



        public static T[] GetRandom<T>(this IEnumerable<T> src, int count, Func<T, float> weight) {

            float sum = 0;
            foreach (var item in src) {
                sum += weight(item);
            }

            float[] darts = new float[count];
            for (int i = 0; i < count; i++) {
                darts[i] = (float)(rng.NextDouble() * sum);
            }
            Array.Sort(darts);

            T[] result = new T[count];

            int currentDart = 0;
            float running = 0;

            foreach (var item in src) {
                var w = weight(item);
                running += w;

                while (darts[currentDart] < running) {
                    result[currentDart] = item;
                    currentDart++;
                    if (currentDart == count) {
                        return result;
                    }


                }
            }

            //int i = -1;

            //float sum = 0;
            //float rSum = 0;

            //foreach (var item in src) {
            //    i++;
            //    sum += weight(item);

            //    if (i < count) {
            //        result[i] = item;
            //        rSum += weight(item);
            //    }
            //    else {
            //        var p = weight(item) / sum * count;
            //        var v = rng.NextDouble();

            //        if (v <= p) {
            //            result[rng.Next(0, count)] = item;
            //        }
            //    }
            //}

            return result;
        }
    }
}
