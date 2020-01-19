using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Variable : Operation {
        static ProfilerMarker profileBackwards = new ProfilerMarker("Variable.Backwards");
        public string name;
        public bool Trainable = true;

        public Tensor Value { get { return result; } set { result = value; } }

        public Variable(Tensor value) : base(value.Shape) {
            result = value;
        }
        public Variable(Tensor value, string name) : base(value.Shape){
            result = value;
            this.name = name;
        }

        public override Tensor Compute(Tensor[] operands) {
          
            return result;
        }


        public override Tensor[] BackwardsPass(Tensor e) {          
            return null;
        }

        
        public static implicit operator Variable(Tensor t) {
            return new Variable(t);
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Variable(Value.Copy());
        }
        public override string ToString(bool requireParanthesis) {
            return name ?? result.ToString();
        }
    }
    public class Constant : Operation {
        public Tensor Value { get { return result; } set { result = value; } }

        public Constant(Tensor value) : base(value.Shape) {
            result = value;
        }

        public override Tensor Compute(Tensor[] operands) {
            return result;
        }

        public override Tensor[] BackwardsPass(Tensor e) {
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