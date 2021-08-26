﻿using System.Threading.Tasks;
using System.Threading;

namespace DumbML {
    public static partial class Blas {
        public static partial class Parallel {
            public static Tensor MatrixMult2x2(Tensor l, Tensor r, Tensor dest) {
                int lx = l.Shape[0];
                int ly = l.Shape[1];
                int rx = r.Shape[0];
                int ry = r.Shape[1];


                if (ly != rx) {
                    throw new System.InvalidOperationException($"Tensors do not have compatible dimensions: {l.Shape.ContentString()}, {r.Shape.ContentString()}");
                }
                var lv = l.value;
                var rv = r.value;
                var dv = dest.value;

                for (int x = 0; x < lx; x++) {
                    for (int y = 0; y < ry; y++) {
                        float sum = 0;
                        for (int i = 0; i < ly; i++) {
                            sum += l[x, i] * r[i, y];
                        }
                        dest[x, y] = sum;
                    }
                }

                return dest;
            }

            public static (Tensor le, Tensor re) MatrixMult2x2Backwards(Tensor l, Tensor r, Tensor e, (Tensor le, Tensor re) dest) {
                throw new System.NotImplementedException();
            }


            public static Tensor MatrixMult1x2(Tensor l, Tensor r, Tensor dest) {
                int lx = l.Shape[0];
                int ry = r.Shape[1];

                var lv = l.value;
                var rv = r.value;
                var dv = dest.value;

                if (lx > 50) {
                    System.Threading.Tasks.Parallel.For(0, ry, (y) => {
                        int rind = y;
                        float v = 0;

                        for (int i = 0; i < lx; i++) {
                            v += lv[i] * rv[rind];
                            rind += ry;
                        }

                        dv[y] = v;
                    });
                }
                else {
                    for (int y = 0; y < ry; y++) {
                        int rind = y;
                        float v = 0;

                        for (int i = 0; i < lx; i++) {
                            v += lv[i] * rv[rind];
                            rind += ry;
                        }

                        dv[y] = v;
                    }
                }

                return dest;
            }


            public static (Tensor le, Tensor re) MatrixMult1x2Backwards(Tensor l, Tensor r, Tensor e, (Tensor le, Tensor re) dest) {
                int ry = r.Shape[1];

                float[] lv = l.value;
                float[] rv = r.value;
                float[] dl = dest.le.value;
                float[] dr = dest.re.value;
                float[] ev = e.value;

                System.Threading.Tasks.Parallel.For(0, l.Shape[0], (i) => {
                    int rind = i * ry;
                    float v = 0;

                    for (int y = 0; y < ry; y++) {
                        float err = ev[y];
                        v += err * rv[rind];
                        dr[rind] = err * lv[i];
                        rind++;

                    }
                    dl[i] = v;
                });

                return dest;
            }
        }

    }
}