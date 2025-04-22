namespace DumbML {
    public class UpSample : Layer {
        (int x, int y) scale;

        public UpSample() { scale = (2, 2); }
        public UpSample(int scale) { this.scale = (scale, scale); }
        public UpSample((int x, int y) scale) { this.scale = scale; }

        public override void Build(Layer prevLayer) {
            inputShape = (int[])prevLayer.outputShape.Clone();
            outputShape = new int[] { inputShape[0] * scale.x, inputShape[1] * scale.y, inputShape[2] };

            IsBuilt = true;
        }


        public override Tensor Compute(Tensor input) {
            Tensor result = new Tensor(outputShape);

            for (int c = 0; c < input.Shape[2]; c++) {
                for (int x = 0; x < input.Shape[0]; x++) {
                    for (int y = 0; y < input.Shape[1]; y++) {

                        for (int sx = 0; sx < scale.x; sx++) {
                            for (int sy = 0; sy < scale.y; sy++) {
                                result[x * scale.x + sx, y * scale.y + sy, c] = input[x, y, c];
                            }
                        }

                    }
                }

            }
            output = result;

            return result;
        }

        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            Tensor input = context.input;
            Tensor inputError = input.SameShape();

            for (int c = 0; c < input.Shape[2]; c++) {
                for (int x = 0; x < input.Shape[0]; x++) {
                    for (int y = 0; y < input.Shape[1]; y++) {
                        float e = 0;
                        for (int sx = 0; sx < scale.x; sx++) {
                            for (int sy = 0; sy < scale.y; sy++) {
                                e += error[x* scale.x + sx, y* scale.y + sy, c];
                            }
                        }
                        inputError[x, y, c] += e;
                    }
                }
            }

            return (inputError, JaggedTensor.Empty);
        }
    }
}