using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {

    public class Add : Operation {
        static ProfilerMarker profile = new ProfilerMarker("Add.Eval");
        Tensor[] backwardsArray = new Tensor[2];


        public Add(Operation left, Operation right) : base(left.shape, left, right) { }


        protected override Tensor _Compute(Tensor[] operands) {
            profile.Begin();
            var length = value.Size;

            unsafe {
                fixed (float* pd = value.value, pl = operands[0].value, pr = operands[1].value) {
                    for (int i = 0; i < length; i++) {
                        pd[i] = pl[i] + pr[i];
                    }
                }
            }
            profile.End();

            return value;
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            backwardsArray[0] = e.Copy();
            backwardsArray[1] = e.Copy();
            return backwardsArray;
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Add(inner[0]._Copy(track), inner[1]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            if (requireParanthesis) {
                return $"({inner[0].ExprString(false)} + {inner[1].ExprString(false)})";
            }
            return $"{inner[0].ExprString(false)} + {inner[1].ExprString(false)}";
        }
    }
}
