using System.Collections.Generic;

namespace DumbML {
    public class Max : Operation {
        int[] selected;

        public Max(params Operation[] ops) : base(ops[0].shape, ops) {
            int count = 1;
            foreach (var i in ops[0].shape) {
                count *= i;
            }
            selected = new int[count];
        }

        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(operands[0].Shape);
            var length = result.tensor.Size;

            for (int i = 0; i < length; i++) {
                int ind = 0;
                var val = operands[0].value[i];

                for (int j = 1; j < operands.Length; j++) {
                    if (operands[j].value[i] > val) {
                        ind = j;
                        val = operands[j].value[i];
                    }
                }
                selected[i] = ind;
                result.tensor[i] = val;
            }
        }


        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            var length = e.Size;

      

            for (int j = 0; j < result.Length; j++) {
                var ba = result[j];
                for (int i = 0; i < length; i++) {
                    if (j == selected[i]) {
                        ba.value[i] += e[i];
                    }
                }
            }
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Max(CopyInner(track));
        }
    }
}
