﻿using Unity.Profiling;

namespace DumbML {
    public class Relu : Operation {
        static ProfilerMarker profile = new ProfilerMarker("Relu.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("Relu.Backwards");
        Tensor error;

        public Relu(Operation op) : base(op.shape, op) {
            error = new Tensor(shape);
        }

        public override Tensor Compute(Tensor[] operands) {
            profile.Begin();
            result.PointWise(operands[0], (a, b) => (b > 0 ? b : 0), true);
            profile.End();
            return result; 
        }

        public override Tensor[] BackwardsPass(Tensor e) {
            profileBackwards.Begin();
            int s = error.Size;
            for (int i = 0; i < s; i++) {
                error._value[i] = e._value[i] * (result._value[i] > 0 ? 1 : 0);
            }
            profileBackwards.End();
            return new[] { error };
        }
        public override string ToString(bool requireParanthesis) {
            return $"Relu({inner[0].ToString(false)})";
        }
    }

    public class Step : Operation {
        public Step(Operation op) : base(op.shape, op) {}


        public override Tensor Compute(Tensor[] operands) {
            result.PointWise(operands[0], (a, b) => (b > 0 ? 1 : 0), true);
            return result;
        }
        public override Tensor[] BackwardsPass(Tensor e) {
            return new[] { e };
        }

    }
}