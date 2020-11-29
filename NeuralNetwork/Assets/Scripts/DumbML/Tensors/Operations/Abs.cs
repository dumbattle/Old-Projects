using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Abs : Operation {
        static ProfilerMarker profile = new ProfilerMarker("Abs.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("Square.Backwards");
        Tensor error;


        public Abs(Operation op) : base(op.shape, op) {
            error= new Tensor(shape);
        }

        protected override Tensor _Compute(Tensor[] operands) {
            profile.Begin();
            value.PointWise(operands[0], (a, b) => b >= 0 ? b : -b, true);
            profile.End();
            return value; 
        }
        protected override Tensor[] _BackwardsPass(Tensor e) {
            profileBackwards.Begin();
            int s = error.Size;
            for (int i = 0; i < s; i++) {
                error.value[i] = inner[0].value.value[i] >= 0 ? e.value[i] : -e.value[i];
            }
            profileBackwards.End();
            return new[] { error};
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Abs(inner[0]._Copy(track));
        }


        public override string ExprString(bool requireParanthesis) {
            return $"|{inner[0].ExprString(false)}|";
        }
    }

    
}