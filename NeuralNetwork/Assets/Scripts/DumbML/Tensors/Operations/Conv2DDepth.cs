using System.Collections.Generic;
namespace DumbML {
    public class Conv2DDepth : Operation {
        Tensor[] error = new Tensor[2];
        public (int, int) stride;
        public bool pad;

        public Conv2DDepth(Operation op, Operation weight, (int x, int y) stride = default, bool pad = false) : base(null, op, weight) {
            if (op.shape[2] != weight.shape[2]) {}
            error[0] = new Tensor(op.shape);
            error[1] = new Tensor(weight.shape);

            int strideX = stride.x > 0 ? stride.x : 1;
            int strideY = stride.y > 0 ? stride.y : 1;
            this.stride = (strideX, strideY);

            this.pad = pad;

            var (padX, padY) = pad ? (weight.shape[0] - 1, weight.shape[1] - 1) : (0, 0);
            int shapeX = (op.shape[0] - weight.shape[0] + padX) / strideX + 1;
            int shapeY = (op.shape[1] - weight.shape[1] + padY) / strideY + 1;

            shape = new[] { shapeX, shapeY, weight.shape[2] };
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(shape);
            Blas.Parallel.Convolution2DDepthwise(operands[0], operands[1], result.tensor, stride, pad);
        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            Blas.Parallel.Convolution2DDepthwiseBackwards(inner[0].value, e, inner[1].value, (result[0], result[1]), stride, pad);
            result[0].Add(error[0], true);
            result[1].Add(error[1], true);
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Conv2DDepth(inner[0]._Copy(track), inner[1]._Copy(track), stride, pad);
        }
    }

    
}