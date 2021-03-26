using System.Collections.Generic;
using System;


namespace DumbML {
    public class Log : Operation {
        Tensor[] error;
        Tensor t;
        public Log(Operation op) : base(op.shape, op) {
            error = new Tensor[1] { t = value.SameShape() };
        }

        protected override Tensor _Compute(Tensor[] operands) {
            for (int i = 0; i < operands[0].Size; i++) {
                value.value[i] = (float)Math.Log(operands[0].value[i] + EPSILON);
            }

            return value;
        }
        protected override Tensor[] _BackwardsPass(Tensor e) {
            for (int i = 0; i < e.Size; i++) {
                t.value[i] = e.value[i] / (inner[0].value.value[i] + EPSILON);
            }
            return error;
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Log(inner[0]._Copy(track));
        }
    }
}