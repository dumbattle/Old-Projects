namespace DumbML {
    public class FlattenOp : Operation {
        Tensor error;
        public FlattenOp(Operation op): base(null, op) {
            shape = new int[] { op.result.Size };
            result = new Tensor(shape);
            error = new Tensor(op.shape);
        }
        public override Tensor Compute(Tensor[] operands) {
            var v = operands[0]._value;
            var r = result._value; 
            for (int i = 0; i < result.Size; i++) {
                r[i] =v[i];
            }
            return result;
        }
        public override Tensor[] BackwardsPass(Tensor e) {
            var v = e._value;
            var r = error._value;

            for (int i = 0; i < result.Size; i++) {
                r[i] = v[i];
            }

            return new[] { error };
        }

    }

    
}