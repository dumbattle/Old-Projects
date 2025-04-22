using System;
using System.Threading.Tasks;
using System.Reflection;
namespace DumbML {
    public static class BLAS {

        public static Tensor MatrixMultiply(Tensor input, Tensor weights) {
            //if (input.Shape.Length > 2 || weights.Shape.Length > 2) {
            //    throw new InvalidOperationException("Tensors have to be 2D or 1D to perform matrix multiplication");
            //}

            int inputSize = input.Shape[0];

            int outputSize = weights.Shape[1];

            if (inputSize != weights.Shape[0]) {
                throw new InvalidOperationException(
                    "Width of left Tensor must match height of right Tensor " +
                    "in order to perform matrix multiplication" +
                    $"Left: {input.Shape.TOSTRING()}  Right: {weights.Shape.TOSTRING()}");
            }

            Tensor result = new Tensor(outputSize);

            Parallel.For(0, outputSize, (o) => {
                int wind = o;
                float v = 0;

                for (int i = 0; i < inputSize; i++) {
                    v += input._value[i] * weights._value[wind];
                    wind += outputSize;
                }
                result._value[o] = v;
            });

            return result;
        }

        public static (Tensor error, Tensor gradient) MatrixMultiplyBackwards(Tensor input, Tensor weights, Tensor error) {
            int inputSize = input.Shape[0];

            int outputSize = weights.Shape[1];

            if (inputSize != weights.Shape[0]) {
                throw new InvalidOperationException(
                    "Width of left Tensor must match height of right Tensor " +
                    "in order to perform matrix multiplication" +
                    $"Left: {input.Shape.TOSTRING()}  Right: {weights.Shape.TOSTRING()}");
            }

            (Tensor error, Tensor gradient) result = (input.SameShape(), weights.SameShape());

            Parallel.For(0, outputSize, (o) => {
                int wind = o;
                float e = error._value[o];
              
                for (int i = 0; i < inputSize; i++) {
                    result.gradient._value[wind] += e * input._value[i];
                    wind += outputSize;
                }
            });


            Parallel.For(0, inputSize, (i) => {
                int wind = i * outputSize;
                float v = 0;

                for (int o = 0; o < outputSize; o++) {
                    v += error._value[o] * weights._value[wind];
                    wind++;
                }
                result.error._value[i] = v;
            });
            return result;
        }



        public static Tensor Convolution2DDepthwise(Tensor input, Tensor filter, (int x, int y) stride = default, bool pad = true) {
            if (input.Rank != 3) {
                throw new InvalidOperationException("Tensors have to be 3D to perform 2D depthwise convolution");
            }
            if (filter.Rank != 3) {
                throw new InvalidOperationException("Tensors have to be 3D to perform 2D depthwise convolution");
            }
            if (filter.Shape[2] != input.Shape[2]) {
                throw new InvalidOperationException($"Input and filters need to have the same number of channels." +
                    $" Input: {input.Shape[2] } Filter: {filter.Shape[2] }");
            }
            int strideX = stride.x > 0 ? stride.x : 1;
            int strideY = stride.y > 0 ? stride.y : 1;

            //int width;
            //int height;
            int channels = filter.Shape[2];

            var (padX, padY) = pad ? (filter.Shape[0] - 1, filter.Shape[1] - 1) : (0, 0);

            int shape = (input.Shape[0] - filter.Shape[0] + padX) / strideX + 1;
            Tensor result =
                new Tensor(
                    shape,
                    (input.Shape[1] - filter.Shape[1] + padY) / strideY + 1,
                    input.Shape[2]);
            padX /= 2;
            padY /= 2;

            int findInt = channels * (input.Shape[1] - filter.Shape[1]);



            Parallel.For(0, channels, (c) => {
                int index = c;

                int iWidth = input.Shape[0];
                int iHeight = input.Shape[1];
                int rWidth = result.Shape[0];
                int rHeight = result.Shape[1];
                int fWidth = filter.Shape[0];
                int fHeight = filter.Shape[1];
                int i_xStart = c - padX * channels * iHeight;
                int i_xInterval =  strideX * channels * iHeight;
                int i_yStart = channels *  - padY;
                int i_yInterval = channels *  strideY;

                int i_x = i_xStart;
                for (int x = 0; x < rWidth; x++) {
                    //int i_x = (x * strideX) * channels * input.Shape[1] + i_xStart;
                    int i_y = i_yStart;
                    for (int y = 0; y < rHeight; y++) {
                        //int i_y = channels * (y * strideY - padY);

                        int fi = c;
                        int i = i_y + i_x;
                        int find = 0;
                        float v = 0;

                        int ix = x * strideX - padX;
                        for (int fx = 0; fx < fWidth; fx++) {
                            int iy = y * strideY - padY;
                            for (int fy = 0; fy < fHeight; fy++) {

                                //ix = x * strideX + fx - padX;
                                //iy = y * strideY + fy - padY;

                                if (ix >= 0 && ix < iWidth && iy >= 0 && iy < iHeight) {
                                    v += input._value[i + find] * filter._value[fi];
                                }


                                fi += channels;
                                find += channels;
                                iy++;
                            }
                            result._value[index] = v;
                            find += findInt;
                            ix++;
                        }
                        index += channels;
                        i_y += i_yInterval;
                    }
                    i_x += i_xInterval;
                }
            });

            return result;
        }

