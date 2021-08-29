using System.Collections.Generic;


namespace DumbML {
    public class Detached : Operation {
        Tensor[] er;

        public Detached(Operation op) : base(op.shape, op) {
            er = new[] { new Tensor(op.shape) };
        }

        protected override Tensor _Compute(Tensor[] operands) {
            return value.CopyFrom(operands[0]);
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            return er;
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Detached(inner[0]._Copy(track));
        }
    }
}