using System.Collections.Generic;
using System;


namespace DumbML {
    public class Exp : Operation {
        public Exp (Operation op) : base(op.shape, op) {
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(operands[0].Shape);

            for (int i = 0; i < operands[0].Size; i++) {
                result.tensor.value[i] = (float)Math.Exp(operands[0].value[i]);
            }

        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            for (int i = 0; i < e.Size; i++) {
                result[0].value[i] += value.value[i] * e.value[i]; ;
            }
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Exp(inner[0]._Copy(track));
        }
    }
}