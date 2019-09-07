using System;

namespace DumbML {
    public class MaxPool : Layer {
        int width, height;

        public MaxPool() {
            width = 2;
            height = 2;
        }
        public MaxPool(int x, int y) {
            width = x;
            height = y;
        }

        public override void Build(Layer prevLayer) {
            if (prevLayer.outputShape.Length != 3) {
                throw new RankException("Input to MaxPool must be 3D");
            }

            inputShape = prevLayer.outputShape;
            outputShape = new int[] { (inputShape[0] + width - 1) / width, (inputShape[1] + height - 1) / height, inputShape[2] };
            IsBuilt = true;
        }

        public override Tensor Compute(Tensor input) {
            Tensor result = new Tensor(outputShape);

            for (int c = 0; c < outputShape[2]; c++) {
                for (int x = 0; x < outputShape[0]; x++) {
                    for (int y = 0; y < outputShape[1]; y++) {

                        (int x, int y) max = (0, 0);

                        for (int wx = 0; wx < width; wx++) {
                            for (int wy = 0; wy < height; wy++) {
                                int newX = width * x + wx;
                                int newY = height * y + wy;

                                int ix = width * x + max.x;
                                int iy = height * y + max.y;

                                if (newX < input.Shape[0] &&
                                    newY < input.Shape[1] &&
                                    input[newX, newY, c] > input[ix, iy, c]) {
                                    max = (wx, wy);
                                }

                            }
                        }
                        result[x, y, c] = input[width * x + max.x, height * y + max.y, c];
                    }
                }

            }
            output = result;
            return result;
        }

        public override Tensor Forward(Tensor input, ref Context context) {
            Tensor result = new Tensor(outputShape);
            (int, int, int)[,,] indices = new (int, int, int)[outputShape[0], outputShape[1], outputShape[2]];


            for (int c = 0; c < outputShape[2]; c++) {
                for (int x = 0; x < outputShape[0]; x++) {
                    for (int y = 0; y < outputShape[1]; y++) {

                        (int x, int y) max = (0, 0);

                        for (int wx = 0; wx < width; wx++) {
                            for (int wy = 0; wy < height; wy++) {
                                int newX = width * x + wx;
                                int newY = height * y + wy;
                                if (input[newX, newY, c] > input[width * x + max.x, height * y + max.y, c]) {
                                    max = (wx, wy);
                                }
                            }
                        }

                        result[x, y, c] = input[width * x + max.x, height * y + max.y, c];
                        indices[x, y, c] = (width * x + max.x, height * y + max.y, c);
                    }
                }

            }

            output = result;
            context = new Context {
                input = input,
                output = result
            };
            context.SaveData("indices", indices);
            return result;
        }

        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            Tensor input = context.input;
            Tensor output = context.output;
            (int, int, int)[,,] indices = context.GetData<(int, int, int)[,,]>("indices");

            Tensor inputError = input.SameShape();


            for (int x = 0; x < outputShape[0]; x++) {
                for (int y = 0; y < outputShape[1]; y++) {
                    for (int c = 0; c < outputShape[2]; c++) {
                        int ix, iy, ic;

                        (ix, iy, ic) = indices[x, y, c];
                        inputError[ix, iy, ic] = error[x, y, c];
                    }
                }
            }

            return (inputError, JaggedTensor.Empty);
        }
    }
    public class AveragePool : Layer {
        int width, height;

        public AveragePool() {
            width = 2;
            height = 2;
        }
        public AveragePool(int x, int y) {
            width = x;
            height = y;
        }

        public override void Build(Layer prevLayer) {
            if (prevLayer.outputShape.Length != 3) {
                throw new RankException("Input to Average Pool must be 3D");
            }

            inputShape = prevLayer.outputShape;
            outputShape = new int[] { (inputShape[0] + width - 1) / width, (inputShape[1] + height - 1) / height, inputShape[2] };
            IsBuilt = true;
        }

        public override Tensor Compute(Tensor input) {
            Tensor result = new Tensor(outputShape);

            for (int c = 0; c < outputShape[2]; c++) {
                for (int x = 0; x < outputShape[0]; x++) {
                    for (int y = 0; y < outputShape[1]; y++) {
                        float val = 0;
                        for (int wx = 0; wx < width; wx++) {
                            for (int wy = 0; wy < height; wy++) {
                                val += input[x * width + wx, y * height + wy, c];

                            }
                        }
                        result[x, y, c] = val /(width * height);
                    }
                }

            }
            output = result;
            return result;
        }

        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            Tensor input = context.input;
            Tensor output = context.output;

            Tensor inputError = input.SameShape();


            for (int x = 0; x < outputShape[0]; x++) {
                for (int y = 0; y < outputShape[1]; y++) {
                    for (int c = 0; c < outputShape[2]; c++) {

                        float e = error[x, y, c];
                        for (int wx = 0; wx < width; wx++) {
                            for (int wy = 0; wy < height; wy++) {
                                inputError[x * width + wx, y * height + wy, c] = e / (width * height);

                            }
                        }



                    }
                }
            }

            return (inputError, JaggedTensor.Empty);
        }
    }
}