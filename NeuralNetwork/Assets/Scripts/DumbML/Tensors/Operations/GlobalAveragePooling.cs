using System.Collections.Generic;


namespace DumbML {
    public class GlobalAveragePooling : Operation {
        public GlobalAveragePooling(Operation op) : base(new int[] { op.shape[2] }, op) {
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(shape);
            Tensor input = operands[0];
            int width = input.Shape[0];
            int height = input.Shape[1];
            int channels = input.Shape[2];

            for (int c = 0; c < channels; c++) {
                float sum = 0;
                int x_val = 0;

                for (int x = 0; x < width; x++) {
                    int ind = x_val + c;
                    for (int y = 0; y < height; y++) {
                        //sum += input[x, y, c];
                        //sum += input[x * channels * height + y * channels + c];
                        sum += input.value[ind];
                        ind += channels;
                    }
                    x_val += channels * height;
                }

                result.tensor[c] = sum / (width * height);
            }
        }

        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            Tensor input = inner[0].value;

            int width = input.Shape[0];
            int height = input.Shape[1];

            for (int c = 0; c < input.Shape[2]; c++) {
                float err = e[c] / (width * height);

                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        result[0][x, y, c] += err;
                    }
                }
            }



        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new GlobalAveragePooling(inner[0]._Copy(track));
        }
    }
}