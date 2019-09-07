using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using DumbML;

public class ConvTest : MonoBehaviour {
    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

    NeuralNetwork nn;
    Optimizer o;
    Tensor input;
    Tensor target;
    public float loss;

    void Start() {
        //TextConvolution();

        input = new Tensor(() => Random.value, 5, 5, 3);

        BuildNetwork();

        target = nn.Compute(input);

        BuildNetwork();


        o = new AdaLerp(nn);

        void BuildNetwork() {
            nn = new NeuralNetwork(5, 5, 3);
            nn.Add(new FullConv((3, 3), 4, (1, 1), WeightInitializer.Default, LeakyRelu.Default, true));
            nn.Build();
        }
    }

    void Update() {
        loss = o.Train(input, target);
    }







    void TextConvolution() {
        int inputChannels = 10;
        int outputChannels = 10;
        int fx = 3;
        int fy = 3;

        int v = 1;
        Tensor t1 = new Tensor(() => v++, 10, 10, inputChannels);
        Tensor filter = new Tensor(() => 1, fx, fy, inputChannels, outputChannels);





        sw.Start();
        for (int i = 0; i < 10; i++) {
            BLAS2(t1, filter);
        }
        sw.Stop();
        print($"{sw.ElapsedMilliseconds}ms");




        sw.Reset();
        sw.Start();
        for (int i = 0; i < 10; i++) {
            Para(t1, filter);
        }
        sw.Stop();
        print($"{sw.ElapsedMilliseconds}ms");





        Tensor filter1 = new Tensor(() => 1, fx, fy, inputChannels);
        Tensor filter2 = new Tensor(() => 1, inputChannels, outputChannels);
        sw.Reset();
        sw.Start();
        for (int i = 0; i < 10; i++) {
            BLAS.Convolution2DPointwise(BLAS.Convolution2DDepthwise(t1, filter1), filter2);
        }
        sw.Stop();
        print($"{sw.ElapsedMilliseconds}ms");


        //print(t1);
        print(Convolution(t1, filter));
        print(BLAS2(t1, filter));
        print(Para(t1, filter));
        //print(Tensor.BLAS.Convolution2DPointwise(Tensor.BLAS.Convolution2DDepthwise(t1, filter1), filter2));
    }

    Tensor Convolution(Tensor input, Tensor filter, (int x, int y) stride = default, bool pad = true) {

        int strideX = stride.x > 0 ? stride.x : 1;
        int strideY = stride.y > 0 ? stride.y : 1;

        int outputChannels = filter.Shape[3];
        int inputChannels = input.Shape[2];

        var (padX, padY) = pad ? (filter.Shape[0] - 1, filter.Shape[1] - 1) : (0, 0);
        int width = (input.Shape[0] - filter.Shape[0] + padX) / strideX + 1;
        int height = (input.Shape[1] - filter.Shape[1] + padY) / strideY + 1;
        Tensor result = new Tensor(width, height, outputChannels);

        padX /= 2;
        padY /= 2;


        for (int oc = 0; oc < outputChannels; oc++) {
            for (int ic = 0; ic < inputChannels; ic++) {


                for (int x = 0; x < result.Shape[0]; x++) {
                    for (int y = 0; y < result.Shape[1]; y++) {


                        for (int fx = 0; fx < filter.Shape[0]; fx++) {
                            for (int fy = 0; fy < filter.Shape[1]; fy++) {

                                int ix = x * strideX + fx - padX;
                                int iy = y * strideY + fy - padY;

                                if (ix >= 0 && ix < input.Shape[0] && iy >= 0 && iy < input.Shape[1]) {
                                    result[x, y, oc] += input[ix, iy, ic] * filter[fx, fy, ic, oc];
                                }

                            }
                        }


                    }
                }


            }
        }
        return result;
    }
    Tensor BLAS2(Tensor input, Tensor filter, (int x, int y) stride = default, bool pad = true) {

        int strideX = stride.x > 0 ? stride.x : 1;
        int strideY = stride.y > 0 ? stride.y : 1;

        int outputChannels = filter.Shape[3];
        int inputChannels = input.Shape[2];

        var (padX, padY) = pad ? (filter.Shape[0] - 1, filter.Shape[1] - 1) : (0, 0);
        int width = (input.Shape[0] - filter.Shape[0] + padX) / strideX + 1;
        int height = (input.Shape[1] - filter.Shape[1] + padY) / strideY + 1;
        Tensor result = new Tensor(width, height, outputChannels);

        padX /= 2;
        padY /= 2;


        for (int oc = 0; oc < outputChannels; oc++) {


            int rind = oc;
            for (int x = 0; x < result.Shape[0]; x++) {
                int _ix = x * strideX - padX;
                for (int y = 0; y < result.Shape[1]; y++) {
                    int _iy = y * strideY - padY;

                    int find = oc;
                    for (int fx = 0; fx < filter.Shape[0]; fx++) {
                        for (int fy = 0; fy < filter.Shape[1]; fy++) {
                            for (int ic = 0; ic < inputChannels; ic++) {

                                int ix = _ix + fx;
                                int iy = _iy + fy;


                                if (ix >= 0 && ix < input.Shape[0] && iy >= 0 && iy < input.Shape[1]) {

                                    int iind = _iy + _ix * input.Shape[1];
                                    int ifind = fy + fx * input.Shape[1];

                                    result._value[rind] += input._value[(iind + ifind) * inputChannels + ic] * filter._value[find];
                                }

                                find += outputChannels;
                            }
                        }
                    }

                    rind += outputChannels;
                }
            }
        }
        return result;
    }
    Tensor Para(Tensor input, Tensor filter, (int x, int y) stride = default, bool pad = true) {

        int strideX = stride.x > 0 ? stride.x : 1;
        int strideY = stride.y > 0 ? stride.y : 1;

        int outputChannels = filter.Shape[3];
        int inputChannels = input.Shape[2];

        var (padX, padY) = pad ? (filter.Shape[0] - 1, filter.Shape[1] - 1) : (0, 0);
        int width = (input.Shape[0] - filter.Shape[0] + padX) / strideX + 1;
        int height = (input.Shape[1] - filter.Shape[1] + padY) / strideY + 1;
        Tensor result = new Tensor(width, height, outputChannels);

        padX /= 2;
        padY /= 2;

        int ifxInterval = input.Shape[1] * inputChannels;
        int ixInterval = strideX * input.Shape[1] * inputChannels;
        int iyInterval = strideY * inputChannels;

        Parallel.For(0, outputChannels, (oc) => {
            int rind = oc;
            int ix =  - padX * ifxInterval;

            for (int x = 0; x < result.Shape[0]; x++) {
                int iy = -padY * inputChannels;

                for (int y = 0; y < result.Shape[1]; y++) {
                    int find = oc;

                    for (int fx = 0; fx < filter.Shape[0]; fx++) {
                        int ifind = fx * ifxInterval;

                        for (int fy = 0; fy < filter.Shape[1]; fy++) {

                            if (ix >= -fx * ifxInterval &&
                            ix < (input.Shape[0] - fx) * ifxInterval &&
                            iy >= -fy * inputChannels &&
                            iy < (input.Shape[1] - fy) * inputChannels) 

                            {
                                int i = ix + iy + ifind;
                                for (int ic = 0; ic < inputChannels; ic++) {
                                    result._value[rind] += input._value[i + ic] * filter._value[find];
                                }
                            }

                            ifind += inputChannels;
                            find += outputChannels;
                        }
                    }
                    iy += iyInterval;
                    rind += outputChannels;
                }
                ix +=ixInterval;
            }
        });
        return result;
    }

}