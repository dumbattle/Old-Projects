using Unity.Profiling;

using System.Collections.Generic;
namespace DumbML {
    public class LeakyRelu : Operation {
        static ProfilerMarker profile = new ProfilerMarker("LeakyRelu.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("LeakyRelu.Backwards");
        Tensor error;
        float leakyness = .001f;
        public LeakyRelu(Operation op) : base(op.shape, op) {
            error = new Tensor(shape);
        }

        protected override Tensor _Compute(Tensor[] operands) {
            profile.Begin();
            value.PointWise(operands[0], (a, b) => (b > 0 ? b : b * leakyness), true);
            profile.End();
            return value; 
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            profileBackwards.Begin();
            int s = error.Size;
            for (int i = 0; i < s; i++) {
                error.value[i] = e.value[i] * (value.value[i] > 0 ? 1 :  leakyness);
            }
            profileBackwards.End();
            return new[] { error };
        }
        public override string ExprString(bool requireParanthesis) {
            return $"LeakyRelu({inner[0].ExprString(false)})";
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new LeakyRelu(inner[0]._Copy(track));
        }
    }

    
}