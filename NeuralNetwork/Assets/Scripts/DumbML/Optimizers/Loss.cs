

namespace DumbML {
    public abstract class Loss {
        public static Loss MSE = new _MSE();
        public static Loss CrossEntropy = new _XEntropy();


        public abstract Operation Compute(Operation op, Placeholder labels);

        private class _MSE : Loss {
            public override Operation Compute(Operation op, Placeholder labels) {
                int size = 2;

                foreach (var i in op.shape) {
                    size *= i;
                }

                return new Sum(new Square(op - labels)) / 2;
            }
        }
        private class _XEntropy : Loss {
            public override Operation Compute(Operation op, Placeholder labels) {
                return new Log(op) * labels * new BroadcastScalar(-1, op.shape);
            }
        }
    }

    
}