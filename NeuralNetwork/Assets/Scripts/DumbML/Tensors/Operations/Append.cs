using System.Collections.Generic;

namespace DumbML {
    public class Append : Operation {
        int[] shapeCache;
        public Append(params Operation[] ops) : base(null, ops) {
            int dim = ops[0].shape.Length;
            shape = new int[dim];
            shapeCache = new int[dim];
            for (int i = 0; i < ops.Length; i++) {
                Operation o = (Operation)ops[i];
                shape[0] += o.shape[0];

                if (o.shape.Length != dim) {
                    throw new System.InvalidOperationException("Cant append Operations");
                }
                for (int j = 1; j < dim; j++) {
                    if (o.shape[j] != ops[0].shape[j]) {
                        throw new System.InvalidOperationException("Cant append Operations");
                    }
                    shape[j] = o.shape[j];
                }
            }
        }

        protected override void _Compute(Tensor[] operands, TensorCache result) {
          
            result.SetShape(shape);

            int ind = 0;

            foreach (var o in operands) {
                for (int i = 0; i < o.value.Length; i++) {
                    result.tensor.value[ind] += o.value[i];
                    ind++;
                }
            }
        }

        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            int ind = 0;

            foreach (var o in result) {
                for (int i = 0; i < o.value.Length; i++) {
                    o.value[i] += e.value[ind];
                    ind++;
                }
            }
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Append(CopyInner(track));
        }
    }
}