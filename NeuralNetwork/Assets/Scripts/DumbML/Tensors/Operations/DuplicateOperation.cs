namespace DumbML {
    /// <summary>
    /// Wraps an operation that appears multiple times (except for the first instance) in a graph to prevent it from being calculated multiple times
    /// </summary>
    public class DuplicateOperation : Operation {
        public Operation i;
        public DuplicateOperation(Operation inner) : base(inner.shape) {
            i = inner;
            i.dupeError = new Tensor(i.shape);
        }

        public override Tensor Compute(Tensor[] operands) {
            return result = i.result;
        }

        public override Tensor[] BackwardsPass(Tensor e) {
            i.dupeError.Add(e, true);
            return new[] { e };
        }

        public override string ToString(bool requireParanthesis) {
            return "x";
        }
    }

}