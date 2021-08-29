using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {


    public class Divide : Operation {
        Tensor ne, de;
        Tensor[] err = new Tensor[2];
        public Divide(Operation numerator, Operation denominator) : base(numerator.shape, numerator, denominator) {
            ne = new Tensor(numerator.shape);
            de = new Tensor(denominator.shape);
        }
        protected override Tensor _Compute(Tensor[] operands) {
            for (int i = 0; i < value.Size; i++) {
                value.value[i] = operands[0].value[i] / operands[1].value[i];
            }
            return value;
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            ne.CopyFrom(e).PointWise(inner[1].value, (a, b) => a / b, true);
            de.CopyFrom(e).PointWise(inner[0].value, (a, b) => -a * b, true).PointWise(inner[1].value, (ea, b) => ea / (b * b), true);
            err[0] = ne;
            err[1] = de;
            return err;
        }
        public override string ExprString(bool requireParanthesis) {
            if (requireParanthesis) {
                return $"({inner[0].ExprString(true)} / {inner[1].ExprString(true)})";
            }
            return $"{inner[0].ExprString(true)} / {inner[1].ExprString(true)}";
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Divide(inner[0]._Copy(track), inner[1]._Copy(track));
        }
    }

    
}