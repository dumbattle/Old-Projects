using System.Collections.Generic;

namespace DumbML {
    public class Max : Operation {
        Tensor[] backwardsArray;
        int[] selected;

        public Max(params Operation[] ops) : base(ops[0].shape, ops) {
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


        protected override Tensor _Compute(Tensor[] operands) {
            var length = value.Size;

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
                value[i] = val;
            }


            return value;
        }


        protected override Tensor[] _BackwardsPass(Tensor e) {
            var length = value.Size;

            //for (int i = 0; i < length; i++) {
            //    var s = selected[i];
            //    var err = e[i];

            //    for (int j = 0; j < backwardsArray.Length; j++) {
            //        if (j == s) {
            //            backwardsArray[j][i] = err;
            //        }
            //        else {
            //            backwardsArray[j][i] = 0;
            //        }
            //    }
            //}

            for (int j = 0; j < backwardsArray.Length; j++) {
                var ba = backwardsArray[j];
                for (int i = 0; i < length; i++) {
                    if (j == selected[i]) {
                        ba.value[i] = e[i];
                    }
                    else {
                        ba.value[i] = 0;
                    }
                }
            }

            return backwardsArray;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Max(CopyInner(track));
        }
    }
}