        public static (Tensor, Tensor) Convolution2DDepthwiseBackwards(Tensor input, Tensor error, Tensor filter, (int x, int y) stride = default, bool pad = true) {
            if (input.Rank != 3) {
                throw new InvalidOperationException("Tensors have to be 3D to perform 2D depthwise convolution");
            }
            if (filter.Rank != 3) {
                throw new InvalidOperationException("Tensors have to be 3D to perform 2D depthwise convolution");
            }
            if (filter.Shape[2] != input.Shape[2]) {
                throw new InvalidOperationException("Input and filters need to have the same number of channels");
            }

            int channels = filter.Shape[2];
            int strideX = stride.x > 0 ? stride.x : 1;
            int strideY = stride.y > 0 ? stride.y : 1;

            var (padX, padY) = pad ? (filter.Shape[0] - 1, filter.Shape[1] - 1) : (0, 0);

            var (width, height) = (
                    (input.Shape[0] - filter.Shape[0] + padX) / stride.x + 1,
                    (input.Shape[1] - filter.Shape[1] + padY) / stride.y + 1);
            padX /= 2;
            padY /= 2;


            (Tensor error, Tensor gradient) result = (input.SameShape(), filter.SameShape());
            Parallel.For(0, channels, (c) => {
                int index = c;

                int iWidth = input.Shape[0];
                int iHeight = input.Shape[1];
                int fWidth = filter.Shape[0];
                int fHeight = filter.Shape[1];

                int i_xStart = c - padX * channels * iHeight;
                int i_xInterval = strideX * channels * iHeight;
                int i_yStart = channels * -padY;
                int i_yInterval = channels * strideY;

                int i_x = i_xStart;
                for (int x = 0; x < width; x++) {
                    int i_y = i_yStart;
                    for (int y = 0; y < height; y++) {
                        int fi = c;
                        int i = i_y + i_x;
                        //int i = c + channels * (y * strideY - padY) + (x * strideY - padX) * channels * input.Shape[1];
                        int find = 0;
                        float e = error._value[index];
                        int ix = x * strideX - padX;
                        for (int fx = 0; fx < fWidth; fx++) {
                            int iy = y * strideY - padY;
                            for (int fy = 0; fy < fHeight; fy++) {

                                //int ix = x * stride.x + fx - padX;
                                //int iy = y * stride.y + fy - padY;

                                if (ix >= 0 && ix < iWidth && iy >= 0 && iy <iHeight) {
                                    result.error._value[i + find] +=e * filter._value[fi];
                                    result.gradient._value[fi] += e* input._value[i + find];
                                }


                                fi += channels;
                                find += channels;
                                fy++;
                            }
                            fx++;
                            find += channels * (iHeight - fHeight);
                        }
                        index += channels;
                        i_y += i_yInterval;
                    }
                    i_x += i_xInterval;
                }
            });

            return result;
        }



