using System;
using FullSerializer;

namespace DumbML {
    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class Conv2DPointwise : Layer {
        [fsProperty]
        public Tensor Weights;
        [fsProperty]
        public ActivationFunction af;

        protected Func<float> weightInitializer;

        public Conv2DPointwise(int numOutputChannels, Func<float> weightInitializer = null, ActivationFunction af = null) {
            this.weightInitializer = weightInitializer ?? WeightInitializer.Default;
            this.af = af ?? ActivationFunction.None;

            outputShape = new int[] { numOutputChannels };
        }

        public override void Build (Layer prevLayer) {
            if (IsBuilt) {
                if (prevLayer.outputShape.TOSTRING() != inputShape.TOSTRING()) {
                    throw new RankException("Cannot connect layers. Input layer is wrong size");
                }
                else {
                    return;
                }
            }

            inputShape = (int[])prevLayer.outputShape.Clone();
            Weights = new Tensor(weightInitializer, inputShape[2], outputShape[0]);
            outputShape = new int[] { inputShape[0], inputShape[1], Weights.Shape[1] };
            IsBuilt = true;
        }

        public override Tensor Compute(Tensor input) {
            Tensor result = BLAS.Convolution2DPointwise(input, Weights);
            //result = result.PointWise((a) => af.Activate(a),true);
            output = result;
            return result;
        }
        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            //error *= context.output.PointWise((f) => af.Derivative(f));
            //error.PointWise(context.output, (e, o) => e * af.Derivative(o), true);
            (Tensor ie, Tensor grad) = BLAS.Convolution2DPointwiseBackwards(context.input, error, Weights);
            grad /= outputShape[0] * outputShape[1];
            return (ie, new JaggedTensor(grad));
        }
        public override void Update(JaggedTensor gradients) {
            //Weights.PointWise(gradients.Value, (w, g) => w - g, true);
            Weights.Subtract(gradients.Value, true);

        }
    }

}