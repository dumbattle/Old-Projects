using System.Collections.Generic;
namespace DumbML {
    public class FlattenOp : Operation {
        Tensor error;
        Tensor[] err = new Tensor[1];
        public FlattenOp(Operation op): base(null, op) {
            int s = 1;
            foreach (var i in op.shape) {
                s *= i;
            }
            shape = new int[] { s };
            value = new Tensor(shape);
            error = new Tensor(op.shape);
        }
        protected override Tensor _Compute(Tensor[] operands) {
            var v = operands[0].value;
            var r = value.value; 
            for (int i = 0; i < value.Size; i++) {
                r[i] =v[i];
            }
            return value;
        }
        protected override Tensor[] _BackwardsPass(Tensor e) {
            var v = e.value;
            var r = error.value;

            for (int i = 0; i < value.value.Length; i++) {
                r[i] = v[i];
            }

            err[0] = error;
            return err;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new FlattenOp(inner[0]._Copy(track));
        }

    }

    
}