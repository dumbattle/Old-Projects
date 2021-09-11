using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Square : Operation {

        public Square(Operation op) : base(op.shape, op) {

        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(operands[0].Shape);
            result.tensor.PointWise(operands[0], (_, b) => b * b, true);
        }

        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            int s = e.Size;
            for (int i = 0; i < s; i++) {
                result[0].value[i] += 2 * e.value[i] * inner[0].value.value[i];
            }
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Square(inner[0]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            return $"({inner[0].ExprString(false)})^2";
        }
    }    
}