//using UnityEngine;
//using System;
//using System.Collections;


//namespace DumbML.Unity {
//    public class UnityBLAS : Tensor.BLAS {
//        public ComputeShader Basic = (ComputeShader)Resources.Load("BLASBasic");

//        void AssertSameShape(Tensor l, Tensor r) {
//            if (!l.CheckShape(r)) {
//                throw new ArgumentException($"Tensors need to be the same size to perform point-wise operation." +
//                    $"  Left: {l.Shape.TOSTRING()}  Right: {r.Shape.TOSTRING()}");
//            }

//        }

//        public override Tensor Add(Tensor l, Tensor r) {
//            AssertSameShape(l, r);
//            int kernalHandle = Basic.FindKernel("Add");

//            ComputeBuffer buffer1 = new ComputeBuffer(l.Size, 4);
//            buffer1.SetData(l._value);
//            ComputeBuffer buffer2 = new ComputeBuffer(r.Size, 4);
//            buffer2.SetData(r._value);

//            Basic.SetBuffer(kernalHandle, "buffer1", buffer1);
//            Basic.SetBuffer(kernalHandle, "buffer2", buffer2);

//            Basic.Dispatch(kernalHandle, l.Size, 1, 1);
//            float[] result = new float[l.Size];
//            buffer1.GetData(result);

//            buffer1.Dispose();
//            buffer2.Dispose();


//            IEnumerator ie = result.GetEnumerator();

//            Func<float> init = () => {
//                ie.MoveNext();
//                return (float)ie.Current;
//            };
//            return new Tensor(init, l.Shape);
//        }
//        public override Tensor Subtract(Tensor l, Tensor r) {
//            AssertSameShape(l, r);
//            int kernalHandle = Basic.FindKernel("Subtract");

//            ComputeBuffer buffer1 = new ComputeBuffer(l.Size, 4);
//            buffer1.SetData(l._value);
//            ComputeBuffer buffer2 = new ComputeBuffer(r.Size, 4);
//            buffer2.SetData(r._value);

//            Basic.SetBuffer(kernalHandle, "buffer1", buffer1);
//            Basic.SetBuffer(kernalHandle, "buffer2", buffer2);

//            Basic.Dispatch(kernalHandle, l.Size, 1, 1);
//            float[] result = new float[l.Size];
//            buffer1.GetData(result);

//            buffer1.Dispose();
//            buffer2.Dispose();


//            IEnumerator ie = result.GetEnumerator();

//            Func<float> init = () => {
//                ie.MoveNext();
//                return (float)ie.Current;
//            };
//            return new Tensor(init, l.Shape);
//        }
//        public override Tensor Multiply(Tensor l, Tensor r) {
//            AssertSameShape(l, r);
//            int kernalHandle = Basic.FindKernel("Multiply");

//            ComputeBuffer buffer1 = new ComputeBuffer(l.Size, 4);
//            buffer1.SetData(l._value);
//            ComputeBuffer buffer2 = new ComputeBuffer(r.Size, 4);
//            buffer2.SetData(r._value);

//            Basic.SetBuffer(kernalHandle, "buffer1", buffer1);
//            Basic.SetBuffer(kernalHandle, "buffer2", buffer2);

//            Basic.Dispatch(kernalHandle, l.Size, 1, 1);
//            float[] result = new float[l.Size];
//            buffer1.GetData(result);

//            buffer1.Dispose();
//            buffer2.Dispose();


//            IEnumerator ie = result.GetEnumerator();

//            Func<float> init = () => {
//                ie.MoveNext();
//                return (float)ie.Current;
//            };
//            return new Tensor(init, l.Shape);
//        }
//        public override Tensor Divide(Tensor l, Tensor r) {
//            AssertSameShape(l, r);
//            int kernalHandle = Basic.FindKernel("Divide");

//            ComputeBuffer buffer1 = new ComputeBuffer(l.Size, 4);
//            buffer1.SetData(l._value);
//            ComputeBuffer buffer2 = new ComputeBuffer(r.Size, 4);
//            buffer2.SetData(r._value);

//            Basic.SetBuffer(kernalHandle, "buffer1", buffer1);
//            Basic.SetBuffer(kernalHandle, "buffer2", buffer2);

//            Basic.Dispatch(kernalHandle, l.Size, 1, 1);
//            float[] result = new float[l.Size];
//            buffer1.GetData(result);

//            buffer1.Dispose();
//            buffer2.Dispose();


//            IEnumerator ie = result.GetEnumerator();

//            Func<float> init = () => {
//                ie.MoveNext();
//                return (float)ie.Current;
//            };
//            return new Tensor(init, l.Shape);
//        }
//    }
//}