using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Sum : Operation {
        static ProfilerMarker profile = new ProfilerMarker("Sum.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("Sum.Backwards");
        Tensor error;

        public Sum(Operation op) : base(new[] { 1 }, op) {
            error = new Tensor(op.shape);
        }

        protected override Tensor _Compute(Tensor[] operands) {
            profile.Begin();

            float sum = 0;
            foreach (float f in operands[0]) {
                sum += f;
            }
            value[0] = sum;

            profile.End();
            return value;
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            profileBackwards.Begin();
            float er = e[0];
            error.PointWise((a) => er, true);
            profileBackwards.End();

            return new[] { error };
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Sum(inner[0]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            return $"sum({inner[0].ExprString(false)})";
        }
    }

    
}