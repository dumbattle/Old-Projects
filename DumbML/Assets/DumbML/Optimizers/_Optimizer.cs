using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace DumbML {

    public abstract class Optimizer {
        static Random rng = new Random();
        public float lr = .05f;

        public NeuralNetwork network;
        protected List<Layer> layers;
        protected LossFunction lossFunction;

        public Optimizer(NeuralNetwork network, float lr) {
            this.network = network;
            layers = network.Layers;
            this.lr = lr;
        }


        public float Train ((Tensor, Tensor) data) {
            return Train(data.Item1, data.Item2);
        }
        public virtual float Train(Tensor input, Tensor target) {
            float loss;
            JaggedTensor[] gradients;

            Session sess = NewSession();

            (loss, gradients) = sess.Run(input, target);

            Update(gradients);
            return loss;
        }
        public virtual float Train(Tensor[] input, Tensor[] target, int batchSize = 8, int epochs = 1) {
            Tensor[] batchInput = new Tensor[batchSize];
            Tensor[] batchTarget = new Tensor[batchSize];
            float loss = 0;
            int count = 0;
            int numBatches = 0;


            for (int e = 0; e < epochs; e++) {
                for (int i = 0; i < input.Length; i++) {
                    batchInput[count] = input[i];
                    batchTarget[count] = target[i];

                    count++;

                    if (count == batchSize) {
                        numBatches++;
                        loss += TrainBatch(batchInput, batchTarget);
                        count = 0;
                        batchInput = new Tensor[batchSize];
                        batchTarget = new Tensor[batchSize];
                    }
                }
            }
            return loss / numBatches / epochs;
        }
        public float Train((Tensor, Tensor)[] data, int batchsize = 32, int epochs = 1, bool shuffle = true) {
            if (shuffle) {
                data = (from x in data orderby rng.NextDouble() select x ).ToArray();
            }
            Tensor[] input = (from x in data select x.Item1).ToArray();
            Tensor[] target = (from x in data select x.Item2).ToArray();

            return Train(input, target, batchsize, epochs);
        }


        public virtual float TrainBatch(Tensor[] input, Tensor[] target) {
            JaggedTensor[] layerGradients = null;
            float loss = 0;

            int batchSize = input.Length;

            object _lock = new object();

            Parallel.For(0, batchSize, (i) => {
                if (input[i] == null) {
                    return;
                }
                Session sess = NewSession();
                var s = sess.Run(input[i], target[i]);

                Interlocked.Exchange(ref loss, loss + s.Item1);

                lock (_lock) {
                    if (layerGradients == null) {
                        layerGradients = s.Item2;
                    }
                    else {
                        Parallel.For(0, layerGradients.Length, (j) => {
                            //for (int j = 0; j < layerGradients.Length; j++) {
                            //layerGradients[j].PointWise(s.Item2[j], (l, r) => l + r, true);
                            layerGradients[j].Add(s.Item2[j], true);
                        });
                    }
                }
            });


            if (layerGradients == null) {
                return -1;
            }


            for (int i = 0; i < layerGradients.Length; i++) {
                //layerGradients[i].PointWise((l) => l / batchSize, true);
                layerGradients[i].Divide(batchSize, true);
            }
            Update(layerGradients);

            return loss / batchSize;
        }


        private Tensor Forward(Tensor input, Layer.GD[] gd) {
            var output = input;
            for (int i = 0; i < layers.Count; i++) {
                output = gd[i].Forward(output);
            }

            return output;
        }
        public virtual (float, Tensor) ComputeLoss(Tensor target, Tensor output) {
            return lossFunction.Compute(output, target);
        }
        public virtual (Tensor, JaggedTensor[]) Backwards(Tensor error, Layer.GD[] gd) {
            JaggedTensor[] layerGradients = new JaggedTensor[layers.Count];

            for (int i = layers.Count - 1; i >= 0; i--) {
                (Tensor, JaggedTensor) bp = gd[i].Backward(error);
                error = bp.Item1;

                layerGradients[i] = bp.Item2;
            }
            return (error, layerGradients);
        }

        public virtual void Update(JaggedTensor[] layerGradients) {
            layerGradients = ClipNorm(layerGradients, 1);
            for (int i = 0; i < layers.Count; i++) {
                layers[i].Update(layerGradients[i].PointWise((a) => a * lr, true));
            }
        }
        protected JaggedTensor[] ClipNorm(JaggedTensor[] grad, float maxNorm) {
            float sum = 0;

            foreach (var g in grad) {
                g.PointWise((a) => { sum += a * a; return a; });
            }

            sum = (sum + 1e-5f).Sqrt();
            if (sum > maxNorm) {
                for (int i = 0; i < grad.Length; i++) {

                    grad[i] /= (sum / maxNorm);
                }
            }
            //sum = 0;

            //foreach (var g in grad) {
            //    g.PointWise((a) => { sum += a * a; return a; });
            //}
            //sum = (sum + 1e-5f).Sqrt();
            //UnityEngine.Debug.Log(sum);
            return grad;
        }






        public Session NewSession() {
            Layer.GD[] gd = new Layer.GD[layers.Count];

            for (int i = 0; i < layers.Count; i++) {
                gd[i] = layers[i].GradientDescent();
            }

            return new Session(Forward, ComputeLoss, Backwards, gd);
        }


        public class Session {
            public delegate Tensor ForwardFunction(Tensor input, Layer.GD[] gd);
            public delegate (float, Tensor) LossFunction(Tensor target, Tensor output);
            public delegate (Tensor, JaggedTensor[]) BackwardFunction(Tensor error, Layer.GD[] gd);

            public (float loss, JaggedTensor[] grad) output;

            Func<Tensor, Tensor> fp;
            Func<Tensor, (float, Tensor)> lf;
            Func<Tensor, (Tensor, JaggedTensor[])> bp;

            ForwardFunction forward;
            LossFunction lossFunction;
            BackwardFunction backwardsPass;

            Layer.GD[] gd;

            public Session(ForwardFunction forward, LossFunction lossFunction, BackwardFunction backwardsPass, Layer.GD[] gd) {
                this.forward = forward;
                this.lossFunction = lossFunction;
                this.backwardsPass = backwardsPass;
                this.gd = gd;
                Initialize();
            }

            public Tensor Forward(Tensor input) {
                return fp(input);
            }
            public (float, Tensor) ComputeLoss(Tensor target) {
                return lf(target);
            }
            public (Tensor, JaggedTensor[]) Backward(Tensor error) {
                return bp(error);
            }


            public (float, JaggedTensor[]) Run(Tensor input, Tensor target) {
                Forward(input);
                (float, Tensor) e = ComputeLoss(target);
                JaggedTensor[] gradients = Backward(e.Item2).Item2;

                return output = (e.Item1, gradients);
            }

            protected virtual void Initialize() {
                Tensor output = null;

                fp =
                    (i) => {
                        output = forward(i, gd);
                        return output;
                    };
                lf =
                    (target) => {
                        return lossFunction(target, output);
                    };
                bp =
                    (e) => {
                        return backwardsPass(e, gd);
                    };
            }
        }
    }

}