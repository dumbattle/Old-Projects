using System.Collections.Generic;

namespace DumbML {
    public class Append : Operation {
        Tensor[] errors;
        public Append(params Operation[] ops) : base(null, ops) {
            int dim = ops[0].shape.Length;
            shape = new int[dim];
            errors = new Tensor[ops.Length];

            for (int i = 0; i < ops.Length; i++) {
                Operation o = (Operation)ops[i];
                shape[0] += o.shape[0];
                errors[i] = new Tensor(ops[i].shape);

                if (o.shape.Length != dim) {
                    throw new System.InvalidOperationException("Cant append tensors");
                }
                for (int j = 1; j < dim; j++) {
                    if (o.shape[j] != ops[0].shape[j]) {
                        throw new System.InvalidOperationException("Cant append tensors");
                    }
                    shape[j] = o.shape[j];
                }
            }
            result = new Tensor(shape);
        }

        protected override Tensor Compute(Tensor[] operands) {

            int ind = 0;

            foreach (var o in operands) {
                for (int i = 0; i < o._value.Length; i++) {
                    result._value[ind] = o._value[i];
                    ind++;
                }
            }
            return result;
        }

        protected override Tensor[] BackwardsPass(Tensor e) {
            int ind = 0;

            foreach (var o in errors) {
                for (int i = 0; i < o._value.Length; i++) {
                    o._value[i] = e._value[ind];
                    ind++;
                }
            }
            return errors;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Append(CopyInner(track));
        }
    }
}