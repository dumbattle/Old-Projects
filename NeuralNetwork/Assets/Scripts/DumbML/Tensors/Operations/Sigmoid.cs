using Unity.Profiling;

using System.Collections.Generic;
namespace DumbML {
    public class Sigmoid : Operation {
        static ProfilerMarker profile = new ProfilerMarker("Sigmoid.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("Sigmoid.Backwards");
        Tensor error;
        Tensor[] backwardsArray = new Tensor[1];

        public Sigmoid(Operation op) : base(op.shape, op) {
            error = new Tensor(shape);
        }

        public override Tensor Compute(Tensor[] operands) {
            profile.Begin();
            operands[0].Sigmoid(result);
            profile.End();

      

            return result;
        }

        public override Tensor[] BackwardsPass(Tensor e) {
            profileBackwards.Begin();
            int s = error.Size;
            for (int i = 0; i < s; i++) {
                error._value[i] = e._value[i] * result._value[i] * (1 - result._value[i]);
            }
            profileBackwards.End();
            backwardsArray[0] = error;
            return backwardsArray;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Sigmoid(inner[0]._Copy(track));
        }
        public override string ToString(bool requireParanthesis) {
            return $"Sigmoid({inner[0].ToString(false)})";
        }
    }
    public class Tanh  : Operation {

        static ProfilerMarker profile = new ProfilerMarker("Tanh.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("Tanh.Backwards");
        Tensor error;
        Tensor[] backwardsArray = new Tensor[1];

        public Tanh(Operation op) : base(op.shape, op) {
            error = new Tensor(shape);
        }

        public override Tensor Compute(Tensor[] operands) {
            profile.Begin();
            result = operands[0].Tanh(result);
            profile.End();
            return result;
        }

        public override Tensor[] BackwardsPass(Tensor e) {
            profileBackwards.Begin();
            int s = error.Size;
            for (int i = 0; i < s; i++) {
                error._value[i] = e._value[i] * (result._value[i] + 1) * (.5f - result._value[i] / 2);
            }
            profileBackwards.End();

            backwardsArray[0] = error;
            return backwardsArray;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Tanh(inner[0]._Copy(track));
        }
        public override string ToString(bool requireParanthesis) {
            return $"Sigmoid({inner[0].ToString(false)})";
        }
    }
    
}