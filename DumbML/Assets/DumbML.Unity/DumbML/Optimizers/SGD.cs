namespace DumbML {
    public class SGD : Optimizer {
        public float momentum = 0;
        JaggedTensor[] m;

        public SGD(NeuralNetwork network, float lr = .01f, float momentum = .9f, LossFunction lossFunction = null) : base(network, lr) {
            base.lossFunction = lossFunction ?? new MSE();
            this.momentum = momentum.Clamp(0, 1);

            m = new JaggedTensor[layers.Count];
        }

        public override void Update(JaggedTensor[] layerGradients) {
            ClipNorm(layerGradients, 1);
            for (int i = 0; i < layers.Count; i++) {

                if (m[i] == null) {
                    m[i] = layerGradients[i];
                }
                else {
                    m[i] *= momentum;
                    m[i] += layerGradients[i] * (1 - momentum);
                }

                layers[i].Update(layerGradients[i] * lr);
            }
        }
    }
}