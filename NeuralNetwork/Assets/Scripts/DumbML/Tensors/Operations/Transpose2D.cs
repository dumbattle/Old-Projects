using System.Collections.Generic;

namespace DumbML {
    public class Transpose2D : Operation {
        Tensor[] error;
        int x;
        int y;
        public Transpose2D(Operation op) : base(null, op) {
            if (op.shape.Length != 2) {
                throw new System.InvalidOperationException($"Must be rank 2. Got: {op.shape.Length}");
            }
            x = op.shape[1];
            y = op.shape[0];
            shape = new int[] { x, y };
            value = new Tensor(shape);
            error = new[] { new Tensor(op.shape) };
        }

        protected override Tensor _Compute(Tensor[] operands) {
            Tensor input = operands[0];

            for (int x = 0; x < this.x; x++) {
                for (int y = 0; y < this.y; y++) {
                    value[x, y] = input[y, x];
                }
            }
            return value;
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            for (int x = 0; x < this.x; x++) {
                for (int y = 0; y < this.y; y++) {
                    error[0][y, x] = e[x, y];
                }
            }

            return error;
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Transpose2D(CopyInner(track)[0]);
        }
    }

    
}