namespace DumbML {
    public abstract class ActivationFunction {
        public static ActivationFunction None = new NONE();
        public static ActivationFunction DoubleLeakyRelu = DumbML.DoubleLeakyRelu.Default;
        public static ActivationFunction LeakyRelu = DumbML.LeakyRelu.Default;
        public static ActivationFunction Relu = DumbML.Relu.Default;
        public static ActivationFunction SmoothRelu = DumbML.SmoothRelu.Default;

        public static ActivationFunction Sigmoid = DumbML.Sigmoid.Default;

        public abstract float Activate(float val);
        public abstract float Derivative(float val);

        private class NONE : ActivationFunction {
            public override float Activate(float val) {
                return val;
            }

            public override float Derivative(float val) {
                return 1;
            }
        }
    }
}