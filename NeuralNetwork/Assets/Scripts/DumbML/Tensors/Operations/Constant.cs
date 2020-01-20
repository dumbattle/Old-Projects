using System.Collections.Generic;

namespace DumbML {
    public class Constant : Operation {
        public Tensor Value { get { return result; } set { result = value; } }

        public Constant(Tensor value) : base(value.Shape) {
            result = value;
        }

        protected override Tensor Compute(Tensor[] operands) {
            return result;
        }

        protected override Tensor[] BackwardsPass(Tensor e) {
            return null;
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Constant(Value.Copy());
        }
        public static implicit operator Constant(float f) {
            return new Constant(new Tensor(() => f, 1));
        }
    }
    
}