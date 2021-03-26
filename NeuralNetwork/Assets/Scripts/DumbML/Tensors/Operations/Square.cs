using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Square : Operation {
        static ProfilerMarker profile = new ProfilerMarker("Square.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("Square.Backwards");
        Tensor error;
        Tensor[] backwardsArray = new Tensor[1];

        public Square(Operation op) : base(op.shape, op) {
            error= new Tensor(shape);

        }
        protected override Tensor _Compute(Tensor[] operands) {
            profile.Begin();
            value.PointWise(operands[0], (_, b) => b * b, true);
            profile.End();
            return value; 
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            profileBackwards.Begin();
            int s = error.Size;
            for (int i = 0; i < s; i++) {
                error.value[i] = 2 * e.value[i] * inner[0].value.value[i];
            }
            profileBackwards.End();

            backwardsArray[0] = error;
            return backwardsArray;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Square(inner[0]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            return $"({inner[0].ExprString(false)})^2";
        }
    }    
}