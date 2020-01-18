﻿using UnityEngine;
using System.Threading.Tasks;

namespace DumbML {
    public static partial class Blas {
        static bool supportComputShaders;
        static Blas() {
            supportComputShaders = SystemInfo.supportsComputeShaders;
        }




        public static Tensor MatrixMult(this Tensor l, Tensor r, Tensor dest) {
            if (l.Rank == 1 && r.Rank == 2) {
                return Parallel.MatrixMult1x2(l, r, dest);
            }

            return Parallel.MatrixMult2x2(l, r, dest);  //needs to be fixed before use

        }

        public static (Tensor, Tensor) MatrixMultBackwards(this Tensor l, Tensor r, Tensor e, (Tensor le, Tensor re) dest) {
            //if (l.Rank == 1 && r.Rank == 2) {
                return Parallel.MatrixMult1x2Backwards(l, r, e, dest);
            //}

            return Parallel.MatrixMultBackwards(l, r, e, dest);        //needs to be fixed before use
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