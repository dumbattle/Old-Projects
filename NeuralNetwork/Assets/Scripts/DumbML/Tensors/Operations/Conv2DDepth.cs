﻿namespace DumbML {
    public class Conv2DDepth : Operation {
        Tensor le, re;
        (int, int) stride;
        bool pad;

        public Conv2DDepth(Operation op, Operation weight, (int x, int y) stride = default, bool pad = false) : base(null, op, weight) {
            if (op.shape[2] != weight.shape[2]) {
            }
            le = new Tensor(op.shape);
            re = new Tensor(weight.shape);

            int strideX = stride.x > 0 ? stride.x : 1;
            int strideY = stride.y > 0 ? stride.y : 1;
            this.stride = (strideX, strideY);

            this.pad = pad;

            var (padX, padY) = pad ? (weight.shape[0] - 1, weight.shape[1] - 1) : (0, 0);
            int shapeX = (op.shape[0] - weight.shape[0] + padX) / strideX + 1;
            int shapeY = (op.shape[1] - weight.shape[1] + padY) / strideY + 1;

            shape = new[] { shapeX, shapeY, weight.shape[2] };
            //Debug.Log(shape.TOSTRING());
            result = new Tensor(shape);
        }

        public override Tensor Compute(Tensor[] operands) {
            return result = Blas.Parallel.Convolution2DDepthwise(operands[0], operands[1], result, stride, pad);
        }
        public override Tensor[] BackwardsPass(Tensor e) {
            (le, re) = Blas.Parallel.Convolution2DDepthwiseBackwards(inner[0].result, e, inner[1].result, (le, re), stride, pad);
            return new[] { le, re };
        }
    }

    
}