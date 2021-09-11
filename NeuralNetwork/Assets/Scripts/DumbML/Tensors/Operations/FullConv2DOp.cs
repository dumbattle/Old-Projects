using System;
using System.Collections.Generic;


namespace DumbML {
    public class FullConv2D : Operation {
        Tensor[] error = new Tensor[2];

        public (int, int) stride;
        public bool pad;

        public FullConv2D(Operation op, Operation filters, (int x, int y) stride = default, bool pad = false) : base(null, op, filters) {
            //error checking
            if (op.shape.Length != 3 && op.shape.Length != 2) {
                throw new ArgumentException("Input tensor must be 3D in order to perform 2D convolution. Got: " + op.shape.Length);
            }
            if (filters.shape.Length != 4) {
                throw new ArgumentException("2D pointwise convolution requires filters to be " +
                    "4D[filter x, filter y, input channels, output channels]. Got: " + filters.shape.Length, "dimensions");
            }
            int inputChannels = op.shape.Length == 2 ? 1 : op.shape[2];

            if (inputChannels != filters.shape[2]) {
                throw new InvalidOperationException($"Number of Input channels ({inputChannels}) " +
                    $"does not match size of input filters: ({filters.shape[2]})");
            }

            error[0] = new Tensor(op.shape);
            error[1] = new Tensor(filters.shape);

            int strideX = stride.x > 0 ? stride.x : 1;
            int strideY = stride.y > 0 ? stride.y : 1;
            this.stride = (strideX, strideY);

            this.pad = pad;

            var (padX, padY) = pad ? (filters.shape[0] - 1, filters.shape[1] - 1) : (0, 0);
            int shapeX = (op.shape[0] - filters.shape[0] + padX) / strideX + 1;
            int shapeY = (op.shape[1] - filters.shape[1] + padY) / strideY + 1;

            shape = new[] { shapeX, shapeY, filters.shape[3] };
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(shape);
            Blas.Parallel.FullConv2D(operands[0], operands[1], result.tensor, stride, pad);

        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            Blas.Parallel.FullConv2DBackwards(inner[0].value, e, inner[1].value, (error[0], error[1]), stride, pad);
            result[0].Add(error[0], true);
            result[1].Add(error[1], true);
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new FullConv2D(inner[0]._Copy(track), inner[1]._Copy(track), stride, pad);
        }

    }
}