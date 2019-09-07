using System;

namespace DumbML {
    public class SmoothRelu : ActivationFunction {
        public static ActivationFunction Default = new SmoothRelu();

        public override float Activate(float val) {
            return (float)Math.Log(1.0 + Math.Exp(val));
        }

        public override float Derivative(float val) {
            return 1f / (1f + (float)Math.Exp(-val));
        }
    }


}