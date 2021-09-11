//using System.Collections.Generic;

//namespace DumbML {
//    /// <summary>
//    /// Wraps an operation that appears multiple times (except for the first instance) in a graph to prevent it from being calculated multiple times
//    /// </summary>
//    public class DuplicateOperation : Operation {
//        public Operation i;

//        Dictionary<List<int>, Tensor> dict = new Dictionary<List<int>, Tensor>( new ShapeComparer());
//        List<int> shapeCache = new List<int>();

//        Tensor[] err = new Tensor[1];
//        public DuplicateOperation(Operation inner) : base(inner.shape) {
//            i = inner;
//            //i.dupeError = new Tensor(i.shape);

//        }

//        protected override Tensor _Compute(Tensor[] operands) {

//            if (i.dupeError == null || !i.value.Shape.CompareContents(i.dupeError.Shape)) {
//                shapeCache.Clear();
//                shapeCache.AddRange(i.value.Shape);

//                if (dict.ContainsKey(shapeCache)) {
//                    i.dupeError = dict[shapeCache];
//                }
//                else {
//                    i.dupeError = new Tensor(shapeCache);
//                    dict.Add(shapeCache, i.dupeError);
//                }
//            }
//            return value = i.value;
//        }

//        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
//            i.dupeError.Add(e, true);
//            err[0] = e;
//            return err;
//        }
//        public override Operation Copy(Dictionary<Operation, Operation> track) {
//            if (track.ContainsKey(i)) {
//                return new DuplicateOperation(track[i]);
//            }

//            return i._Copy(track);
//        }
//        public override string ExprString(bool requireParanthesis) {
//            return "x";
//        }
//    }

//}