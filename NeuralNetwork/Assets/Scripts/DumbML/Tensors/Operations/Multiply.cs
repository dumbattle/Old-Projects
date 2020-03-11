using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Multiply : Operation {
        static ProfilerMarker profile = new ProfilerMarker("Multiply.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("Multiply.Backwards");
        Tensor le, re;

        public Multiply(Operation left, Operation right) : base(left.shape, left, right) {
            le = new Tensor(left.shape);
            re = new Tensor(right.shape);
        }
        protected override Tensor Compute(Tensor[] operands) {
            profile.Begin();
            for (int i = 0; i < value.Size; i++) {
                value._value[i] = operands[0]._value[i] * operands[1]._value[i];
            }
            profile.End();
            return value;
        }

        protected override Tensor[] BackwardsPass(Tensor e) {
            profileBackwards.Begin();
            le.Copy(e).PointWise(inner[1].value, (a, b) => a * b, true);
            re.Copy(e).PointWise(inner[0].value, (a, b) => a * b, true);
            profileBackwards.End();

            return new[] { le,re};

        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Multiply(inner[0]._Copy(track), inner[1]._Copy(track));
        }
        public override string ToString(bool requireParanthesis) {
            if (requireParanthesis) {
                return $"({inner[0].ToString(true)} * {inner[1].ToString(true)})";
            }
            return $"{inner[0].ToString(true)} * {inner[1].ToString(true)}";
        }
    }

    
}