        public static Tensor Convolution2DPointwise(Tensor input, Tensor weights) {
            if (input.Rank != 3) {
                throw new ArgumentException("Input tensor must be 3D in order to perform 2D pointwise convolution. Got: " + input.Rank);
            }
            if (weights.Rank != 2) {
                throw new ArgumentException("weights", "2D pointwise convolution requires weights to be 2D. Got: " + weights.Rank);
            }
            if (input.Shape[2] != weights.Shape[0]) {
                throw new InvalidOperationException($"Number of Input channels ({input.Shape[2]}) " +
                    $"does not match size of weights: ({weights.Shape[0]})");
            }

            int width = input.Shape[0];
            int height = input.Shape[1];
            int outputChannels = weights.Shape[1];

            Tensor result = new Tensor(width, height, outputChannels);


            Parallel.For(0, outputChannels, (c) => {
                //for (int c = 0; c < outputChannels; c++) {
                int weightIndex = c;

                for (int ic = 0; ic < input.Shape[2]; ic++) {

                    int index = c;
                    int inputIndex = ic;
                    float wv = weights._value[weightIndex];
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {

                            result._value[index] += input._value[inputIndex] * wv;

                            index += outputChannels;
                            inputIndex += input.Shape[2];
                        }
                    }
                    weightIndex += outputChannels;
                }


            });

            return result;
        }

        public static (Tensor, Tensor) Convolution2DPointwiseBackwards(Tensor input, Tensor error, Tensor weights) {
            if (input.Rank != 3) {
                throw new ArgumentException("Input tensor must be 3D in order to perform 2D pointwise convolution. Got: " + input.Rank);
            }
            if (weights.Rank != 2) {
                throw new ArgumentException("weights", "2D pointwise convolution requires weights to be 2D. Got: " + weights.Rank);
            }
            if (input.Shape[2] != weights.Shape[0]) {
                throw new InvalidOperationException($"Number of Input channels ({input.Shape[2]}) " +
                    $"does not match size of weights: ({weights.Shape[0]})");
            }

            int width = input.Shape[0];
            int height = input.Shape[1];
            int outputChannels = weights.Shape[1];

            (Tensor error, Tensor gradient) result = (input.SameShape(), weights.SameShape());

            Parallel.For(0, outputChannels, (c) => {
                int weightIndex = c;

                for (int ic = 0; ic < input.Shape[2]; ic++) {

                    int index = c;
                    int inputIndex = ic;
                    float v = 0;
                    float w = weights._value[weightIndex];
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {

                            result.error._value[inputIndex] += error._value[index] * w;
                            v += error._value[index] * input._value[inputIndex];


                            index += outputChannels;
                            inputIndex += input.Shape[2];
                        }
                    }
                    result.gradient._value[weightIndex] = v;
                    weightIndex += outputChannels;
                }


            });

