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
                    throw new System.InvalidOperationException("Cant append Operations");
                }
                for (int j = 1; j < dim; j++) {
                    if (o.shape[j] != ops[0].shape[j]) {
                        throw new System.InvalidOperationException("Cant append Operations");
                    }
                    shape[j] = o.shape[j];
                }
            }
            value = new Tensor(shape);
        }

        protected override Tensor _Compute(Tensor[] operands) {

            int ind = 0;

            foreach (var o in operands) {
                for (int i = 0; i < o.value.Length; i++) {
                    value.value[ind] = o.value[i];
                    ind++;
                }
            }
            return value;
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            int ind = 0;

            foreach (var o in errors) {
                for (int i = 0; i < o.value.Length; i++) {
                    o.value[i] = e.value[ind];
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