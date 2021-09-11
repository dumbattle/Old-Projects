using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Variable : Operation {
        public bool Trainable = true;
        public Tensor Value;

        public Variable(Tensor value) : base(value.Shape) {
            _val.SetShape(value.Shape);
            _val.tensor.CopyFrom(value);
            Value = _val.tensor;
        }
        public Variable(Tensor value, string name) : this(value){
            Name = name;
        }


        protected override void _Compute(Tensor[] operands, TensorCache result) {
        }


        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
        }

        
        public static implicit operator Variable(Tensor t) {
            return new Variable(t);
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Variable(_val.tensor.Copy());
        }
        public override string ExprString(bool requireParanthesis) {
            return Name ?? value.ToString();
        }
    }
    
}