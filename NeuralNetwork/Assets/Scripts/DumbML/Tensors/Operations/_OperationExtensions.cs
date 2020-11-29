namespace DumbML {
    public static class OperationExtensions {
        static System.Random rng = new System.Random();
        public static Operation Softmax(this Operation op) {
            var e = new Exp(op);
            var sum = new BroadcastScalar(new Sum(e), e.shape);

            return e / sum;           

        }

        public static Operation Detach (this Operation op) {
            return new Detached(op);
        }

        public static Operation L2loss(this Operation op, float a = .0001f) {
            Operation result = null;

            foreach (var o in op.GetVariables()) {
                UnityEngine.Debug.Log(o.Name);
                result = result == null ? new Sum(new Square(o)) : result + new Sum(new Square(o));
            }

            return result * a;
        }

        public static int Sample(this Tensor t) {
            float dart = (float)rng.NextDouble();

            for (int i = 0; i < t.Shape[0]; i++) {
                dart -= t[i];

                if (dart <= 0) {
                    return i;
                }
            }
            return 0;
        }
    }
}