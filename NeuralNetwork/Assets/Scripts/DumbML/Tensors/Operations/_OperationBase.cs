using System.Collections.Generic;
using System;
using System.Linq;


namespace DumbML {
    public abstract class Operation {
        public const float EPSILON = 1e-9f;
        public Tensor value => _val.tensor;
        public int[] shape;
        public Operation[] inner;
        public string Name;


        string pfName;
        static string pfForward = "OP Forwards";
        static string pfBackward = "OP Backwards";

        Tensor[] operands;
        Tensor[] inputGradients;
        List<Operation> computationOrder;
        public Tensor dupeError;
        TensorCache error = new TensorCache();
        protected TensorCache _val = new TensorCache();


        public Operation(int[] shape, params Operation[] inner) {
            if (shape != null) {
                this.shape = (int[])shape.Clone();
            }
            this.inner = (Operation[])inner.Clone();
            operands = new Tensor[inner.Length];
            inputGradients = new Tensor[inner.Length];


            Name = GetType().ToString();
            pfName = Name;
        }

        public void Optimize() {
            //RemoveDupes();
        }
        //void RemoveDupes(List<Operation> ops = null) {
        //    if (ops == null) {
        //        ops = new List<Operation>() { };
        //    }

        //    ops.Add(this);

        //    for (int i = 0; i < inner.Length; i++) {
        //        Operation op = inner[i];

        //        if (ops.Contains(op)) {
        //            inner[i] = new DuplicateOperation(op);
        //        }
        //        else {
        //            op.RemoveDupes(ops);
        //        }
        //    }
        //}

        #region Forward
        public Tensor Eval() {
            SetComputationOrder();
            for (int i = 0; i < computationOrder.Count; i++) {
                var op = computationOrder[i];
                var input = op.operands;


                for (int j = 0; j < input.Length; j++) {
                    input[j] = op.inner[j].value;
                }



                ProfileUtility.Start(pfForward);
                ProfileUtility.Start(op.pfName);
                op._Compute(input, op._val);
                ProfileUtility.End(op.pfName);
                ProfileUtility.End(pfForward);
            }

            //for (int i = 0; i < inner.Length; i++) {
            //    operands[i] = inner[i].Eval();
            //}
            //ProfileUtility.Start(pfForward);
            //ProfileUtility.Start(pfName);
            //value = _Compute(operands);
            //ProfileUtility.End(pfName);
            //ProfileUtility.End(pfForward);
            return value;
        }

        protected abstract void _Compute(Tensor[] operands, TensorCache result);
        #endregion
        #region Backward
        public void Backwards(Gradients g, Tensor e) {
            for (int i = 0; i < computationOrder.Count; i++) {
                var op = computationOrder[i];
                // set error
                op.error.SetShape(op.value.Shape).SetValuesToZero();

                // set input gradient array
                var ig = op.inputGradients;

                for (int j = 0; j < ig.Length; j++) {
                    ig[j] = op.inner[j].error.tensor;
                }
            }
            // set initial gradient
            error.tensor.CopyFrom(e);

            // do in reverse
            for (int i = computationOrder.Count - 1; i >= 0; i--) {
                var op = computationOrder[i];

                if (g.Contains(op)) {
                    g[op].Add(op.error.tensor, true);
                }
                op.BackwardsPass();
            }
        }
        public void Backwards(Gradients g) {
            errorCache ??= new Tensor(() => 1, shape);
            Backwards(g, errorCache);
        }
        Tensor errorCache;
        public void Backwards(Optimizer o) {
            errorCache ??= new Tensor(() => 1, shape);
            Backwards(o.grad, errorCache);
        }


        public void Backwards(Optimizer o, Tensor e) {
            Backwards(o.grad, e);
        }



        void BackwardsPass() {
            ProfileUtility.Start(pfBackward);
            ProfileUtility.Start(pfName);
            _BackwardsPass(error.tensor, inputGradients);
            ProfileUtility.End(pfName);
            ProfileUtility.End(pfBackward);
        }
        protected abstract void _BackwardsPass(Tensor e, Tensor[] results);

        #endregion



        #region Optimization

        void SetComputationOrder() {
            if (computationOrder != null) {
                // already done
                return;
            }

            computationOrder = new List<Operation>();
            AddToList(this, computationOrder);

            static void AddToList(Operation op, List<Operation> order) {

                foreach (var o in op.inner) {
                    AddToList(o, order);
                }
                if (!order.Contains(op)) {
                    order.Add(op);
                }
            }
        }




        #endregion

        #region Query
        public List<Operation> GetOperations(List<Operation> results = null, Func<Operation, bool> condition = null) {
            if (results == null) {
                results = new List<Operation>();
            }

            if (condition == null || condition(this) && !results.Contains(this)) {
                results.Add(this);
            }
            foreach (var op in inner) {
                op.GetOperations(results, condition);
            }
            return results;
        }

        public List<T> GetOperations<T>(List<T> results = null, Func<T, bool> condition = null) where T : Operation {
            if (results == null) {
                results = new List<T>();
            }

            if (this is T t && (condition == null || condition(t)) && !results.Contains(t)) {
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
        #endregion;

        #region Utility
        public Gradients GetNewGradients() => new Gradients(GetVariables());

        public Operation Copy() {
            var t = new Dictionary<Operation, Operation>();
            var c = _Copy(t);
            return c;

        }

        public Operation _Copy(Dictionary<Operation, Operation> track) {
            var c = Copy(track);
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

        #endregion
        #region Operators

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


        public static Operation operator /(Operation l, float r) {
            return new DivideByConst(l, r);
        }
        


        public static implicit operator Operation(float f) {
            return new Constant(new Tensor(() => f, 1));
        }
        public static implicit operator Operation(Tensor t) {
            return new Variable(t);
        }
        #endregion


    
    }


    /// <summary>
    /// If operation needs
    /// </summary>
    public class TensorCache {
        public Tensor tensor { get; private set; }
        Dictionary<List<int>, Tensor> dict = new Dictionary<List<int>, Tensor>(new ShapeComparer());
        List<int> shapeList = new List<int>(); // used to search dict


        public Tensor SetShape(int[] shape) {
            P.S();
            bool same = false;
            if (shape.Length == shapeList.Count) {
                same = true;
                for (int i = 0; i < shape.Length; i++) {
                    if(shape[i] != shapeList[i]) {
                        same = false;
                    }
                }
            }

            if (same) {
            P.E();
                return tensor;
            }

            shapeList.Clear();
            foreach (var i in shape) {
                shapeList.Add(i);
            }

            if (dict.ContainsKey(shapeList)) {
                tensor = dict[shapeList];
            }
            else {
                tensor = new Tensor(shapeList);

                dict.Add(new List<int>(shapeList), tensor);
            }
            P.E();
            return tensor;

        }
        
        public Tensor SetShape(List<int> shape) {
            P.S();
            bool same = false;
            if (shape.Count == shapeList.Count) {
                same = true;
                for (int i = 0; i < shape.Count; i++) {
                    if (shape[i] != shapeList[i]) {
                        same = false;
                    }
                }
            }

            if (same) {
            P.E();
                return tensor;
            }
            shapeList.Clear();
            foreach (var i in shape) {
                shapeList.Add(i);
            }

            if (dict.ContainsKey(shapeList)) {
                tensor = dict[shapeList];
            }
            else {
                tensor = new Tensor(shapeList);

                dict.Add(new List<int>(shapeList), tensor);
            }
            P.E();
            return tensor;

        }
    }


}