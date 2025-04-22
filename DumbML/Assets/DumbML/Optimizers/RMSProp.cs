using System;

namespace DumbML {
    public class RMSProp : Optimizer {
        float rho = .9f;
        JaggedTensor[] squaredGradients;

        public RMSProp(NeuralNetwork network, float lr = .001f, LossFunction lossFunction = null, float rho = .9f) : base(network, lr) {
            base.lossFunction = lossFunction ?? new MSE();
            this.rho = rho;

            squaredGradients = new JaggedTensor[layers.Count];
        }

        public override (Tensor, JaggedTensor[]) Backwards(Tensor error, Layer.GD[] gd) {
            JaggedTensor[] layerGradients = new JaggedTensor[layers.Count];
            JaggedTensor gradient = null;

            for (int i = layers.Count - 1; i >= 0; i--) {
                (Tensor, JaggedTensor) bp = gd[i].Backward(error);
                error = bp.Item1;
                gradient = bp.Item2;

                layerGradients[i] = gradient;

                if (squaredGradients[i] == null) {
                    squaredGradients[i] = gradient.SameShape();
                }
                squaredGradients[i] = squaredGradients[i].PointWise(gradient, UpdateAvg);
                layerGradients[i] = squaredGradients[i].PointWise(gradient, ScaleLearningRate);


            }

            return (error, layerGradients);


            float UpdateAvg(float sg, float g) {
                return rho * sg + g.Sqr() * (1 - rho);
            }
            float ScaleLearningRate(float sg, float g) {
                return lr * g / ((float)Math.Sqrt(sg) + .00000001f);
            }
        }

    }

}