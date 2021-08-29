using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Multiply : Operation {
        Tensor[] errarr = new Tensor[2];
        public Multiply(Operation left, Operation right) : base(left.shape, left, right) {
            errarr = new Tensor[2] { new Tensor(left.shape), new Tensor(right.shape) };
        }
        protected override Tensor _Compute(Tensor[] operands) {
            for (int i = 0; i < value.Size; i++) {
                value.value[i] = operands[0].value[i] * operands[1].value[i];
            }
            return value;
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            errarr[0].CopyFrom(e).PointWise(inner[1].value, (a, b) => a * b, true);
            errarr[1].CopyFrom(e).PointWise(inner[0].value, (a, b) => a * b, true);

            return errarr;

        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Multiply(inner[0]._Copy(track), inner[1]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            if (requireParanthesis) {
                return $"({inner[0].ExprString(true)} * {inner[1].ExprString(true)})";
            }
            return $"{inner[0].ExprString(true)} * {inner[1].ExprString(true)}";
        }
    }

    
}