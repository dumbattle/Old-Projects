using System;
using System.Threading;


namespace DumbML {
    public static partial class Blas {
        public static partial class Parallel {

            public static unsafe Tensor FullConv2D(Tensor input, Tensor filters, Tensor output, (int x, int y) stride, bool pad) {
                //error checking
                if (input.Rank != 3 && input.Rank != 2) {
                    throw new ArgumentException("Input tensor must be 3D in order to perform 2D convolution. Got: " + input.Rank);
                }
                if (filters.Rank != 4) {
                    throw new ArgumentException("2D pointwise convolution requires filters to be " +
                        "4D[filter x, filter y, input channels, output channels]. Got: " + filters.Rank, "dimensions");
                }
                int inputChannels = input.Rank == 2 ? 1 : input.Shape[2];

                if (inputChannels != filters.Shape[2]) {
                    throw new InvalidOperationException($"Number of Input channels ({inputChannels}) " +
                        $"does not match size of input filters: ({filters.Shape[2]})");
                }

                int strideX = stride.x > 0 ? stride.x : 1;
                int strideY = stride.y > 0 ? stride.y : 1;
                int outputChannels = filters.Shape[3];

                var (padX, padY) = pad ? (filters.Shape[0] - 1, filters.Shape[1] - 1) : (0, 0);
                int shape = (input.Shape[0] - filters.Shape[0] + padX) / strideX + 1;

                padX /= 2;
                padY /= 2;

                int outWidth = (input.Shape[0] - filters.Shape[0] + padX) / strideX + 1;
                int outHeight = (input.Shape[1] - filters.Shape[1] + padY) / strideY + 1;
                int filterWidth = filters.Shape[0];
                int filterHeight = filters.Shape[1];

                //cache
                int f_x_delta = outputChannels * inputChannels * filterHeight;
                int i_s_x_interval = strideX * inputChannels * input.Shape[1];
                int i_f_x_interval = inputChannels * input.Shape[1];

                float[] input_arr = input.value;
                float[] filters_arr = filters.value;
                float[] output_arr = output.value;


                System.Threading.Tasks.Parallel.For(0, outputChannels, (oc) => {
                    fixed (float* ip = input_arr, fp = filters_arr, op = output_arr) {
                        int outputIndex = oc;
                        for (int x = 0; x < outWidth; x++) {
                            for (int y = 0; y < outHeight; y++) {
                                float v = 0;

                                for (int ic = 0; ic < inputChannels; ic++) {
                                    int inputStart =
                                        x * i_s_x_interval +
                                        y * strideY * inputChannels +
                                        ic;
                                    int i_f_x = 0;

                                    int filterIndex = ic * outputChannels + oc;
                                    for (int fx = 0; fx < filterWidth; fx++) {
                                        for (int fy = 0; fy < filterHeight; fy++) {
                                            //v += input[x * strideX + fx, y * strideY + fy, ic] * filters[fx, fy, ic, oc];
                                            int inputIndex =
                                                inputStart +
                                                i_f_x +
                                                fy * inputChannels;

                                            v += ip[inputIndex] * fp[filterIndex];
                                            filterIndex += outputChannels * inputChannels;
                                        }
                                        i_f_x += i_f_x_interval;
                                    }
                                }

                                outputIndex += outputChannels;
                                op[outputIndex] = v;
                            }

                        }
                    }
                });
                return output;
            }

            public static unsafe (Tensor ie, Tensor fe) FullConv2DBackwards(Tensor input, Tensor error, Tensor filters, (Tensor ie, Tensor fe) output, (int x, int y) stride, bool pad) {
                //error checking
                if (input.Rank != 3 && input.Rank != 2) {
                    throw new ArgumentException("Input tensor must be 3D in order to perform 2D convolution. Got: " + input.Rank);
                }
                if (filters.Rank != 4) {
                    throw new ArgumentException("2D pointwise convolution requires filters to be " +
                        "4D[filter x, filter y, input channels, output channels]. Got: " + filters.Rank, "dimensions");
                }
                int inputChannels = input.Rank == 2 ? 1 : input.Shape[2];

                if (inputChannels != filters.Shape[2]) {
                    throw new InvalidOperationException($"Number of Input channels ({inputChannels}) " +
                        $"does not match size of input filters: ({filters.Shape[2]})");
                }

                int strideX = stride.x > 0 ? stride.x : 1;
                int strideY = stride.y > 0 ? stride.y : 1;
                int outputChannels = filters.Shape[3];

                var (padX, padY) = pad ? (filters.Shape[0] - 1, filters.Shape[1] - 1) : (0, 0);
                int shape = (input.Shape[0] - filters.Shape[0] + padX) / strideX + 1;

                padX /= 2;
                padY /= 2;

                int outWidth = (input.Shape[0] - filters.Shape[0] + padX) / strideX + 1;
                int outHeight = (input.Shape[1] - filters.Shape[1] + padY) / strideY + 1;
                int filterWidth = filters.Shape[0];
                int filterHeight = filters.Shape[1];

                output.ie.PointWise((a) => 0, true);
                output.fe.PointWise((a) => 0, true);

                //cache
                int f_x_delta = outputChannels * inputChannels * filterHeight;
                int i_s_x_interval = strideX * inputChannels * input.Shape[1];
                int i_f_x_interval = inputChannels * input.Shape[1];

                float[] oie_arr = output.ie.value;
                float[] ofe_arr = output.fe.value;
                float[] input_arr = input.value;
                float[] filters_arr = filters.value;


                System.Threading.Tasks.Parallel.For(0, outputChannels, (oc) => {
                    fixed (float* ip = input_arr, fp = filters_arr, oip = oie_arr, ofp = ofe_arr, ep = error.value) {
                        for (int x = 0; x < outWidth; x++) {
                            for (int y = 0; y < outHeight; y++) {

                                int eIndex =
                                    x * outputChannels * outHeight +
                                    y * outputChannels +
                                    oc;
                                float e = ep[eIndex];

                                for (int ic = 0; ic < inputChannels; ic++) {
                                    int inputStart =
                                            x * i_s_x_interval +
                                            y * strideY * inputChannels +
                                            ic;
                                    int i_f_x = 0;
                                    int filterIndex = ic * outputChannels + oc;
                                    for (int fx = 0; fx < filterWidth; fx++) {
                                        for (int fy = 0; fy < filterHeight; fy++) {

                                            int inputIndex =
                                                inputStart +
                                                i_f_x +
                                                fy * inputChannels;

                                            //output.ie[x * strideX + fx, y * strideY + fy, ic] += e * filters[fx, fy, ic, oc]; // <= not thread safe
                                            //output.fe[fx, fy, ic, oc] += e * input[x * strideX + fx, y * strideY + fy, ic];
                                            Interlocked.Exchange(ref oie_arr[inputIndex], oip[inputIndex] + e * fp[filterIndex]);
                                            ofp[filterIndex] += e * ip[inputIndex];

                                            filterIndex += outputChannels * inputChannels;
                                        }
                                        i_f_x += i_f_x_interval;
                                    }
                                }
                            }
                        }
                    }
                });
                return output;
            }
        }
    }
}
