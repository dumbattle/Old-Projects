using System.Collections.Generic;
namespace DumbML {
    public class FlattenOp : Operation {
        Tensor error;
        public FlattenOp(Operation op): base(null, op) {
            shape = new int[] { op.value.Size };
            value = new Tensor(shape);
            error = new Tensor(op.shape);
        }
        protected override Tensor Compute(Tensor[] operands) {
            var v = operands[0]._value;
            var r = value._value; 
            for (int i = 0; i < value.Size; i++) {
                r[i] =v[i];
            }
            return value;
        }
        protected override Tensor[] BackwardsPass(Tensor e) {
            var v = e._value;
            var r = error._value;

            for (int i = 0; i < value.Size; i++) {
                r[i] = v[i];
            }

            return new[] { error };
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new FlattenOp(inner[0]._Copy(track));
        }

    }

    
}