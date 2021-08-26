using System.Collections.Generic;
using System;
using System.Linq;


namespace DumbML {
    public abstract class Operation {
        public const float EPSILON = 1e-12f;
        public Tensor value { get; protected set; }
        public int[] shape;
        public Operation[] inner;
        public string Name;

        public Tensor[] operands;
        public Tensor dupeError;

        public Operation(int[] shape, params Operation[] inner) {
            if (shape != null) {
                value = new Tensor(shape);
                this.shape = (int[])shape.Clone();
            }
            this.inner = (Operation[])inner.Clone();
            operands = new Tensor[inner.Length];

            
            Name = GetType().ToString();
        }

        public void Optimize() {
            RemoveDupes();
        }
        void RemoveDupes(List<Operation> ops = null) {
            if (ops == null) {
                ops = new List<Operation>() { };
            }

            ops.Add(this);

            for (int i = 0; i < inner.Length; i++) {
                Operation op = inner[i];

                if (ops.Contains(op)) {
                    inner[i] = new DuplicateOperation(op);
                }
                else {
                    op.RemoveDupes(ops);
                }
            }
        }

        public Tensor Eval() {
            for (int i = 0; i < inner.Length; i++) {
                operands[i] = inner[i].Eval();
            }

            return value = _Compute(operands);
        }
        //public Tensor Compute(Tensor[] operands) {
        //    return _Compute(operands);
        //}

        protected abstract Tensor _Compute(Tensor[] operands);

        public void Backwards(Gradients g, Tensor e) {
            if (g.Contains(this)) {
                g[this].Add(e, true);
            }
            Tensor[] inputErrors = _BackwardsPass(e);

            for (int i = inner.Length - 1; i >= 0; i--) {
                if (inner[i].dupeError != null) {
                    inputErrors[i].Add(inner[i].dupeError, true);
                    inner[i].dupeError.SetValuesToZero();
                }

                inner[i].Backwards(g, inputErrors[i]);
            }
        }
        public void Backwards(Gradients g) {
            Tensor e = new Tensor(() => 1, shape);
            Backwards(g, e);
        }
        public void Backwards(Optimizer o) {
            Tensor e = new Tensor(() => 1, shape);
            Backwards(o.grad, e);
        }


        public void Backwards(Optimizer o,Tensor e) {
            Backwards(o.grad, e);
        }



       public  Tensor[] BackwardsPass(Tensor e) {
            return _BackwardsPass(e);
        }
        protected abstract Tensor[] _BackwardsPass(Tensor e);



        public List<Operation> GetOperations (List<Operation> results = null, Func<Operation, bool> condition = null) {
            if (results == null) {
                results = new List<Operation>();
            }

            if (condition == null || condition(this)) {
                results.Add(this);
            }
            foreach (var op in inner) {
                op.GetOperations(results, condition);
            }
            return results;
        }

        public List <T> GetOperations<T>(List<T> results = null, Func<T, bool> condition = null) where T : Operation {
            if (results == null) {
                results = new List<T>();
            }

            if (this is T t && (condition == null|| condition(t))) {
                results.Add(t);
            }
            foreach (var op in inner) {
                op.GetOperations(results, condition);
            }
            return results;
        }


        public Variable[] GetVariables() {
            Func<Operation, bool> query = (o) => o as Variable != null;
            return (from x in GetOperations(condition: query) select x as Variable).ToArray();
        }
        public Gradients GetNewGradients() => new Gradients(GetVariables());

        public Operation Copy() {
            var t = new Dictionary<Operation, Operation>();
            var c = _Copy(t);
            return c;

        }

        public Operation _Copy(Dictionary<Operation, Operation> track) {
            if(track.ContainsKey(this)) {
                return track[this];
            }
            var c= Copy(track);
            track.Add(this, c);
            return c;
        }
        public abstract Operation Copy(Dictionary<Operation, Operation> track);

        protected Operation[] CopyInner(Dictionary<Operation, Operation> track) {
            Operation[] result = new Operation[inner.Length];

            for (int i = 0; i < result.Length; i++) {
                result[i] = inner[i]._Copy(track);
            }
            return result;
        }


        public static Operation operator +(Operation l, Operation r) {
            return new Add(l, r);
        }
        public static Operation operator -(Operation l, Operation r) {
            return new Subtract(l, r);
        }
        public static Operation operator *(Operation l, Operation r) {
            return new Multiply(l, r);
        }
        public static Operation operator /(Operation l, Operation r) {
            return new Divide(l, r);
        }       

        public static implicit operator Operation(float f) {
            return new Constant(new Tensor(() => f, 1));
        }
        public static implicit operator Operation(Tensor t) {
            return new Variable(t);
        }


        public Operation SetName(string name) {
            Name = name;
            return this;
        }
        public virtual string ExprString(bool requireParanthesis) {
            return "";
        }
        public string ExprString() {
            return ExprString(false);
        }
        public override string ToString() {
            return Name;
        }
    }
}