using System;

namespace DumbML {
    public class Sigmoid : ActivationFunction {
        public static ActivationFunction Default = new Sigmoid();


        public override float Activate(float val) {
            return 1f / (1f + (float)Math.Exp(-val));
        }

        public override float Derivative(float val) {
            //float v = Activate(val);

            return val * (1 - val);
        }
    }
}