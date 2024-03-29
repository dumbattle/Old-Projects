﻿using System.Collections.Generic;
namespace DumbML {
    public class Conv2DPoint : Operation {
        Tensor le, re;
        public Conv2DPoint(Operation op, Operation weight) : base(null, op, weight) {
            shape = new[] { op.shape[0], op.shape[1], weight.shape[1] };
            le = new Tensor(op.shape);
            re = new Tensor(weight.shape);
        }

        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(shape);
            Blas.Parallel.Convolution2DPointwise(operands[0], operands[1], result.tensor);
        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            (le, re) = Blas.Parallel.Convolution2DPointwiseBackwards(inner[0].value, e, inner[1].value, (le, re));
            result[0].Add(le, true); ;
            result[1].Add(re, true); ;
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Conv2DPoint(inner[0]._Copy(track), inner[1]._Copy(track));
        }
    }

    
}