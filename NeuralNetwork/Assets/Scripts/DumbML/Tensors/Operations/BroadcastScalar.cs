using System.Collections.Generic;


namespace DumbML {
    public class BroadcastScalar : Operation {
        Tensor[] error;
        Tensor t;
        public BroadcastScalar(Operation scalar, params int[] shape) : base(shape, scalar) {
            error = new Tensor[] { t = new Tensor(1) };
        }

        protected override Tensor _Compute(Tensor[] operands) {
            var s = operands[0][0];
            for (int i = 0; i < value.Size; i++) {
                value.value[i] = s;
            }
            return value;
        }
        protected override Tensor[] _BackwardsPass(Tensor e) {
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