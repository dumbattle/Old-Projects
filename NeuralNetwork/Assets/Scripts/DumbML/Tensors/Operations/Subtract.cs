using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Subtract : Operation {
        static ProfilerMarker profile = new ProfilerMarker("Subtract.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("Subtract.Backwards");
        Tensor rError;
        Tensor[] backwardsArray = new Tensor[2];

        public Subtract(Operation left, Operation right) : base(left.shape, left,right) {
            rError = new Tensor(right.shape);
        }
        public override Tensor Compute(Tensor[] operands) {
            profile.Begin();
            for (int i = 0; i < result.Size; i++) {
                result._value[i] = operands[0]._value[i] - operands[1]._value[i];
            }

            profile.End();
            return result;
        }
        public override Tensor[] BackwardsPass( Tensor e) {
            profileBackwards.Begin();
            rError.PointWise(e, (_, b) => -b, true);
            profileBackwards.End();

            backwardsArray[0] = e;
            backwardsArray[1] = rError;


            return backwardsArray;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Subtract(inner[0]._Copy(track),inner[1]._Copy(track));
        }
        public override string ToString(bool requireParanthesis) {
            if (requireParanthesis) {
                return $"({inner[0].ToString(false)} - {inner[1].ToString(true)})";
            }
            return $"{inner[0].ToString(false)} - {inner[1].ToString(true)}";
        }
    }

    
}