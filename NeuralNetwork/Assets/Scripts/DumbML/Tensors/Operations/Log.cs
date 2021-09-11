using System.Collections.Generic;
using System;


namespace DumbML {
    public class Log : Operation {
        public Log(Operation op) : base(op.shape, op) {
        }

        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(operands[0].Shape);

   
            for (int i = 0; i < operands[0].Size; i++) {
                var v = (float)Math.Log(operands[0].value[i] + EPSILON);
                result.tensor.value[i] = v;
            }


        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            for (int i = 0; i < e.Size; i++) {
                result[0].value[i] += e.value[i] / (inner[0].value.value[i] + EPSILON);
            }
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Log(inner[0]._Copy(track));
        }


        class Cache {
            public Tensor o;
            public Tensor e;
        }
    }
}