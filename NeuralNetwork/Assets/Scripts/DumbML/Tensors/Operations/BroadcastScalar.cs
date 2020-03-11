using System.Collections.Generic;


namespace DumbML {
    public class BroadcastScalar : Operation {
        Tensor[] error;
        Tensor t;
        public BroadcastScalar(Operation scalar, params int[] shape) : base(shape, scalar) {
            error = new Tensor[] { t = new Tensor(1) };
        }

        protected override Tensor Compute(Tensor[] operands) {
            var s = operands[0][0];
            for (int i = 0; i < value.Size; i++) {
                value._value[i] = s;
            }
            return value;
        }
        protected override Tensor[] BackwardsPass(Tensor e) {
            t[0] = 0;
            foreach (var f in e) {
                t[0] += f;
            }
            return error;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new BroadcastScalar(inner[0]._Copy(track));
        }
    }
}