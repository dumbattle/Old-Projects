﻿using System;
using FullSerializer;

namespace DumbML {
    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class Deconv2DDepthwise : Layer {
        [fsProperty]
        public Tensor Weights;
        [fsProperty]
        public ActivationFunction af;
        [fsProperty]
        public bool pad;
        [fsProperty]
        public (int x, int y) stride;

        protected Func<float> weightInitializer;


        public Deconv2DDepthwise((int x, int y) filterSize, (int x, int y) stride = default, Func<float> weightInitializer = null, ActivationFunction af = null, bool pad = false) {
            this.weightInitializer = weightInitializer ?? WeightInitializer.Default;
            outputShape = new int[] { filterSize.x, filterSize.y, 0 };

            this.af = af ?? ActivationFunction.None;
            this.pad = pad;
            this.stride = (
                stride.x > 0 ? stride.x : 1,
                stride.y > 0 ? stride.y : 1);
        }

        public override void Build(Layer prevLayer) {
            if (IsBuilt) {
                if (prevLayer.outputShape.TOSTRING() != inputShape.TOSTRING()) {
                    throw new RankException("Cannot connect layers. Input layer is wrong size");
                }
                else {
                    return;
                }
            }

            inputShape = (int[])prevLayer.outputShape.Clone();
            Weights = new Tensor(weightInitializer, outputShape[0], outputShape[1], inputShape[2]);


            (int padX, int padY) = pad ? (Weights.Shape[0] - 1, Weights.Shape[1] - 1) : (0, 0);

            int width = stride.x * (inputShape[0] - 1) + Weights.Shape[0] - padX;
            int height = stride.y * (inputShape[1] - 1) + Weights.Shape[1] - padY;
      

            outputShape = new int[] { width, height, inputShape[2] };
            IsBuilt = true;
        }

        public override Tensor Compute(Tensor input) {
            Tensor result = BLAS.Deconvolution2DDepthwise(input, Weights, stride, pad);
            //result = result.PointWise((a) => af.Activate(a),true);
            output = result;
            return result;
        }


        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            //error *= context.output.PointWise((f) => af.Derivative(f));
            //error.PointWise(context.output, (e, o) => e * af.Derivative(o), true);
            (Tensor inputError, Tensor grad) = BLAS.Deconvolution2DDepthwiseBackwards(context.input, error, Weights, stride, pad);
            grad /= context.input.Shape[0] * context.input.Shape[1];
            return (inputError, new JaggedTensor(grad));
        }

        public override void Update(JaggedTensor gradients) {
            //Weights.PointWise(gradients.Value, (w, g) => w - g, true);
            Weights.Subtract(gradients.Value, true);
        }
    }

}