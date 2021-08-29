using System.Collections.Generic;
namespace DumbML {
    public class Placeholder : Operation {
        Dictionary<List<int>, Tensor> dict = new Dictionary<List<int>, Tensor>(new ShapeComparer());
        List<int> shapeCache = new List<int>();


        public Placeholder(string name, params int[] shape) : base(shape) {
            SetName(name);
        }

        public void SetVal(Tensor t) {
            var s = t.Shape;

            shapeCache.Clear();
            shapeCache.AddRange(s);

            if (dict.ContainsKey(shapeCache)) {
                value = dict[shapeCache];
            }
            else {
                value = new Tensor(shapeCache);
                dict.Add(shapeCache, value);
            }
            value.CopyFrom(t);
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