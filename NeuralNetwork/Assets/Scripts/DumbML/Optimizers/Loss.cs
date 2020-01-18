

namespace DumbML {
    public abstract class Loss {
        public static Loss MSE = new _MSE();


        public abstract Operation Compute(Operation op, Placeholder labels);

        private class _MSE : Loss {
            public override Operation Compute(Operation op, Placeholder labels) {
                int size = 1;

                foreach (var i in op.shape) {
                    size *= i;
                }

                return new Sum(new Square(op - labels)) / size;
            }
        }
    }

    
}