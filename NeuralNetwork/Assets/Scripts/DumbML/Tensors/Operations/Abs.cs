using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Abs : Operation {

        public Abs(Operation op) : base(op.shape, op) {
        }

        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(operands[0].Shape);
            result.tensor.PointWise(operands[0], (a, b) => b >= 0 ? b : -b, true);
        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            int s = e.Size;
            var error = result[0];
            for (int i = 0; i < s; i++) {
                error.value[i] += inner[0].value.value[i] >= 0 ? e.value[i] : -e.value[i];
            }
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Abs(inner[0]._Copy(track));
        }


        public override string ExprString(bool requireParanthesis) {
            return $"|{inner[0].ExprString(false)}|";
        }
    }

    
}