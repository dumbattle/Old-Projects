using Unity.Profiling;

namespace DumbML {
    public class Square : Operation {
        static ProfilerMarker profile = new ProfilerMarker("Square.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("Square.Backwards");
        Tensor error;
        Tensor[] backwardsArray = new Tensor[1];

        public Square(Operation op) : base(op.shape, op) {
            error= new Tensor(shape);

        }
        public override Tensor Compute(Tensor[] operands) {
            profile.Begin();
            result.PointWise(operands[0], (_, b) => b * b, true);
            profile.End();
            return result; 
        }

        public override Tensor[] BackwardsPass(Tensor e) {
            profileBackwards.Begin();
            int s = error.Size;
            for (int i = 0; i < s; i++) {
                error._value[i] = 2 * e._value[i] * inner[0].result._value[i];
            }
            profileBackwards.End();

            backwardsArray[0] = error;
            return backwardsArray;
        }
        public override string ToString(bool requireParanthesis) {
            return $"({inner[0].ToString(false)})^2";
        }
    }

    
}