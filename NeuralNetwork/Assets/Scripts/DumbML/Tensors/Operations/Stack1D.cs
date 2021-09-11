using System.Collections.Generic;

namespace DumbML {

    /// <summary>
    /// Combine multiple 1D Operations of same size
    /// N x [s] => [N x s]
    /// </summary>
    public class Stack1D : Operation {
        Tensor[] errors;
        public Stack1D(params Operation[] ops) : base(new[] { ops.Length, ops[0].shape[0] }, ops) {
            errors = new Tensor[ops.Length];
            var length = ops[0].shape[0];

            for (int i = 0; i < ops.Length; i++) {
                Operation o = ops[i];

                if (o.shape.Rank != 1) {
                    throw new System.InvalidOperationException($"Invalid rank: {o.value.Rank}");
                }
                if (o.shape[0] != length) {
                    throw new System.InvalidOperationException($"Invalid lenght: {o.shape[0]}");
                }
                errors[i] = new Tensor(ops[i].shape);
            }
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(shape);

            for (int i = 0; i < operands.Length; i++) {
                var op = operands[i];
                for (int j = 0; j < op.Size; j++) {
                    result.tensor[i, j] = op[j];
                }
            }
        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            for (int i = 0; i < errors.Length; i++) {
                var err = result[i];
                for (int j = 0; j < err.Size; j++) {
                    err[j] += e[i, j];
                }
            }
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Stack1D(CopyInner(track));
        }
    }


    /// <summary>
    /// Stacks multiple 1D tensors of same size.
    /// Can handle different amounts, but can only be used as input operation
    /// </summary>
    public class VariableStack1D : Operation {
        int[] shapeCache = new int[2];
        int size;
        TensorCache val = new TensorCache();
        public VariableStack1D(int size) : base(null) {
            shape = new[] { -1, size };
            this.size = size;
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(shapeCache);
            result.tensor.CopyFrom(val.tensor);
        }

        public void SetValue(params Tensor[] t) {
            int l = t.Length;
            shapeCache[0] = l;
            shapeCache[1] = size;

            val.SetShape(shapeCache);

   


            for (int i = 0; i < l; i++) {
                Tensor ten = t[i];
                for (int x = 0; x < size; x++) {
                    val.tensor[i, x] = ten[x];
                }
            }
        }  
        
        
        public void SetValue(List<Tensor> t) {
            int l = t.Count;
            shapeCache[0] = l;
            shapeCache[1] = size;

            val.SetShape(shapeCache);

   


            for (int i = 0; i < l; i++) {
                Tensor ten = t[i];
                for (int x = 0; x < size; x++) {
                    val.tensor[i, x] = ten[x];
                }
            }
        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new VariableStack1D(size);
        }
    }


}