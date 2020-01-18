using System.Collections.Generic;
using System;
using System.Linq;

namespace DumbML {
    public abstract class Operation {
        public Tensor result { get; protected set; }
        public int[] shape;
        public Operation[] inner;

        Tensor[] operands;
        public Tensor dupeError;

        public Operation(int[] shape, params Operation[] inner) {
            if (shape != null) {
                result = new Tensor(shape);
                this.shape = (int[])shape.Clone();
            }
            this.inner = inner;
            operands = new Tensor[inner.Length];

            for (int i = 0; i < inner.Length; i++) {

                if (this as DuplicateOperation == null) {
                    var ops = inner[i].GetOperations();
                    for (int j = 0; j < inner.Length; j++) {
                        if (i == j) continue;
                        if (ops.Contains(inner[j])) {
                            inner[j] = new DuplicateOperation(inner[j]);
                        }
                    }
                }



                operands[i] = new Tensor(inner[i].shape);
            }
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

            return result = Compute(operands);
        }

        public bool Test;
        public abstract Tensor Compute(Tensor[] operands);

        public void Backwards(Gradients g) {
            Tensor e = new Tensor(() => 1, shape);
            Backwards(g, e);
        }
        public void Backwards(Optimizer o) {
            Tensor e = new Tensor(() => 1, shape);
            Backwards(o.grad, e);
        }
        public void Backwards(Gradients g, Tensor e) {

            if (g.Contains(this)) {
                g[this].Add(e, true);
            }
            Tensor[] inputErrors = BackwardsPass(e);

            for (int i = inner.Length - 1; i >= 0; i--) {
                if (inner[i].dupeError != null) {
                    inputErrors[i].Add(inner[i].dupeError, true);
                    inner[i].dupeError.Multiply(0, true);
                }

                inner[i].Backwards(g, inputErrors[i]);
            }

        }
        public void Backwards(Optimizer o,Tensor e) {
            Backwards(o.grad, e);
        }



        public abstract Tensor[] BackwardsPass(Tensor e);



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



        public virtual string ToString(bool requireParanthesis) {
            return "";
        }
        public override string ToString() {
            return ToString(false);
        }
    }

}