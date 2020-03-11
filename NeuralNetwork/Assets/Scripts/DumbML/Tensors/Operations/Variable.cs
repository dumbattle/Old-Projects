﻿using Unity.Profiling;
using System.Collections.Generic;

namespace DumbML {
    public class Variable : Operation {
        static ProfilerMarker profileBackwards = new ProfilerMarker("Variable.Backwards");
        public string name;
        public bool Trainable = true;

        public Tensor Value { get { return value; } set { base.value = value; } }

        public Variable(Tensor value) : base(value.Shape) {
            base.value = value;
        }
        public Variable(Tensor value, string name) : base(value.Shape){
            base.value = value;
            this.name = name;
        }

        protected override Tensor Compute(Tensor[] operands) {
          
            return value;
        }


        protected override Tensor[] BackwardsPass(Tensor e) {          
            return null;
        }

        
        public static implicit operator Variable(Tensor t) {
            return new Variable(t);
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Variable(Value.Copy());
        }
        public override string ToString(bool requireParanthesis) {
            return name ?? value.ToString();
        }
    }
    
}