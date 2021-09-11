using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {

    public class Add : Operation {


        public Add(Operation left, Operation right) : base(left.shape, left, right) { 

        }

        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(operands[0].Shape);
            var length = result.tensor.Size;

            unsafe {
                fixed (float* pd = result.tensor.value, pl = operands[0].value, pr = operands[1].value) {
                    for (int i = 0; i < length; i++) {
                        pd[i] = pl[i] + pr[i];
                    }
                }
            }
        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            result[0].Add(e, true);
            result[1].Add(e, true);
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
