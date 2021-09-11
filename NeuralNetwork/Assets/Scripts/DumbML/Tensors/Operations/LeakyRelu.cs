using Unity.Profiling;

using System.Collections.Generic;
namespace DumbML {
    public class LeakyRelu : Operation {
        float leakyness = .001f;
        public LeakyRelu(Operation op) : base(op.shape, op) {
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(operands[0].Shape);
            result.tensor.PointWise(operands[0], (a, b) => (b > 0 ? b : b * leakyness), true);
        }

        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            int s = e.Size;
            for (int i = 0; i < s; i++) {
                result[0].value[i] += e.value[i] * (value.value[i] > 0 ? 1 :  leakyness);
            }
        }
        public override string ExprString(bool requireParanthesis) {
            return $"LeakyRelu({inner[0].ExprString(false)})";
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new LeakyRelu(inner[0]._Copy(track));
        }
    }

    
}