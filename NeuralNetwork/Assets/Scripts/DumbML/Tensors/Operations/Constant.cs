using System.Collections.Generic;

namespace DumbML {
    public class Constant : Operation {
        Tensor val;
        public Tensor Value { get { return value; } }

        public Constant(Tensor value) : base(value.Shape) {
            val = value;
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(val.Shape);
            result.tensor.CopyFrom(val);
        }

        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Constant(Value.Copy());
        }
        public static implicit operator Constant(float f) {
            return new Constant(new Tensor(() => f, 1));
        }
    }
    
}