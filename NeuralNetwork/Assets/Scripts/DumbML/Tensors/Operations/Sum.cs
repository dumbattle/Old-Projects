using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Sum : Operation {

        public Sum(Operation op) : base(new[] { 1 }, op) {
        }


        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(shape);


            float sum = 0;
            foreach (float f in operands[0].value) {
                sum += f;
            }

            result.tensor[0] = sum;
        }

        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            float er = e[0];

            for (int i = 0; i < result[0].value.Length; i++) {
                result[0].value[i] += er;
            }
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Sum(inner[0]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            return $"sum({inner[0].ExprString(false)})";
        }
    }

    
}