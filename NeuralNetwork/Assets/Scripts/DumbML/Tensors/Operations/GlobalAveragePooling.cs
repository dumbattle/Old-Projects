using System.Collections.Generic;


namespace DumbML {
    public class GlobalAveragePooling : Operation {
        Tensor[] error;
        public GlobalAveragePooling(Operation op) : base(new int[] { op.shape[2] }, op) {
            error = new Tensor[1];
            error[0] = new Tensor(op.shape);
        }

        protected override Tensor _Compute(Tensor[] operands) {
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

                value[c] = sum / (width * height);
            }
            return value;
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            Tensor input = inner[0].value;
            Tensor result = error[0];

            int width = input.Shape[0];
            int height = input.Shape[1];

            for (int c = 0; c < input.Shape[2]; c++) {
                float err = e[c] / (width * height);

                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        result[x, y, c] = err;
                    }
                }
            }




            //Tensor input = inner[0].value;
            //int width = input.Shape[0];
            //int height = input.Shape[1];
            //int channels = input.Shape[2];

            //for (int c = 0; c < channels; c++) {
            //    float err = e[c] / (width * height);
            //    float sum = 0;
            //    int x_val = 0;

            //    for (int x = 0; x < width; x++) {
            //        int ind = x_val + c;
            //        for (int y = 0; y < height; y++) {
            //            //error[0][x, y, c] = err;
            //            error[0]._value[ind] = err;
            //            ind += channels;
            //        }
            //        x += channels * height;
            //    }

            //}




            return error;
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new GlobalAveragePooling(inner[0]._Copy(track));
        }
    }
}