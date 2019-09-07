using System;
using System.Linq;
using System.Threading.Tasks;
using FullSerializer;

namespace DumbML {

    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class FullyConnected : Layer {
        [fsProperty]
        public ActivationFunction af;
        [fsProperty]
        public Tensor weights;
        protected Func<float> weightInitializer;

        public FullyConnected(int size, Func<float> weightInitializer = null, ActivationFunction af = null) {
            this.weightInitializer = weightInitializer ?? new WeightInitializer.Uniform(-1, 1);
            outputShape = new int[] { size };
            this.af = af ?? ActivationFunction.None;
        }

        public override Tensor Compute(Tensor input) {
            if (!input.CheckShape(inputShape)) {
                throw new ArgumentException($"Input is not correct shape Expected: {inputShape.TOSTRING()} Got {input.Shape.TOSTRING()}");
            }

            input = input.Append(new Tensor(() => 1, 1));

            Tensor result = BLAS.MatrixMultiply(input, weights);
            result = result.PointWise((f) => af.Activate(f),true);


            output = result;
            return result;
        }

        public override void Build(Layer prevLayer) {
            if (prevLayer.outputShape.Length != 1) {
                throw new RankException("Cannot connect layers. Wrong tensor dimensions");
            }

            if (IsBuilt) {
                if (prevLayer.outputShape[0] != inputShape[0]) {
                    throw new RankException($"Cannot connect layers. Input layer is wrong size.  expected: {inputShape.TOSTRING() }  got: {prevLayer.outputShape.TOSTRING() }");
                }
                else {
                    return;
                }
            }
            inputShape = new int[] { prevLayer.outputShape[0] };

            weights = new Tensor(weightInitializer, inputShape[0] + 1, outputShape[0]);
            IsBuilt = true;
        }

        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            Tensor input = context.input.Append(new Tensor(() => 1, 1)).Reshape(inputShape[0] + 1, 1);
            error.PointWise(context.output, (e, o) => e * af.Derivative(o), true);

            var (inputError, gradient) = BLAS.MatrixMultiplyBackwards(input, weights, error);
            inputError = inputError.Shorten(1).Reshape(inputShape);

            return (inputError, new JaggedTensor(gradient));
        }

        public override void Update(JaggedTensor gradients) {
            if (!Trainable) {
                return;
            }

            weights.PointWise(gradients.Value, (w, g) => w - g, true);
        }
    }
}