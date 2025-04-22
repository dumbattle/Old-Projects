using System.Collections.Generic;

namespace DumbML {
    public class Flatten : Layer {
        
        public override void Build(Layer prevLayer) {
            inputShape = prevLayer.outputShape;
            int size = 1;
            for (int i = 0; i < inputShape.Length; i++) {
                size *= inputShape[i];
            }
            outputShape = new int[] { size };

            IsBuilt = true;
        }

        public override Tensor Compute(Tensor input) {
            IEnumerator<float> ie = input.GetEnumerator();

            System.Func<float> values = () => {
                ie.MoveNext();
                return ie.Current;
            };
            Tensor result = new Tensor(values, outputShape);

            output = result;
            return result;
        }

        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            Tensor input = context.input;

            IEnumerator<float> ie = error.GetEnumerator();

            System.Func<float> values = () => {
                ie.MoveNext();
                return ie.Current;
            };

            Tensor inputError = new Tensor(values, input.Shape);





            return (inputError, JaggedTensor.Empty);
        }
    }
}