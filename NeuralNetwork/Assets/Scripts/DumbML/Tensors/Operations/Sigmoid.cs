using Unity.Profiling;

using System.Collections.Generic;
namespace DumbML {
    public class Sigmoid : Operation {
        public Sigmoid(Operation op) : base(op.shape, op) {
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {

            var s = operands[0].Shape;
            result.SetShape(s);

            operands[0].Sigmoid(result.tensor);

        }

        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            int s = e.Size;
            for (int i = 0; i < s; i++) {
                result[0].value[i] += e.value[i] * value.value[i] * (1 - value.value[i]);
            }
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Sigmoid(inner[0]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            return $"Sigmoid({inner[0].ExprString(false)})";
        }

        class Cache {
            public Tensor o;
            public Tensor e;
        }
    }
    
}