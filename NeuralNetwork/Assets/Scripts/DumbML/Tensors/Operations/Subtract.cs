using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Subtract : Operation {
        Tensor rError;

        public Subtract(Operation left, Operation right) : base(left.shape, left,right) {
            rError = new Tensor(right.shape);

        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(operands[0].Shape);

            for (int i = 0; i < result.tensor.Size; i++) {
                result.tensor.value[i] = operands[0].value[i] - operands[1].value[i];
            }
        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            rError.PointWise(e, (_, b) => -b, true);

            result[0].Add(e, true);
            result[1].Add(rError, true);

        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Subtract(inner[0]._Copy(track),inner[1]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            if (requireParanthesis) {
                return $"({inner[0].ExprString(false)} - {inner[1].ExprString(true)})";
            }
            return $"{inner[0].ExprString(false)} - {inner[1].ExprString(true)}";
        }
    }

    
}