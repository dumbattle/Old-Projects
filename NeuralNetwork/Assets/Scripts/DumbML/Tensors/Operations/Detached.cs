using System.Collections.Generic;


namespace DumbML {
    public class Detached : Operation {

        public Detached(Operation op) : base(op.shape, op) {
        }

        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(operands[0].Shape);
            result.tensor.CopyFrom(operands[0]);
        }

        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Detached(inner[0]._Copy(track));
        }
    }
}