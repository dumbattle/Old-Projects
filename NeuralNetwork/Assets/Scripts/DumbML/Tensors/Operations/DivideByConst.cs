using System.Collections.Generic;

namespace DumbML {
    public class DivideByConst : Operation {
        Dictionary<List<int>, (Tensor o, Tensor e)> dict = new Dictionary<List<int>, (Tensor o, Tensor e)>(new ShapeComparer());
        float f;
        List<int> shapeCache = new List<int>();
        Tensor e;
        Tensor[] arr = new Tensor[1];

        public DivideByConst(Operation op, float f) : base(op.shape, op) {
            this.f = f;
        }
        protected override Tensor _Compute(Tensor[] operands) {
            shapeCache.Clear();
            shapeCache.AddRange(operands[0].Shape);

            Tensor o;
                
            if (dict.ContainsKey(shapeCache)) {
                (o, e) = dict[shapeCache];
            }
            else {
                o = new Tensor(operands[0].Shape);
                e = new Tensor(operands[0].Shape);
                dict.Add(shapeCache, (o, e));
            }


            for (int i = 0; i < o.Size; i++) {
                o.value[i] = operands[0].value[i] / f;
            }

            return o;
        }
        protected override Tensor[] _BackwardsPass(Tensor err) {
            for (int i = 0; i < value.Size; i++) {
                e.value[i] = err.value[i] / f;
            }
            arr[0] = e;
            return arr;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            throw new System.NotImplementedException();
        }
    }

    
}