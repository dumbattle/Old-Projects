using System;
using System.Collections.Generic;
using FullSerializer;

namespace DumbML {
    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class ActivationLayer : Layer {
        [fsProperty]
        public ActivationFunction af;

        public ActivationLayer(ActivationFunction af) {
            this.af = af ?? ActivationFunction.None;
        }

        public override void Build(Layer prevLayer) {
            outputShape = inputShape = (int[])prevLayer.outputShape.Clone();
            IsBuilt = true;
        }

        public override Tensor Compute(Tensor input) {
            IEnumerator<float> ie = input.GetEnumerator();

            Func<float> function =
                () => {
                    ie.MoveNext();
                    return af.Activate(ie.Current);
                };
            Tensor result = new Tensor(function, input.Shape);
            output = result;
            return result;
        }


        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            Tensor inputError = error.PointWise(context.output, (e, o) => e * af.Derivative(o),true);


            return (inputError, JaggedTensor.Empty);

        }
        public override void Update(JaggedTensor gradients) { }
    }
}