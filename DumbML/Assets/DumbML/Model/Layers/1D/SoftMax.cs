using System;
using System.Collections.Generic;

namespace DumbML {
    public class SoftMax : Layer {
        public override void Build(Layer prevLayer) {
            outputShape = inputShape = prevLayer.outputShape;
            IsBuilt = true;
        }

        public override Tensor Compute(Tensor input) {
            IEnumerator<float> ie = input.GetEnumerator();

            Func<float> exp = () => { ie.MoveNext(); return (float)Math.Exp(ie.Current); };

            Tensor result = new Tensor(exp, input.Shape);
            float sum = 0;
            foreach (float f in result) {
                sum += f;
            }

            this.output = result / sum;
            return result / sum;
        }

        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            Tensor inputError = context.input.SameShape();
            Tensor output = context.output;

            for (int j = 0; j < outputShape[0]; j++) {
                float e = error[j];

                for (int k = 0; k < inputShape[0]; k++) {
                    float derivative = j != k ? -output[j] * output[k] : output[k] * (1 - output[j]);

                    inputError[k] += e * derivative;
                }
            }
            return (inputError, JaggedTensor.Empty);
        }
        public override void Update(JaggedTensor gradients) { }
    }
}