using UnityEngine;
using System.Threading.Tasks;

namespace DumbML {
    public static partial class Blas {
        public static Tensor MatrixMult(this Tensor l, Tensor r, Tensor dest) {
            var ld = l.Shape.Length;
            var rd = r.Shape.Length;
            if (ld == 1 && rd == 2) {
                return Parallel.MatrixMult1x2(l, r, dest);
            }
            if (ld == 2 && rd == 2) {
                return Parallel.MatrixMult2x2(l, r, dest);
            }
            return null;
        }

        public static (Tensor, Tensor) MatrixMultBackwards(this Tensor l, Tensor r, Tensor e, (Tensor le, Tensor re) dest) {
            var ld = l.Shape.Length;
            var rd = r.Shape.Length;
            if (ld == 1 && rd == 2) {
                return Parallel.MatrixMult1x2Backwards(l, r, e, dest);
            }
            if (ld == 2 && rd == 2) {
                return Parallel.MatrixMult2x2Backwards(l, r, e, dest);
            }
            return (null, null);
        }


        public static Tensor Sigmoid(this Tensor t) {
            return t.PointWise(S);

            static float S(float a) {

                if (a > 0) {
                    return 1 / (1 + (float)System.Math.Exp(-a));
                }
                else {
                    float exp = (float)System.Math.Exp(a);
                    return exp / (1 + exp);
                }
            }
        }
        public static Tensor Sigmoid(this Tensor t, Tensor dest) {
            return dest.PointWise(
                t, 
                (a,b) => {
                    if (b > 0) {
                        return 1 / (1 + (float)System.Math.Exp(-b));
                    }
                    else {
                        float exp = (float)System.Math.Exp(b);
                        return exp / (1 + exp);
                    }
                },
                true);

        }
        static float S(float a, float b) {

            if (b > 0) {
                return 1 / (1 + (float)System.Math.Exp(-b));
            }
            else {
                float exp = (float)System.Math.Exp(b);
                return exp / (1 + exp);
            }
        }
        public static Tensor Tanh(this Tensor t, Tensor dest) {
            return dest.PointWise(t, T, true);

            static float T(float a, float b) {
                float result;
                if (b > 0) {
                    result =  1 / (1 + (float)System.Math.Exp(-b));
                }
                else {
                    float exp = (float)System.Math.Exp(b);
                    result = exp / (1 + exp);
                }

                return result * 2 - 1;
            }
        }
        public static Tensor Relu(this Tensor t) {
            return t.PointWise((a) => a > 0 ? a : 0);
        }
        public static Tensor LeakyRelu(this Tensor t, float leakyness = .05f) {
            return t.PointWise((a) => a > 0 ? a : a * leakyness);
        }

    }
}