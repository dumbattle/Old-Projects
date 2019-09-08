using System;

namespace DumbML {
    public class Adam : Optimizer {
        JaggedTensor[] m;
        JaggedTensor[] v;

        float bm;
        float bv;


        public Adam(NeuralNetwork network, float lr = .001f, LossFunction lossFunction = null, float betam = .9f, float betav = .99f) : base(network, lr) {
            bm = betam;
            bv = betav;

            m = new JaggedTensor[layers.Count];
            v = new JaggedTensor[layers.Count];
            this.lossFunction = lossFunction ?? new MSE();
        }

        public override void Update(JaggedTensor[] layerGradients) {
            for (int i = 0; i < layers.Count; i++) {
                if (!layers[i].Trainable) {
                    continue;
                }
                JaggedTensor gradient = layerGradients[i];

                if (m[i] == null) {
                    m[i] = gradient;
                    v[i] = gradient * gradient;
                }
                else {
                    m[i] = m[i].PointWise(gradient, MomentumUpdate, true);
                    v[i] = v[i].PointWise(gradient, VarianceUpdate, true);
                }
                layers[i].Update(m[i].PointWise(v[i], Update));
            }



            float MomentumUpdate(float l, float r) {
                return l * bm + r * (1 - bm);
            }
            float VarianceUpdate(float l, float r) {
                return l * bv + r * r * (1 - bv);
            }
            float Update(float m, float v) {
                return m * lr / ((float)Math.Sqrt(v) + 1e-10f);
            }
        }
    }

}