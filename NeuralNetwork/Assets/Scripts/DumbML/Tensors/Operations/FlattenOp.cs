using System.Collections.Generic;
namespace DumbML {
    public class FlattenOp : Operation {
        public FlattenOp(Operation op): base(null, op) {
            int s = 1;
            foreach (var i in op.shape) {
                s *= i;
            }
            shape = new int[] { s };
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(shape);
            var v = operands[0].value;
            var r = result.tensor.value; 
            for (int i = 0; i < r.Length; i++) {
                r[i] =v[i];
            }
        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            var v = e.value;
            var r = result[0].value;

            for (int i = 0; i < e.value.Length; i++) {
                r[i] += v[i];
            }

        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new FlattenOp(inner[0]._Copy(track));
        }

    }

    
}