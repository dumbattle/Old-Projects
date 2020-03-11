using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class MatrixMult : Operation {
        static ProfilerMarker profile = new ProfilerMarker("MatrixMult.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("MatrixMult.Backwards");
        Tensor le, re;
        Tensor[] backwardsArray = new Tensor[2];

        public MatrixMult(Operation left, Operation right) : base(null, left,right) {
            le = new Tensor(left.shape);
            re = new Tensor(right.shape);

            if (left.shape.Length == 2 && right.shape.Length == 2) {
                shape = new[] { left.shape[0], right.shape[1] };

            }

            if (left.shape.Length == 1 && right.shape.Length == 2) {
                shape = new[] { right.shape[1] };
            }

            value = new Tensor(shape);
        }

        protected override Tensor Compute(Tensor[] operands) {
            profile.Begin();
            Blas.MatrixMult(operands[0], operands[1], value);
            profile.End();

            return value;
        }

        protected override Tensor[] BackwardsPass(Tensor e) {
            profileBackwards.Begin();

            Blas.MatrixMultBackwards(inner[0].value, inner[1].value, e, (le, re));

            profileBackwards.End();
            backwardsArray[0] = le;
            backwardsArray[1] = re;
            return backwardsArray;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new MatrixMult(inner[0]._Copy(track), inner[1]._Copy(track));
        }
        public override string ToString(bool requireParanthesis) {
            if (requireParanthesis) {
                return $"({inner[0].ToString(false)} x {inner[1].ToString(false)})";
            }
            return $"{inner[0].ToString(false)} x {inner[1].ToString(false)}";
        }
    }

    
}