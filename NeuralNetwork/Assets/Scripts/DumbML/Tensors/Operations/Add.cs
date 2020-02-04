using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {

    public class Add : Operation {
        static ProfilerMarker profile = new ProfilerMarker("Add.Eval");
        Tensor[] backwardsArray = new Tensor[2];


        public Add(Operation left, Operation right) : base(left.shape, left, right) { }


        protected override Tensor Compute(Tensor[] operands) {
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

        protected override Tensor[] BackwardsPass(Tensor e) {
            backwardsArray[0] = e.Copy();
            backwardsArray[1] = e.Copy();
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
}
