using Unity.Profiling;
using UnityEngine;
using System.Collections.Generic;

namespace DumbML {

    
    public class MatrixMult : Operation {
        TensorCache le = new TensorCache();
        TensorCache re = new TensorCache();
        List<int> shapeCache = new List<int>();
        public MatrixMult(Operation left, Operation right) : base(null, left, right) {

            if (left.shape.Length == 1) {
                shape = new[] { right.shape[1] };

            }
            else {
                shape = new[] { left.shape[0], right.shape[1] };
            }
        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {

            var lshape = operands[0].Shape;
            var rshape = operands[1].Shape;
            var code = new Vector3Int(lshape.Length == 1 ? 1 : lshape[0], rshape[0], rshape[1]);

            shapeCache.Clear();
            if (lshape.Length == 1) {
                shapeCache.Add(code.z);
            }
            else {
                shapeCache.Add(code.x);
                shapeCache.Add(code.z);
            }

            result.SetShape(shapeCache);
            result.tensor.SetValuesToZero();
            Blas.MatrixMult(operands[0], operands[1], result.tensor);

        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            //le.SetShape(result[0].Shape);
            //re.SetShape(result[1].Shape);

            Blas.MatrixMultBackwards(inner[0].value, inner[1].value, e, (result[0], result[1]));
            //result[0].Add(le.tensor, true);
            //result[1].Add(re.tensor, true);
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new MatrixMult(inner[0]._Copy(track), inner[1]._Copy(track));
        }
        public override string ExprString(bool requireParanthesis) {
            if (requireParanthesis) {
                return $"({inner[0].ExprString(false)} x {inner[1].ExprString(false)})";
            }
            return $"{inner[0].ExprString(false)} x {inner[1].ExprString(false)}";
        }
    }
   

}