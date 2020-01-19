using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {

    public class Add : Operation {
        static ProfilerMarker profile = new ProfilerMarker("Add.Eval");
        Tensor[] backwardsArray = new Tensor[2];


        public Add(Operation left, Operation right) : base(left.shape, left, right) { }


        public override Tensor Compute(Tensor[] operands) {
            profile.Begin();
            var length = result.Size;

            unsafe {
                fixed (float* pd = result._value, pl = operands[0]._value, pr = operands[1]._value) {
                    for (int i = 0; i < length; i++) {
                        pd[i] = pl[i] + pr[i];
                    }
                }
            }
            profile.End();

            return result;
        }

        public override Tensor[] BackwardsPass(Tensor e) {
            backwardsArray[0] = e;
            backwardsArray[1] = e;
            return backwardsArray;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Add(inner[0]._Copy(track), inner[1]._Copy(track));
        }
        public override string ToString(bool requireParanthesis) {
            if (requireParanthesis) {
                return $"({inner[0].ToString(false)} + {inner[1].ToString(false)})";
            }
            return $"{inner[0].ToString(false)} + {inner[1].ToString(false)}";
        }
    }
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


        public override Tensor Compute(Tensor[] operands) {
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


        public override Tensor[] BackwardsPass(Tensor e) {
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
