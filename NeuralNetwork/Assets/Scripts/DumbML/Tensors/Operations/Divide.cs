using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Divide : Operation {
        static ProfilerMarker profile = new ProfilerMarker("Divide.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("Divide.Backwards");
        Tensor ne, de;

        public Divide(Operation numerator, Operation denominator) : base(numerator.shape, numerator, denominator) {
            ne = new Tensor(numerator.shape);
            de = new Tensor(denominator.shape);
        }
        protected override Tensor Compute(Tensor[] operands) {
            profile.Begin();
            for (int i = 0; i < value.Size; i++) {
                value._value[i] = operands[0]._value[i] / operands[1]._value[i];
            }
            profile.End();
            return value;
        }

        protected override Tensor[] BackwardsPass(Tensor e) {
            profileBackwards.Begin();
            ne.Copy(e).PointWise(inner[1].value, (a, b) => a / b, true);
            de.Copy(e).PointWise(inner[0].value, (a, b) => -a * b, true).PointWise(inner[1].value, (ea, b) => ea / (b * b), true);
            profileBackwards.End();

            return new[] { ne,de };
        }
        public override string ToString(bool requireParanthesis) {
            if (requireParanthesis) {
                return $"({inner[0].ToString(true)} / {inner[1].ToString(true)})";
            }
            return $"{inner[0].ToString(true)} / {inner[1].ToString(true)}";
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Divide(inner[0]._Copy(track), inner[1]._Copy(track));
        }
    }

    
}