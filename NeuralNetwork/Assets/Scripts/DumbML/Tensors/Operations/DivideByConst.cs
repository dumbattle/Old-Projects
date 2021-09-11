using System.Collections.Generic;

namespace DumbML {
    public class DivideByConst : Operation {
        float f;

        public DivideByConst(Operation op, float f) : base(op.shape, op) {
            this.f = f;
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(operands[0].Shape);

            for (int i = 0; i < result.tensor.Size; i++) {
                result.tensor.value[i] = operands[0].value[i] / f;
            }
        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            for (int i = 0; i < e.Size; i++) {
                result[0].value[i] += e.value[i] / f;
            }
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            throw new System.NotImplementedException();
        }
    }

    
}