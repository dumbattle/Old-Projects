using System.Collections.Generic;
namespace DumbML {
    public class Placeholder : Operation {
        public Placeholder(string name, params int[] shape) : base(shape) {
            SetName(name);
        }

        public void SetVal(Tensor t) {
            value.Copy(t);
        }
        protected override Tensor _Compute(Tensor[] operands) {
            return value;
        }
        protected override Tensor[] _BackwardsPass(Tensor e) {
            return null;
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