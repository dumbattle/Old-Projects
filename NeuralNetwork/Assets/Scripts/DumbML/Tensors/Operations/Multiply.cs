using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Multiply : Operation {
        TensorCache le = new TensorCache();
        TensorCache re = new TensorCache();


        public Multiply(Operation left, Operation right) : base(left.shape, left, right) {
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(operands[0].Shape);

            for (int i = 0; i < result.tensor.Size; i++) {
                result.tensor.value[i] = operands[0].value[i] * operands[1].value[i];
            }
        }

        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            le.SetShape(e.Shape);
            re.SetShape(e.Shape);
            le.tensor.CopyFrom(e).PointWise(inner[1].value, (a, b) => a * b, true);
            re.tensor.CopyFrom(e).PointWise(inner[0].value, (a, b) => a * b, true);
            result[0].Add(le.tensor, true);
            result[1].Add(re.tensor, true);

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


        class Cache {
            public Tensor o;
            public Tensor el;
            public Tensor er;
        }
    }

    
}