﻿using Unity.Profiling;

namespace DumbML {
    public class Abs : Operation {
        static ProfilerMarker profile = new ProfilerMarker("Abs.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("Square.Backwards");
        Tensor error;

        public Abs(Operation op) : base(op.shape, op) {
            error= new Tensor(shape);

        }
        public override Tensor Compute(Tensor[] operands) {
            profile.Begin();
            result.PointWise(operands[0], (a, b) => b >= 0 ? b : -b, true);
            profile.End();
            return result; 
        }

        public override Tensor[] BackwardsPass(Tensor e) {
            profileBackwards.Begin();
            int s = error.Size;
            for (int i = 0; i < s; i++) {
                error._value[i] = inner[0].result._value[i] >= 0 ? e._value[i] : -e._value[i];
            }
            profileBackwards.End();
            return new[] { error};
        }
        public override string ToString(bool requireParanthesis) {
            return $"|{inner[0].ToString(false)}|";
        }
    }

    
}