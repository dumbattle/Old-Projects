using System;
using System.Threading;

namespace DumbML {
    public static partial class Blas {
        public static partial class Parallel {

            public static Tensor Convolution2DPointwise(Tensor input, Tensor weights, Tensor dest) {
                if (input.Rank != 3) {
                    throw new ArgumentException("Input tensor must be 3D in order to perform 2D pointwise convolution. Got: " + input.Rank);
                }
                if (weights.Rank != 2) {
                    throw new ArgumentException("2D pointwise convolution requires weights to be 2D. Got: " + weights.Rank, "weights");
                }
                if (input.Shape[2] != weights.Shape[0]) {
                    throw new InvalidOperationException($"Number of Input channels ({input.Shape[2]}) " +
                        $"does not match size of weights: ({weights.Shape[0]})");
                }
                dest.PointWise((a) => 0, true);
                int width = input.Shape[0];
                int height = input.Shape[1];
                int outputChannels = weights.Shape[1];

                //Tensor result = new Tensor(width, height, outputChannels);


                System.Threading.Tasks.Parallel.For(0, outputChannels, (c) => {
                    int weightIndex = c;

                    for (int ic = 0; ic < input.Shape[2]; ic++) {

                        int index = c;
                        int inputIndex = ic;
                        float wv = weights._value[weightIndex];
                        for (int x = 0; x < width; x++) {
                            for (int y = 0; y < height; y++) {

                                dest._value[index] += input._value[inputIndex] * wv;

                                index += outputChannels;
                                inputIndex += input.Shape[2];
                            }
                        }
                        weightIndex += outputChannels;
                    }


                });

                return dest;
            }

            public static (Tensor ie, Tensor we) Convolution2DPointwiseBackwards(Tensor input, Tensor error, Tensor weights, (Tensor ie, Tensor we) dest) {
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

                dest.ie.PointWise((a) => 0, true);
                System.Threading.Tasks.Parallel.For(0, outputChannels, (c) => {
                    int weightIndex = c;

                    for (int ic = 0; ic < input.Shape[2]; ic++) {

                        int index = c;
                        int inputIndex = ic;
                        float v = 0;
                        float w = weights._value[weightIndex];
                        for (int x = 0; x < width; x++) {
                            for (int y = 0; y < height; y++) {
                                dest.ie._value[inputIndex] += error._value[index] * w;
                                v += error._value[index] * input._value[inputIndex];
                                index += outputChannels;
                                inputIndex += input.Shape[2];
                            }
                        }
                        dest.we._value[weightIndex] = v;
                        weightIndex += outputChannels;
                    }


                });

                return dest;
            }


            public static Tensor Convolution2DDepthwise(Tensor input, Tensor filter,Tensor dest, (int x, int y) stride = default, bool pad = true) {
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
                padX /= 2;
                padY /= 2;

                //Tensor dest =
                //    new Tensor(
                //        shape,
                //        (input.Shape[1] - filter.Shape[1] + padY) / strideY + 1,
                //        input.Shape[2]);
                int findInt = channels * (input.Shape[1] - filter.Shape[1]);



                System.Threading.Tasks.Parallel.For(0, channels, (c) => {
                    int index = c;

                    int iWidth = input.Shape[0];
                    int iHeight = input.Shape[1];
                    int rWidth = dest.Shape[0];
                    int rHeight = dest.Shape[1];
                    int fWidth = filter.Shape[0];
                    int fHeight = filter.Shape[1];
                    int i_xStart = c - padX * channels * iHeight;
                    int i_xInterval = strideX * channels * iHeight;
                    int i_yStart = channels * -padY;
                    int i_yInterval = channels * strideY;

                    int i_x = i_xStart;
                    for (int x = 0; x < rWidth; x++) {
                        int i_y = i_yStart;
                        for (int y = 0; y < rHeight; y++) {

                            int fi = c;
                            int i = i_y + i_x;
                            int find = 0;
                            float v = 0;

                            int ix = x * strideX - padX;
                            for (int fx = 0; fx < fWidth; fx++) {
                                int iy = y * strideY - padY;
                                for (int fy = 0; fy < fHeight; fy++) {

                                    if (ix >= 0 && ix < iWidth && iy >= 0 && iy < iHeight) {
                                        v += input._value[i + find] * filter._value[fi];
                                    }


                                    fi += channels;
                                    find += channels;
                                    iy++;
                                }
                                dest._value[index] = v;
                                find += findInt;
                                ix++;
                            }
                            index += channels;
                            i_y += i_yInterval;
                        }
                        i_x += i_xInterval;
                    }
                });

                return dest;
            }

            public static (Tensor ie, Tensor we) Convolution2DDepthwiseBackwards(Tensor input, Tensor error, Tensor filter, (Tensor ie, Tensor we) dest, (int x, int y) stride = default, bool pad = true) {
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
                dest.ie.PointWise((a) => 0, true);
                dest.we.PointWise((a) => 0, true);

                System.Threading.Tasks.Parallel.For(0, channels, (c) => {
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
                            int find = 0;
                            float e = error._value[index];
                            int ix = x * strideX - padX;

                            for (int fx = 0; fx < fWidth; fx++) {
                                int iy = y * strideY - padY;
                                for (int fy = 0; fy < fHeight; fy++) {

                                    if (ix >= 0 && ix < iWidth && iy >= 0 && iy < iHeight) {
                                        dest.ie._value[i + find] += e * filter._value[fi];
                                        //dest.we._value[fi] += e * input._value[i + find];
                                        //Interlocked.Exchange(ref dest.ie._value[i + find], dest.ie._value[i + find] + e * filter._value[fi]);
                                        Interlocked.Exchange(ref dest.we._value[fi], dest.we._value[fi] + e * input._value[i + find]);
                                    }


                                    fi += channels;
                                    find += channels;
                                    iy++;
                                }
                                ix++;
                                find += channels * (iHeight - fHeight);
                            }
                            index += channels;
                            i_y += i_yInterval;
                        }
                        i_x += i_xInterval;
                    }
                });

                return dest;
            }


        }

    }
}