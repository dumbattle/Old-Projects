using FullSerializer;

namespace DumbML {
    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class DoubleLeakyRelu : ActivationFunction {
        public static ActivationFunction Default = new DoubleLeakyRelu(.01f);
        [fsProperty]
        public float leakyness;

        public DoubleLeakyRelu(float leakyness) {
            this.leakyness = leakyness;
        }


        public override float Activate(float val) {
            if (val > 1) {
                return 1 + (val - 1) * leakyness;
            }
            return val > 0 ? val : val * leakyness;
        }

        public override float Derivative(float val) {
            if (val > 1) {
                return leakyness;
            }
            return val > 0 ? 1 : leakyness;
        }
    }
}