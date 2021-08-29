using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Sum : Operation {
        Tensor error;
        Tensor[] errarr = new Tensor[1];
        public Sum(Operation op) : base(new[] { 1 }, op) {
            error = new Tensor(op.shape);
        }

        protected override Tensor _Compute(Tensor[] operands) {

            float sum = 0;
            foreach (float f in operands[0].value) {
                sum += f;
            }
            value[0] = sum;

            return value;
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            float er = e[0];

            for (int i = 0; i < error.value.Length; i++) {
                error.value[i] = er;
            }
    
            errarr[0] = error;
            return errarr;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Sum(inner[0]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            return $"sum({inner[0].ExprString(false)})";
        }
    }

    
}