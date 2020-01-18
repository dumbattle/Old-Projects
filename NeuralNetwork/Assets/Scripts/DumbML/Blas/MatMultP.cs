using System.Threading.Tasks;
using System.Threading;

namespace DumbML {
    public static partial class Blas {
        public static partial class Parallel {
            public static Tensor MatrixMult(Tensor l, Tensor r) {
                return Mult2dx2d();


                Tensor Mult2dx2d() {
                    int lx = l.Shape[0];
                    int ly = l.Shape[1];
                    int rx = r.Shape[0];
                    int ry = r.Shape[1];

                    Tensor result = new Tensor(lx, ry);

                    System.Threading.Tasks.Parallel.For(0, ry, (y) => {

                        int lInd = 0;
                        for (int x = 0; x < lx; x++) {
                            int rInd = y;
                            float v = 0;
                            for (int i = 0; i < ly; i++) {
                                v += l._value[lInd++] * r._value[rInd];
                                rInd += ry;
                            }
                            result._value[x * ry + y] = v;
                        }

                    });

                    return result;
                }

            }

            public static Tensor MatrixMult2x2(Tensor l, Tensor r, Tensor dest) {
                int lx = l.Shape[0];
                int ly = l.Shape[1];
                int rx = r.Shape[0];
                int ry = r.Shape[1];

                var lv = l._value;
                var rv = r._value;
                var dv = dest._value;

                if (lx > ry) {
                    return ThreadFirst();
                }
                return ThreadLast();

                //one of these seems to not be thread safe
                Tensor ThreadLast() {
                    System.Threading.Tasks.Parallel.For(0, ry, (y) => {

                        int lInd = 0;
                        for (int x = 0; x < lx; x++) {
                            int rInd = y;
                            float v = 0;
                            for (int i = 0; i < ly; i++) {
                                v += lv[lInd++] * rv[rInd];
                                rInd += ry;
                            }
                            dv[x * ry + y] = v;
                        }

                    });

                    return dest;
                }
                Tensor ThreadFirst() {
                    System.Threading.Tasks.Parallel.For(0, lx, (x) => {
                        int dInd = x * ry;
                        for (int y = 0; y < ry; y++) {
                            float v = 0;
                            int lInd = x * ly;
                            int rInd = y;
                            for (int i = 0; i < ly; i++) {
                                v += lv[lInd] * rv[rInd];
                                lInd++;
                                rInd += ry;
                            }
                            dv[dInd] = v;
                            dInd++;
                        }

                    });

                    return dest;
                }

            }

            public static Tensor MatrixMult1x2(Tensor l, Tensor r, Tensor dest) {
                int ly = l.Shape[0];
                int ry = r.Shape[1];

                var lv = l._value;
                var rv = r._value;
                var dv = dest._value;

                System.Threading.Tasks.Parallel.For(0, ry, (y) => {
                    int rind = y;
                    float v = 0;

                    for (int i = 0; i < ly; i++) {
                        v += lv[i] * rv[rind];
                        rind += ry;
                    }

                    dv[y] = v;
                });

                return dest;
            }


            public static (Tensor le, Tensor re) MatrixMult1x2Backwards(Tensor l, Tensor r, Tensor e, (Tensor le, Tensor re) dest) {
                int ry = r.Shape[1];

                float[] lv = l._value;
                float[] rv = r._value;
                float[] dl = dest.le._value;
                float[] dr = dest.re._value;
                float[] ev = e._value;

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

            public static (Tensor le, Tensor re) MatrixMultBackwards(Tensor l, Tensor r, Tensor e, (Tensor le, Tensor re) dest) {
                int lx = l.Shape[0];
                int ly = l.Shape[1];
                int rx = r.Shape[0];
                int ry = r.Shape[1];

                var dl = dest.le._value;
                var dr = dest.re._value;
                var rv = r._value;
                var lv = l._value;
                var ev = e._value;

                for (int i = 0; i < dl.Length; i++) {
                    dl[i] = 0;
                }

                for (int i = 0; i < dr.Length; i++) {
                    dr[i] = 0;
                }


                if (lx > ry) {
                    return ThreadFirst();
                }
                return ThreadLast();

                //one of these seems to not be thread safe
                (Tensor le, Tensor re) ThreadLast() {
                    System.Threading.Tasks.Parallel.For(0, ry, (y) => {
                        int eInd = y;
                        int lInd = 0;

                        for (int x = 0; x < lx; x++) {
                            float v = ev[eInd];
                            int rInd = y;

                            for (int i = 0; i < ly; i++) {
                                dl[lInd] += v * rv[rInd];
                                dr[rInd] += v * lv[lInd++];
                                rInd += ry;
                            }
                            eInd += ry;
                        }

                    });

                    return dest;
                }
                (Tensor le, Tensor re) ThreadFirst() {
                    System.Threading.Tasks.Parallel.For(0, lx, (x) => {
                        int dInd = x * ry;

                        for (int y = 0; y < ry; y++) {
                            float v = ev[dInd];
                            int lInd = x * ly;
                            int rInd = y;

                            for (int i = 0; i < ly; i++) {
                                dl[lInd] += v * rv[rInd];
                                dr[rInd] += v * lv[lInd++];
                                rInd += ry;
                            }
                            dInd++;
                        }

                    });

                    return dest;
                }
            }
        }

    }
}