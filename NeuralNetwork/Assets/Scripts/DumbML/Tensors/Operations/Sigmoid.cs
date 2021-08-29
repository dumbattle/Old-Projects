using Unity.Profiling;

using System.Collections.Generic;
namespace DumbML {
    public class Sigmoid : Operation {

        Dictionary<List<int>, Cache> dict = new Dictionary<List<int>, Cache>(new ShapeComparer());
        List<int> shapeCache = new List<int>();

        Tensor error;
        Tensor[] backwardsArray = new Tensor[1];

        public Sigmoid(Operation op) : base(op.shape, op) {
        }

        protected override Tensor _Compute(Tensor[] operands) {
            var s = operands[0].Shape;

            shapeCache.Clear();
            shapeCache.AddRange(s);
            Tensor result;

            if (dict.ContainsKey(shapeCache)) {
                var c = dict[shapeCache];
                result = c.o;
                error = c.e;
            }
            else {
                var c = new Cache();
                result = c.o = new Tensor(shapeCache);
                error = c.e = new Tensor(shapeCache);
                dict.Add(shapeCache, c);
            }

            operands[0].Sigmoid(result);

      

            return result;
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            int s = error.Size;
            for (int i = 0; i < s; i++) {
                error.value[i] = e.value[i] * value.value[i] * (1 - value.value[i]);
            }
            backwardsArray[0] = error;
            return backwardsArray;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Sigmoid(inner[0]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            return $"Sigmoid({inner[0].ExprString(false)})";
        }

        class Cache {
            public Tensor o;
            public Tensor e;
        }
    }
    
}