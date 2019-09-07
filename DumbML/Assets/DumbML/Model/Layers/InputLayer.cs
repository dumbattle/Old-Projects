using System;
using FullSerializer;

namespace DumbML {
    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class InputLayer : Layer {

        public InputLayer(int[] shape) {
            inputShape = outputShape = shape;
        }

        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            return(error, JaggedTensor.Empty);
        }

        public override void Build(Layer prevLayer = null) {
            IsBuilt = true;
        }

        public override Tensor Compute(Tensor input) {
            return input;
        }


        public override void Update(JaggedTensor gradients) {
            throw new NotImplementedException();
        }
    }
}