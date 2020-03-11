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