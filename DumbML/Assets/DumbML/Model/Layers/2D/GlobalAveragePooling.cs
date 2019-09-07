using System;

namespace DumbML {

    public class GlobalAveragePooling : Layer {
        
        public override void Build(Layer prevLayer) {

            if (prevLayer.outputShape.Length != 3) {
                throw new RankException("Input to GlobalAveragePooling2D must be 3D");
            }
            inputShape = new int[] { 0, 0, prevLayer.outputShape[2] };
            outputShape = new int[] { prevLayer.outputShape[2] };

        }

        public override Tensor Compute(Tensor input) {
            Tensor result = new Tensor(outputShape);

            for (int c = 0; c < outputShape[0]; c++) {
                float sum = 0;

                for (int x = 0; x < input.Shape[0]; x++) {
                    for (int y = 0; y < input.Shape[1]; y++) {
                        sum += input[x, y, c];
                    }
                }

                result[c] = sum / (input.Shape[0] * input.Shape[1]);
            }

            output = result;
            return result;
        }



        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            Tensor input = context.input;
            Tensor inputError = input.SameShape();

            for (int c = 0; c < outputShape[0]; c++) {
                float e = error[c] / (input.Shape[0] * input.Shape[1]);
                for (int x = 0; x < input.Shape[0]; x++) {
                    for (int y = 0; y < input.Shape[1]; y++) {
                        inputError[x, y, c] = e;
                    }
                }
            }
            return (inputError, JaggedTensor.Empty);

        }
    }
}