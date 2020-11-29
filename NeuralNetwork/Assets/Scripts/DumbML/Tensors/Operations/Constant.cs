using System.Collections.Generic;

namespace DumbML {
    public class Constant : Operation {
        public Tensor Value { get { return value; } }

        public Constant(Tensor value) : base(value.Shape) {
            base.value = value;
        }

        protected override Tensor _Compute(Tensor[] operands) {
            return value;
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
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