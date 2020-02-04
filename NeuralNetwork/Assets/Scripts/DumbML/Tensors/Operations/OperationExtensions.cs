namespace DumbML {
    public static class OperationExtensions {
        public static Operation Softmax(this Operation op) {
            var e = new Exp(op);
            var sum = new BroadcastScalar(new Sum(e), e.shape);

            return e / sum;           

        }
    }
}