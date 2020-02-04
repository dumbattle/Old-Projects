using System.Collections.Generic;
namespace DumbML {
    public class Placeholder : Operation {
        public Placeholder(params int[] shape) : base(shape) { }

        public void SetVal(Tensor t) {
            result.Copy(t);
        }
        protected override Tensor Compute(Tensor[] operands) {
            return result;
        }
        protected override Tensor[] BackwardsPass(Tensor e) {
            return null;
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Placeholder(shape);
        }

    }

    
}