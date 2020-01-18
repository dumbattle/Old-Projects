
namespace DumbML {
    public abstract class ActivationFunction {
        public static ActivationFunction None = new _None();
        public static ActivationFunction Sigmoid = new _Sigmoid();
        public static ActivationFunction Tanh = new _Tanh();
        public static ActivationFunction Relu = new _Relu();
        public static ActivationFunction LeakyRelu = new _LeakyRelu();
        public static ActivationFunction Step = new _Step();


        public abstract Operation Activate(Operation op);

        private class _None : ActivationFunction {
            public override Operation Activate(Operation op) {
                return op;
            }
        }
        private class _Sigmoid : ActivationFunction {
            public override Operation Activate(Operation op) {
                return new Sigmoid(op);
            }
        }
        private class _Tanh : ActivationFunction {
            public override Operation Activate(Operation op) {
                return new Tanh(op);
            }
        }
        private class _LeakyRelu : ActivationFunction {
            public override Operation Activate(Operation op) {
                return new LeakyRelu(op);
            }
        }
        private class _Relu : ActivationFunction {
            public override Operation Activate(Operation op) {
                return new Relu(op);
            }
        }
        private class _Step : ActivationFunction {
            public override Operation Activate(Operation op) {
                return new Step(op);
            }
        }
    }
}