namespace DumbML {
    public class LeakyRelu : ActivationFunction {
        public static readonly ActivationFunction Default = new LeakyRelu(.001f);
        public float leakyness;

        public LeakyRelu(float leakyness) {
            this.leakyness = leakyness;
        }


        public override float Activate(float val) {
            return val > 0 ? val : val * leakyness;
        }

        public override float Derivative(float val) {
            return val > 0 ? 1 : leakyness;
        }
    }
}