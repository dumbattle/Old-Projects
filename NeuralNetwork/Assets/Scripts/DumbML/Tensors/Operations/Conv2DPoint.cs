﻿using System.Collections.Generic;
namespace DumbML {
    public class Conv2DPoint : Operation {
        Tensor le, re;
        public Conv2DPoint(Operation op, Operation weight) : base(null, op, weight) {
            shape = new[] { op.shape[0], op.shape[1], weight.shape[1] };
            result = new Tensor(shape);
            le = new Tensor(op.shape);
            re = new Tensor(weight.shape);
        }

        public override Tensor Compute(Tensor[] operands) {
            return Blas.Parallel.Convolution2DPointwise(operands[0], operands[1], result);
        }
        public override Tensor[] BackwardsPass(Tensor e) {
            (le, re) = Blas.Parallel.Convolution2DPointwiseBackwards(inner[0].result, e, inner[1].result, (le, re));
            return new[] { le, re };
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Conv2DPoint(inner[0]._Copy(track), inner[1]._Copy(track));
        }
    }

    
}