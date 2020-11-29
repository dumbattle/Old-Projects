using System.Collections.Generic;

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

        protected override Tensor _Compute(Tensor[] operands) {
            return value = i.value;
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            i.dupeError.Add(e, true);
            return new[] { e };
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            if (track.ContainsKey(i)) {
                return new DuplicateOperation(track[i]);
            }

            return i._Copy(track);
        }
        public override string ExprString(bool requireParanthesis) {
            return "x";
        }
    }

}