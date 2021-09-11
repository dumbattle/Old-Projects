using System.Collections.Generic;
namespace DumbML {
    public class Placeholder : Operation {
        public Placeholder(string name, params int[] shape) : base(shape) {
            SetName(name);
        }

        public void SetVal(Tensor t) {
            _val.SetShape(t.Shape);
            _val.tensor.CopyFrom(t);
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {
        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Placeholder(Name, shape);
        }

        public new Placeholder SetName(string name) {
            base.SetName(name);
            return this;
        }
    }

    
}