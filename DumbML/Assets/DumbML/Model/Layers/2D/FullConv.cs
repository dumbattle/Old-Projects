using System;
using System.Threading.Tasks;
using FullSerializer;

namespace DumbML {
    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class FullConv : Layer {
        [fsProperty]
        public Tensor Weights;
        [fsProperty]
        public ActivationFunction af;
        [fsProperty]
        public bool pad;
        [fsProperty]
        public (int x, int y) stride;

        protected Func<float> weightInitializer;

        public FullConv((int x, int y) filterSize, int numOutputChannels, (int x, int y) stride = default, Func<float> weightInitializer = null, ActivationFunction af = null, bool pad = false) {

            this.stride = (stride.x > 0 ? stride.x : 1, stride.y > 0 ? stride.y : 1);
            this.af = af ?? ActivationFunction.None;
            outputShape = new int[] { filterSize.x, filterSize.y, numOutputChannels };
            this.weightInitializer = weightInitializer;
            this.pad = pad;
        }

        public override void Build(Layer prevLayer) {
            inputShape = (int[])prevLayer.outputShape.Clone();

            Weights = new Tensor(weightInitializer, outputShape[0], outputShape[1], inputShape[2], outputShape[2]);

            var (padX, padY) = pad ? (Weights.Shape[0] - 1, Weights.Shape[1] - 1) : (0, 0);
            var (width, height) = (
                    (inputShape[0] - Weights.Shape[0] + padX) / stride.x + 1,
                    (inputShape[1] - Weights.Shape[1] + padY) / stride.y + 1);

            outputShape[0] = width;
            outputShape[1] = height;
            IsBuilt = true;
        }

        public override Tensor Compute(Tensor input) {
            var (padX, padY) = pad ? (Weights.Shape[0] - 1, Weights.Shape[1] - 1) : (0, 0);
            int width = (input.Shape[0] - Weights.Shape[0] + padX) / stride.x + 1;
            int height = (input.Shape[1] - Weights.Shape[1] + padY) / stride.y + 1;

            padX /= 2;
            padY /= 2;
            output = new Tensor(outputShape);

            int outputChannels = output.Shape[2];
            int inputChannels = input.Shape[2];

            int ifxInterval = input.Shape[1] * inputChannels;
            int ixInterval = stride.x * input.Shape[1] * inputChannels;
            int iyInterval = stride.y * inputChannels;

            Parallel.For(0, outputChannels, (oc) => {
                int rind = oc;
                int ix = -padX * ifxInterval;

                for (int x = 0; x < output.Shape[0]; x++) {
                    int iy = -padY * inputChannels;

                    for (int y = 0; y < output.Shape[1]; y++) {
                        int find = oc;

                        for (int fx = 0; fx < Weights.Shape[0]; fx++) {
                            int ifind = fx * ifxInterval;

                            for (int fy = 0; fy < Weights.Shape[1]; fy++) {

                                if (ix >= -fx * ifxInterval &&
                                ix < (input.Shape[0] - fx) * ifxInterval &&
                                iy >= -fy * inputChannels &&
                                iy < (input.Shape[1] - fy) * inputChannels) {
                                    int i = ix + iy + ifind;
                                    for (int ic = 0; ic < inputChannels; ic++) {
                                        output._value[rind] += input._value[i + ic] * Weights._value[find];
                                    }
                                }

                                ifind += inputChannels;
                                find += outputChannels;
                            }
                        }
                        iy += iyInterval;
                        rind += outputChannels;
                    }
                    ix += ixInterval;
                }
            });

            output = output.PointWise((a) => af.Activate(a));
            return output;
        }
        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            Tensor input = context.input;
            Tensor ierror = input.SameShape();
            Tensor grad = Weights.SameShape();

            var (padX, padY) = pad ? (Weights.Shape[0] - 1, Weights.Shape[1] - 1) : (0, 0);
            int width = (input.Shape[0] - Weights.Shape[0] + padX) / stride.x + 1;
            int height = (input.Shape[1] - Weights.Shape[1] + padY) / stride.y + 1;

            padX /= 2;
            padY /= 2;

            error *= context.output.PointWise((f) => af.Derivative(f));

            int outputChannels = output.Shape[2];
            int inputChannels = input.Shape[2];

            int ifxInterval = input.Shape[1] * inputChannels;
            int ixInterval = stride.x * input.Shape[1] * inputChannels;
            int iyInterval = stride.y * inputChannels;

            Parallel.For(0, outputChannels, (oc) => {
                int rind = oc;
                int ix = -padX * ifxInterval;

                for (int x = 0; x < output.Shape[0]; x++) {
                    int iy = -padY * inputChannels;

                    for (int y = 0; y < output.Shape[1]; y++) {
                        int find = oc;

                        for (int fx = 0; fx < Weights.Shape[0]; fx++) {
                            int ifind = fx * ifxInterval;

                            for (int fy = 0; fy < Weights.Shape[1]; fy++) {

                                if (ix >= -fx * ifxInterval &&
                                ix < (input.Shape[0] - fx) * ifxInterval &&
                                iy >= -fy * inputChannels &&
                                iy < (input.Shape[1] - fy) * inputChannels) {
                                    int i = ix + iy + ifind;
                                    for (int ic = 0; ic < inputChannels; ic++) {
                                        ierror._value[i + ic] += error._value[rind] * Weights._value[find];
                                        grad._value[find] += error._value[rind] * input._value[i + ic];
                                    }
                                }

                                ifind += inputChannels;
                                find += outputChannels;
                            }
                        }
                        iy += iyInterval;
                        rind += outputChannels;
                    }
                    ix += ixInterval;
                }
            });


            grad /= output.Shape[0] * output.Shape[1];
            return (ierror, new JaggedTensor(grad));
        }
        public override void Update(JaggedTensor gradients) {
            Weights -= gradients.Value;
        }
    }
}