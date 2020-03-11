using UnityEngine;
using System.Threading.Tasks;

namespace DumbML {
    public static partial class Blas {
        public static Tensor MatrixMult(this Tensor l, Tensor r, Tensor dest) {
            return Parallel.MatrixMult1x2(l, r, dest);
        }

        public static (Tensor, Tensor) MatrixMultBackwards(this Tensor l, Tensor r, Tensor e, (Tensor le, Tensor re) dest) {
            return Parallel.MatrixMult1x2Backwards(l, r, e, dest);
        }


        public static Tensor Sigmoid(this Tensor t) {
            return t.PointWise((a) => 1 / (1 + (float)System.Math.Exp(-a)));
        }
        public static Tensor Sigmoid(this Tensor t, Tensor dest) {
            return dest.PointWise(t,(a,b) => 1 / (1 + (float)System.Math.Exp(-b)),true);
        }
        public static Tensor Tanh(this Tensor t, Tensor dest) {
            return dest.PointWise(t,(a,b) => 2 / (1 + (float)System.Math.Exp(-b)) - 1,true);
        }
        public static Tensor Relu(this Tensor t) {
            return t.PointWise((a) => a > 0 ? a : 0);
        }
        public static Tensor LeakyRelu(this Tensor t, float leakyness = .05f) {
            return t.PointWise((a) => a > 0 ? a : a * leakyness);
        }

    }
}