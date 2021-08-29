using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Subtract : Operation {
        Tensor rError;
        Tensor[] backwardsArray = new Tensor[2];

        public Subtract(Operation left, Operation right) : base(left.shape, left,right) {
            rError = new Tensor(right.shape);
        }
        protected override Tensor _Compute(Tensor[] operands) {
            for (int i = 0; i < value.Size; i++) {
                value.value[i] = operands[0].value[i] - operands[1].value[i];
            }

            return value;
        }
        protected override Tensor[] _BackwardsPass( Tensor e) {
            rError.PointWise(e, (_, b) => -b, true);

            backwardsArray[0] = e;
            backwardsArray[1] = rError;


            return backwardsArray;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Subtract(inner[0]._Copy(track),inner[1]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            if (requireParanthesis) {
                return $"({inner[0].ExprString(false)} - {inner[1].ExprString(true)})";
            }
            return $"{inner[0].ExprString(false)} - {inner[1].ExprString(true)}";
        }
    }

    
}