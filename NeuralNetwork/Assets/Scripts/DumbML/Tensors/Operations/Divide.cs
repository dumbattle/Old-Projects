using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {


    public class Divide : Operation {
        Tensor ne, de;
        public Divide(Operation numerator, Operation denominator) : base(numerator.shape, numerator, denominator) {
            ne = new Tensor(numerator.shape);
            de = new Tensor(denominator.shape);
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(operands[0].Shape);
            for (int i = 0; i < operands[0].Size; i++) {
                result.tensor.value[i] = operands[0].value[i] / operands[1].value[i];
            }
        }
        
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            ne.CopyFrom(e).PointWise(inner[1].value, (a, b) => a / b, true);
            de.CopyFrom(e).PointWise(inner[0].value, (a, b) => -a * b, true).PointWise(inner[1].value, (ea, b) => ea / (b * b), true);
            result[0].Add(ne, true);
            result[1].Add(de, true);
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