using Unity.Profiling;

using System.Collections.Generic;
namespace DumbML {
    public class Tanh  : Operation {


        public Tanh(Operation op) : base(op.shape, op) {
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(operands[0].Shape);
            operands[0].Tanh(result.tensor);
        }

        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            int s = e.Size;
            for (int i = 0; i < s; i++) {
                result[0].value[i] += e.value[i] * (value.value[i] + 1) * (.5f - value.value[i] / 2);
            }

        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Tanh(inner[0]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            return $"Sigmoid({inner[0].ExprString(false)})";
        }
    }
    
}