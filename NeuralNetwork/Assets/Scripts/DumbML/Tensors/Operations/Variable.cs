using Unity.Profiling;

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

        public static implicit operator Variable(float f) {
            return new Variable(new Tensor(() => f, 1)) {
                Trainable = false
            };
        }
        public static implicit operator Variable(Tensor t) {
            return new Variable(t);
        }
        public override string ToString(bool requireParanthesis) {
            return name ?? result.ToString();
        }
    }

    
}