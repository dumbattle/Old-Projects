using UnityEngine;

namespace DumbML {
    public static partial class Blas {
        public static class Compute {
            static ComputeShader _matrixShader;
            static ComputeShader _pointwise;
            public static ComputeShader PointwiseShader {
                get {
                    if (_pointwise == null) {
                        _pointwise = (ComputeShader)Resources.Load("Pointwise");
                    }
                    return _pointwise;
                }
            }
            public static ComputeShader MatrixShader {
                get {
                    if (_matrixShader == null) {
                        _matrixShader = (ComputeShader)Resources.Load("MatrixMult");
                    }
                    return _matrixShader;
                }
            }



            public static Tensor MatrixMult(Tensor l, Tensor r) {
                Tensor result = new Tensor(l.Shape[0], r.Shape[1]);

                ComputeBuffer bufferLeft = new ComputeBuffer(l.Size, 4);
                ComputeBuffer bufferRight = new ComputeBuffer(r.Size, 4);
                ComputeBuffer bufferOut = new ComputeBuffer(result.Size, 4);

                bufferLeft.SetData(l._value);
                bufferRight.SetData(r._value);

                int kernal = MatrixShader.FindKernel("Mult2x2");
                MatrixShader.SetBuffer(kernal, "left", bufferLeft);
                MatrixShader.SetBuffer(kernal, "right", bufferRight);
                MatrixShader.SetBuffer(kernal, "output", bufferOut);

                MatrixShader.SetInt("lx", l.Shape[0]);
                MatrixShader.SetInt("ry", r.Shape[1]);
                MatrixShader.SetInt("intermediate", r.Shape[0]);

                for (int i = 0; i < l.Shape[1]; i++) {
                    MatrixShader.SetInt("z", i);
                    MatrixShader.Dispatch(kernal, (result.Shape[0] + 7) / 8, (result.Shape[1] + 7) / 8, 1);
                }

                bufferOut.GetData(result._value);

                bufferLeft.Dispose();
                bufferRight.Dispose();
                bufferOut.Dispose();

                return result;
            }
            public static Tensor MatrixMult(Tensor l, Tensor r,Tensor dest) {
                ComputeBuffer bufferLeft = l.GetBuffer();
                ComputeBuffer bufferRight = r.GetBuffer();
                ComputeBuffer bufferOut = dest.GetBuffer(false);

                int kernal = MatrixShader.FindKernel("Mult2x2");
                MatrixShader.SetBuffer(kernal, "left", bufferLeft);
                MatrixShader.SetBuffer(kernal, "right", bufferRight);
                MatrixShader.SetBuffer(kernal, "output", bufferOut);

                MatrixShader.SetInt("lx", l.Shape[0]);
                MatrixShader.SetInt("ry", r.Shape[1]);
                MatrixShader.SetInt("intermediate", r.Shape[0]);

                for (int i = 0; i < l.Shape[1]; i++) {
                    MatrixShader.SetInt("z", i);
                    MatrixShader.Dispatch(kernal, (l.Shape[0] + 7) / 8, (r.Shape[1] + 7) / 8, 1);
                }

                bufferOut.GetData(dest._value);

                return dest;
            }

            public static Tensor Sigmoid(Tensor t) {
                Tensor result = t.SameShape();
                ComputeBuffer src = t.GetBuffer();

                PointwiseShader.SetInt("length", result.Size);
                int kernal = PointwiseShader.FindKernel("Sigmoid");

                PointwiseShader.SetBuffer(kernal, "src", src);
                PointwiseShader.Dispatch(kernal, 1, 1, 1);
                src.GetData(result._value);
                src.Dispose();
                return result;
            }
            public static Tensor Relu(Tensor t) {
                Tensor result = t.SameShape();
                ComputeBuffer src = t.GetBuffer();

                PointwiseShader.SetInt("length", result.Size);
                int kernal = PointwiseShader.FindKernel("Relu");

                PointwiseShader.SetBuffer(kernal, "src", src);
                PointwiseShader.Dispatch(kernal, 1, 1, 1);
                src.GetData(result._value);
                src.Dispose();
                return result;
            }
            public static Tensor LeakyRelu(Tensor t) {
                Tensor result = t.SameShape();
                ComputeBuffer src = t.GetBuffer();

                PointwiseShader.SetFloat("leakyness", .05f);
                PointwiseShader.SetInt("length", result.Size);
                int kernal = PointwiseShader.FindKernel("LeakyRelu");

                PointwiseShader.SetBuffer(kernal, "src", src);
                PointwiseShader.Dispatch(kernal, 1, 1, 1);
                src.GetData(result._value);
                src.Dispose();
                return result;
            }

        }

    }
}