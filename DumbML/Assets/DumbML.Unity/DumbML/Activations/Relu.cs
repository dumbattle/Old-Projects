namespace DumbML {
    public class Relu : ActivationFunction {
        public static ActivationFunction Default = new Relu();
        public override float Activate(float val) {
            return val > 0 ? val : 0;
        }

        public override float Derivative(float val) {
            return val > 0 ? 1 : 0;
        }
    }
}