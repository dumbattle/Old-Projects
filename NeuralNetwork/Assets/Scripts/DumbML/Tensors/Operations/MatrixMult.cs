using Unity.Profiling;
using UnityEngine;
using System.Collections.Generic;

namespace DumbML {

    
    public class MatrixMult : Operation {

        class Cache {
            public Tensor value;
            public Tensor le;
            public Tensor re;
        }
        Tensor[] backwardsArray = new Tensor[2];
        Dictionary<Vector3Int, Cache> dict = new Dictionary<Vector3Int, Cache>();

        Vector3Int code;
        public MatrixMult(Operation left, Operation right) : base(null, left, right) {

            if (left.shape.Length == 1) {
                shape = new[] { right.shape[1] };

            }
            else {
                shape = new[] { left.shape[0], right.shape[1] };
            }
        }

        protected override Tensor _Compute(Tensor[] operands) {
            var lshape = operands[0].Shape;
            var rshape = operands[1].Shape;
            code = new Vector3Int(lshape.Length == 1 ? 1 : lshape[0], rshape[0], rshape[1]);
            Tensor result;

            if (dict.ContainsKey(code)) {
                result = dict[code].value;
            }
            else {
                var c = new Cache();
                dict.Add(code, c);

                if (lshape.Length == 1) {
                    c.value = new Tensor(code.z);
                    c.le = new Tensor(code.y);
                    c.re = new Tensor(code.y, code.z);
                }
                else {
                    c.value = new Tensor(code.x, code.z);
                    c.le = new Tensor(code.x, code.y);
                    c.re = new Tensor(code.y, code.z);
                }


                result = c.value;
            }
            Blas.MatrixMult(operands[0], operands[1], result);
            return result;
        }
        protected override Tensor[] _BackwardsPass(Tensor e) {
            var c = dict[code];
            Blas.MatrixMultBackwards(inner[0].value, inner[1].value, e, (c.le, c.re));
            backwardsArray[0] = c.le;
            backwardsArray[1] = c.re;
            return backwardsArray;
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