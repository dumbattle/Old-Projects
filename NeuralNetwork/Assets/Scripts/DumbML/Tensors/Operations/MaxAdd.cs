using System.Collections.Generic;

namespace DumbML {
    public class MaxAdd : Operation {
        Tensor[] backwardsArray;
        int[] selected;

        public MaxAdd(params Operation[] ops) : base(ops[0].shape, ops) {
            int count = 1;
            foreach (var i in ops[0].shape) {
                count *= i;
            }
            selected = new int[count];
            backwardsArray = new Tensor[ops.Length];
            for (int i = 0; i < ops.Length; i++) {
                backwardsArray[i] = new Tensor(ops[i].shape);
            }
        }


        protected override Tensor Compute(Tensor[] operands) {
            var length = result.Size;

            for (int i = 0; i < length; i++) {
                int ind = 0;
                var val = operands[0]._value[i];
                for (int j = 1; j < operands.Length; j++) {
                    if (operands[j]._value[i] > val) {
                        ind = j;
                        val = operands[j]._value[i];
                    }
                }
                selected[i] = ind;
                result[i] = val;
            }
            return result;
        }


        protected override Tensor[] BackwardsPass(Tensor e) {
            var length = result.Size;

            for (int i = 0; i < length; i++) {
                for (int j = 0; j < backwardsArray.Length; j++) {
                    if (j == selected[i]) {
                        backwardsArray[j][i] = e[i];
                    }
                    else {
                        backwardsArray[j][i] = 0;
                    }
                }
            }

            return backwardsArray;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new MaxAdd(CopyInner(track));
        }
    }
}
