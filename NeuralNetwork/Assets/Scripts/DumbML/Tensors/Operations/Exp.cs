using System.Collections.Generic;
using System;


namespace DumbML {
    public class Exp : Operation {
        Tensor[] error;
        Tensor t;
        public Exp (Operation op) : base(op.shape, op) {
            error = new Tensor[1] {t = value.SameShape() };
        }

        protected override Tensor _Compute(Tensor[] operands) {
            for (int i = 0; i < operands[0].Size; i++) {
                value.value[i] = (float)Math.Exp(operands[0].value[i]);
            }

            return value;
        }
        protected override Tensor[] _BackwardsPass(Tensor e) {
            for (int i = 0; i < e.Size; i++) {
                t.value[i] = value.value[i] * e.value[i]; ;
            }
            return error;
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Exp(inner[0]._Copy(track));
        }
    }
}