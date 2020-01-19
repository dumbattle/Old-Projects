using System.Collections.Generic;
namespace DumbML {
    public class Placeholder : Operation {
        public Placeholder(params int[] shape) : base(shape) { }

        public void SetVal(Tensor t) {
            result = t;
        }
        public override Tensor Compute(Tensor[] operands) {
            return result;
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Placeholder(shape);
        }
        public override Tensor[] BackwardsPass(Tensor e) {
            return null;
        }

    }

    
}