            return result;
        }



        public static Tensor Deconvolution2DDepthwise(Tensor input, Tensor filter, (int x, int y) stride, bool pad) {
            if (input.Rank != 3) {
                throw new InvalidOperationException("Tensors have to be 3D to perform 2D depthwise deconvolution");
            }
            if (filter.Rank != 3) {
                throw new InvalidOperationException("Tensors have to be 3D to perform 2D depthwise deconvolution");
            }
            if (filter.Shape[2] != input.Shape[2]) {
                throw new InvalidOperationException(
                    $"Input and filters need to have the same number of channels. " +
                    $"Input: {input.Shape.TOSTRING()} Filter: {filter.Shape.TOSTRING()}");
            }

            (int padX, int padY) = pad ? (filter.Shape[0] - 1, filter.Shape[1] - 1) : (0, 0);

            int width = stride.x * (input.Shape[0] - 1) + filter.Shape[0] - padX;
            int height = stride.y * (input.Shape[1] - 1) + filter.Shape[1] - padY;

            padX /= 2;
            padY /= 2;
            Tensor result = new Tensor(width, height, input.Shape[2]);



            //cache incriments
            int rindX = (result.Shape[1] * stride.x - input.Shape[1] * stride.y) * result.Shape[2];
            int rindY = result.Shape[2] * stride.y;
            int frindX = result.Shape[2] * (result.Shape[1] - filter.Shape[1]);
            int frindStart = -padY * result.Shape[2] - padX * result.Shape[1] * result.Shape[2];


            Parallel.For(0, input.Shape[2], (c) => {
                int iWidth = input.Shape[0];
                int iHeight = input.Shape[1];
                int fWidth = filter.Shape[0];
                int fHeight = filter.Shape[1];
                int channels = input.Shape[2];

                int i = c;
                int rind = c;

                for (int ix = 0; ix < iWidth; ix++) {
                    for (int iy = 0; iy < iHeight; iy++) {
                        int find = c;
                        int frind = frindStart;

                        int ox = -padX + ix * stride.x;

                        float iv = input._value[i];
                        for (int fx = 0; fx < fWidth; fx++) {
                            int oy = -padY + iy * stride.y;
                            for (int fy = 0; fy < fHeight; fy++) {

                                if (ox >= 0 && oy >= 0 && ox < width && oy < height) {
                                    result._value[rind + frind] += iv * filter._value[find];
                                }

                                find += channels;
                                frind += channels;
                                oy++;
                            }
                            frind += frindX;
                            ox++;
                        }
                        rind += rindY;
                        i += channels;
                    }
                    rind += rindX;
                }
            });



            return result;
        }

        public static (Tensor, Tensor) Deconvolution2DDepthwiseBackwards(Tensor input, Tensor error, Tensor filter, (int x, int y) stride, bool pad) {
            if (input.Rank != 3) {
                throw new InvalidOperationException("Tensors have to be 3D to perform 2D depthwise deconvolution");
            }
            if (filter.Rank != 3) {
                throw new InvalidOperationException("Tensors have to be 3D to perform 2D depthwise deconvolution");
            }
            if (filter.Shape[2] != input.Shape[2]) {
                throw new InvalidOperationException("Input and filters need to have the same number of channels");
            }

            (int padX, int padY) = pad ? (filter.Shape[0] - 1, filter.Shape[1] - 1) : (0, 0);

            int width = stride.x * (input.Shape[0] - 1) + filter.Shape[0] - padX;
            int height = stride.y * (input.Shape[1] - 1) + filter.Shape[1] - padY;

            padX /= 2;
            padY /= 2;
            (Tensor error, Tensor gradient) result = (input.SameShape(), filter.SameShape());





            //cache incriments
            int rindX = (error.Shape[1] * stride.x - input.Shape[1] * stride.y) * error.Shape[2];
            int rindY = error.Shape[2] * stride.y;

            int frindX = error.Shape[2] * (error.Shape[1] - filter.Shape[1]);
            int frindStart = -padY * error.Shape[2] - padX * error.Shape[1] * error.Shape[2];


            Parallel.For(0, input.Shape[2], (c) => {
                int iWidth = input.Shape[0];
                int iHeight = input.Shape[1];
                int fWidth = filter.Shape[0];
                int fHeight = filter.Shape[1];
                int channels = input.Shape[2];

                int i = c;
                int rind = c;

                for (int ix = 0; ix < iWidth; ix++) {
                    for (int iy = 0; iy < iHeight; iy++) {
                        int find = c;
                        int frind = frindStart;

                        int ox = -padX + ix * stride.x;

                        float v = 0;
                        float val = input._value[i];
                        for (int fx = 0; fx < fWidth; fx++) {

                            int oy = -padY + iy * stride.y;
                            for (int fy = 0; fy < fHeight; fy++) {

                                if (ox >= 0 && oy >= 0 && ox < width && oy < height) {
                                    v += error._value[rind + frind] * filter._value[find];
                                    result.gradient._value[find] += error._value[rind + frind] * val;
                                }

                                find += channels;
                                frind += channels;
                                oy++;
                            }
                            frind += frindX;
                            ox++;
                        }

                        result.error._value[i] = v;
                        rind += rindY;
                        i += channels;
                    }
                    rind += rindX;
                }
            });



            return result;
        }





        public static Tensor Add(Tensor l, Tensor r) {
            return l.PointWise(r, (a, b) => a + b);
        }
        public static Tensor Subtract(Tensor l, Tensor r) {
            return l.PointWise(r, (a, b) => a - b);
        }
        public static Tensor Multiply(Tensor l, Tensor r) {
            return l.PointWise(r, (a, b) => a * b);
        }
        public static Tensor Divide(Tensor l, Tensor r) {
            return l.PointWise(r, (a, b) => a / b);
        }

        public static Tensor Add(Tensor t, float f) {
            return t.PointWise((a) => a + f);
        }
        public static Tensor Subtract(Tensor t, float f) {
            return t.PointWise((a) => a - f);
        }
        public static Tensor Multiply(Tensor t, float f) {
            return t.PointWise((a) => a * f);
        }
        public static Tensor Divide(Tensor t, float f) {
            return t.PointWise((a) => a / f);
        }
    }



}