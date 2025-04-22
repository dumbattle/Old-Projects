using FullSerializer;

namespace DumbML {
    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class Bias : Layer {
        [fsProperty]
        public Tensor weights;

        public Bias() { }

        public override void Build(Layer prevLayer) {
            inputShape = (int[])prevLayer.outputShape.Clone();
            outputShape = inputShape;
            weights = new Tensor(inputShape);
        }

        public override Tensor Compute(Tensor input) {
            Tensor result = input + weights;
            output = result;
            return result;
        }

        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            return (error, new JaggedTensor(error));
        }
        public override void Update(JaggedTensor gradients) {
            //weights.PointWise(gradients.Value, (w, g) => w - g, true);
            weights.Subtract(gradients.Value, true);

        }
    }

    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class BatchNorm : Layer {
        public Tensor means;
        public Tensor stdDev;

        public BatchNorm() { }


        public override void Build(Layer prevLayer) {
            inputShape = outputShape = (int[])prevLayer.outputShape.Clone();

            means = new Tensor(outputShape);
            stdDev = new Tensor(() => 1, outputShape);
        }

        public override Tensor Compute(Tensor input) {
            output = input - means;
            output = output.PointWise(stdDev, (m, sd) => m / (sd + 1e-10f));
            return output.Copy();
        }

        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            Tensor inputError = error.Copy() / stdDev;
            Tensor mGrad = means - context.input ;
            Tensor sdGrad = stdDev - mGrad.PointWise((a) => a.Abs());


            return (inputError, new JaggedTensor(mGrad, sdGrad));
        }

        public override void Update(JaggedTensor gradients) {
            means -= gradients.inner[0].Value;
            stdDev -= gradients.inner[1].Value;
        }
    }
}