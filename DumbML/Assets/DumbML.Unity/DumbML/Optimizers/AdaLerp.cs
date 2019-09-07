using System;

namespace DumbML {
    public class AdaLerp : Optimizer {
        JaggedTensor[] m;
        JaggedTensor[] v;

        float bm;
        float bv;
        float lrFinal;
        float gamma;
        int step = 0;
        float t = 1;

        public AdaLerp(NeuralNetwork network, float lr = .001f, float finalLR = .1f, float gamma = .001f, LossFunction lossFunction = null, float betam = .9f, float betav = .999f) : base(network, lr) {
            bm = betam;
            bv = betav;

            m = new JaggedTensor[layers.Count];
            v = new JaggedTensor[layers.Count];
            this.lossFunction = lossFunction ?? new MSE();
            lrFinal = finalLR;
            this.gamma = gamma;
        }

        public override void Update(JaggedTensor[] layerGradients) {
            layerGradients = ClipNorm(layerGradients, 1);
            for (int i = 0; i < layers.Count; i++) {
                JaggedTensor gradient = layerGradients[i];

                if (m[i] == null) {
                    m[i] = gradient;
                    v[i] = gradient * gradient;
                }
                m[i] = m[i].PointWise(gradient, MomentumUpdate);
                v[i] = v[i].PointWise(gradient, VarianceUpdate);
                layerGradients[i] = m[i].PointWise(v[i], Update);

                layers[i].Update(m[i].PointWise(v[i], Update));
            }


            step++;
            float g = 1f / gamma;
            t = g / (step + g);



            float MomentumUpdate(float l, float r) {
                return l * bm + r * (1 - bm);
            }
            float VarianceUpdate(float l, float r) {
                return l * bv + r * r * (1 - bv);
            }
            float Update(float m, float v) {
                return m * (lr / ((float)Math.Sqrt(v) + 1e-10f)).Lerp(lrFinal, 1f -t);
            }
        }
    }

}