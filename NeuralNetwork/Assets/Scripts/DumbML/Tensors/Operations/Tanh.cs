﻿using Unity.Profiling;

using System.Collections.Generic;
namespace DumbML {
    public class Tanh  : Operation {

        static ProfilerMarker profile = new ProfilerMarker("Tanh.Eval");
        static ProfilerMarker profileBackwards = new ProfilerMarker("Tanh.Backwards");
        Tensor error;
        Tensor[] backwardsArray = new Tensor[1];

        public Tanh(Operation op) : base(op.shape, op) {
            error = new Tensor(shape);
        }

        protected override Tensor _Compute(Tensor[] operands) {
            profile.Begin();
            value = operands[0].Tanh(value);
            profile.End();
            return value;
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            profileBackwards.Begin();
            int s = error.Size;
            for (int i = 0; i < s; i++) {
                error.value[i] = e.value[i] * (value.value[i] + 1) * (.5f - value.value[i] / 2);
            }
            profileBackwards.End();

            backwardsArray[0] = error;
            return backwardsArray;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Tanh(inner[0]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            return $"Sigmoid({inner[0].ExprString(false)})";
        }
    }
    
}