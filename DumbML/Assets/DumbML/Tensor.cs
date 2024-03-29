﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using FullSerializer;
using System.Linq;

namespace DumbML {
    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class Tensor {
        public static readonly Tensor Empty = new Tensor(0);

        [fsProperty]
        public float[] _value { get; private set; }

        public float this[params int[] index] {
            get {
                CheckIndex(index);
                int i = GetIndex(index);

                return _value[i];
            }
            set {
                CheckIndex(index);
                int i = GetIndex(index);
                _value[i] = value;
            }
        }

        [fsProperty]
        public int[] Shape { get; private set; }
        public int Size { get => _value.Length; }
        public int Rank { get => Shape.Length; }

        public Tensor (params int[] shape) {
            Shape = (int[])shape.Clone();
            int size = 1;

            foreach (var i in shape) {
                size *= i;
            }

            _value = new float[size];
            
        }
        public Tensor(Func<float> initializer, params int[] shape) : this (shape) {
            for (int i = 0; i < _value.Length; i++) {
                _value[i] = initializer();
            }
        }

        public Tensor SameShape() {
            return new Tensor(Shape);
        }

        public Tensor Copy() {
            IEnumerator<float> ie = GetEnumerator();


            return new Tensor(() => { ie.MoveNext(); return ie.Current; }, this.Shape);
        }

        public Tensor Reshape(params int[] shape) {
            IEnumerator<float> ie = GetEnumerator();

            Func<float> f = () => {
                ie.MoveNext();
                return ie.Current;
            };
            return new Tensor(f, shape);
        }
        public Tensor Append(Tensor t) {
            if (Shape.Length != t.Shape.Length) {
                throw new RankException("Trying to append Tensors of different dimensions");
            }

            int[] newShape = new int[Rank];
            for (int i = 1; i < Shape.Length; i++) {
                if (Shape[i] != t.Shape[i]) {
                    throw new WrongShapeException("Trying to append Tensors of different sizes");
                }
                newShape[i] = Shape[i];
            }
            newShape[0] = Shape[0] + t.Shape[0];
            IEnumerator<float> src = GetEnumerator();
            IEnumerator<float> appnd = t.GetEnumerator();
            Func<float> f = () => {
                if (src.MoveNext()) {
                    return src.Current;
                }
                else {
                    appnd.MoveNext();
                    return appnd.Current;
                }
            };
            Tensor result = new Tensor(f,newShape);
            return result;
        }
        public Tensor Shorten(int num) {
            //pretty sure this isnt correct...
            int[] newShape = new int[Rank];


            newShape[0] = Shape[0] - num;

            for (int i = 1; i < Rank; i++) {
                newShape[i] = Shape[i];
            }

            IEnumerator<float> src = GetEnumerator();
            Func<float> f = () => {
                src.MoveNext();
                return src.Current;
            };

            return new Tensor(f, newShape);

        }
        public Tensor Extend(int index, int length) {
            int[] shape = (int[])Shape.Clone();
            shape[index] += length;
            Tensor result = new Tensor(shape);


            int[] indices = new int[Rank];


            int tind = 0;

            indices[Rank - 1] = -1;

            for (int i = 0; i < result._value.Length; i++) {
                indices[Rank - 1]++;
                int j = Rank - 1;
                for (; j >= 0; j--) {
                    if (indices[j] >= shape[j]) {
                        if (j != 0) {
                            indices[j - 1]++;
                        }
                        indices[j] = 0;
                    }
                    else {
                        break;
                    }
                }


                if (indices[index] >= Shape[index]) {
                    continue;
                }

                result._value[i] = _value[tind];
                tind++;
            }
            return result;
        }

        public Tensor SwapAxis(int a, int b) {
            Tensor result = SameShape();

            //a = Rank - 1 - a;
            //b = Rank - 1 - b;
            int swap = 0;
            int[] index = new int[Rank];

            int[] scales = new int[Rank];
            int s = 1;
            for (int i = Rank - 1; i >= 0; i--) {
                scales[i] = s;
                s *= Shape[i];
            }
            s = scales[a];
            scales[a] = scales[b];
            scales[b] = s;


            for (int i = 0; i < result._value.Length; i++) {
                //UnityEngine.Debug.Log(swap);
                result._value[i] = _value[swap];
                //swap++;

                index[Rank - 1]++;

                int j = Rank - 1;
                for (; j >= 0; j--) {
                    if (index[j] >= Shape[j == a ? b : (j == b ? a : j)]) {
                        if (j != 0) {
                            index[j - 1]++;

                        }
                        swap -= scales[j] * index[j];
                        swap += scales[j];
                        index[j] = 0;
                    }
                    else {

                        swap += scales[j];
                        break;
                    }
                }


            }


            result.Shape[a] = Shape[b];
            result.Shape[b] = Shape[a];
            return result;
        }
        public static Tensor FromArray(Array A) {
            int[] shape = new int[A.Rank];

            for (int i = 0; i < A.Rank; i++) {
                shape[i] = A.GetLength(i);
            }
            IEnumerator ie = A.GetEnumerator();


            Func<float> a =
               () => {
                   ie.MoveNext();

                   return (float)ie.Current;
               };

            Tensor result = new Tensor(a, shape);

            return result;
        }


        public bool CheckShape(Tensor t) {
            return CheckShape(t.Shape);
        }
        public bool CheckShape(params int[] shape) {
            if (this.Shape.Length != shape.Length) {
                return false;
            }

            for (int i = 0; i < shape.Length; i++) {
                if (this.Shape[i] != shape[i]) {
                    return false;
                }
            }
            return true;
        }
        public void CheckIndex(params int[] indexes) {
            if (indexes.Length != Shape.Length) {
                throw new ArgumentException("Index has invalid number of parameters");
            }

            for (int i = 0; i < indexes.Length; i++) {
                if (indexes[i] < 0 || indexes[i] >= Shape[i]) {
                    throw new ArgumentOutOfRangeException("indexes", $"Shape: {Shape.TOSTRING()}  Index:{indexes.TOSTRING()}");
                }
            }
        }



        public IEnumerator<float> GetEnumerator() {
            for (int i = 0; i < _value.Length; i++) {
                yield return _value[i];
            }
        }

        int GetIndex(params int[] indexes) {
            int result = 0;
            int scale = 1;

            for (int i = indexes.Length - 1; i >= 0; i--) {
                result += indexes[i] * scale;
                scale *= Shape[i];
            }

            return result;
        }
        protected float GetVal (int index, float defaultVal = 0) {
            if (index < 0 || index >= Size) {
                return defaultVal;
            }

            return _value[index];
        }
        public float CheckVal (params int[] index) {
            int ind = GetIndex(index);
            return GetVal(ind);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            int dimension = Shape.Length;
            int[] index = new int[Shape.Length];

          

            for (int i = 0; i < _value.Length; i++) {
                for (int d = Shape.Length - 1; d >= 0; d--) {
                    if (index[d] == 0) {
                        sb.Append("[");
                    } else {
                        break;
                    }
                }

                sb.Append(_value[i].ToString(/*"N2"*/));

                //up index
                bool brace = false;
                index[Shape.Length - 1]++;
                for (int d = Shape.Length - 1; d>= 0; d--) {
                    if(index[d] == Shape[d]) {
                        if (d != 0) {
                            index[d - 1]++;
                        }

                        index[d] = 0;

                        sb.Append("]");
                        brace = true;
                    } 
                }

                if (!brace) {
                    sb.Append(", ");
                }



                
            }
            //sb.Append("]");

            return sb.ToString();
        }


        public Tensor PointWise(Func<float, float> operation, bool self = false) {
            if (self) {
                for (int i = 0; i < Size; i++) {
                    _value[i] = operation(_value[i]);
                }
                return this;
            }

            Tensor result = SameShape();
            for (int i = 0; i < Size; i++) {
                result._value[i] = operation(_value[i]);
            }
            return result;
        }
        public Tensor PointWise(Tensor t, Func<float, float, float> operation, bool self = false) {
            return PointWise(this, t, operation, self);
        }
        public static Tensor PointWise(Tensor left, Tensor right, Func<float, float, float> operation, bool self = false) {
            if (!left.CheckShape(right)) {
                throw new ArgumentException($"Tensors need to be the same size to perform point-wise operation." +
                    $"  Left: {left.Shape.TOSTRING()}  Right: {right.Shape.TOSTRING()}");
            }

            if (self) {
                for (int i = 0; i < left.Size; i++) {
                    left._value[i] = operation(left._value[i], right._value[i]);
                }
                return left;
            }

            Tensor result = left.SameShape();
            for (int i = 0; i < left.Size; i++) {
                result._value[i] = operation(left._value[i], right._value[i]);
            }
            return result;
        }


        public Tensor Add(float val, bool self = false) {
            if (self) {
                for (int i = 0; i < Size; i++) {
                    _value[i] += val;
                }
                return this;
            }

            Tensor result = SameShape();
            for (int i = 0; i < Size; i++) {
                result._value[i] = _value[i] + val;
            }
            return result;
        }
        public Tensor Subtract(float val, bool self = false) {
            if (self) {
                for (int i = 0; i < Size; i++) {
                    _value[i] -= val;
                }
                return this;
            }

            Tensor result = SameShape();
            for (int i = 0; i < Size; i++) {
                result._value[i] = _value[i] - val;
            }
            return result;
        }
        public Tensor Multiply(float val, bool self = false) {
            if (self) {
                for (int i = 0; i < Size; i++) {
                    _value[i] *= val;
                }
                return this;
            }

            Tensor result = SameShape();
            for (int i = 0; i < Size; i++) {
                result._value[i] = _value[i] * val;
            }
            return result;
        }
        public Tensor Divide(float val, bool self = false) {
            if (self) {
                for (int i = 0; i < Size; i++) {
                    _value[i] /= val;
                }
                return this;
            }

            Tensor result = SameShape();
            for (int i = 0; i < Size; i++) {
                result._value[i] = _value[i] / val;
            }
            return result;
        }

        public Tensor Add(Tensor t, bool self = false) {
            if (!CheckShape(t)) {
                throw new ArgumentException($"Tensors need to be the same size to perform point-wise operation." +
                    $"  Left: {Shape.TOSTRING()}  Right: {t.Shape.TOSTRING()}");
            }

            if (self) {
                for (int i = 0; i < Size; i++) {
                    _value[i] += t._value[i];
                }
                return this;
            }

            Tensor result = t.SameShape();
            for (int i = 0; i < Size; i++) {
                result._value[i] = _value[i] + t._value[i];
            }
            return result;
        }
        public Tensor Subtract(Tensor t, bool self = false) {
            if (!CheckShape(t)) {
                throw new ArgumentException($"Tensors need to be the same size to perform point-wise operation." +
                    $"  Left: {Shape.TOSTRING()}  Right: {t.Shape.TOSTRING()}");
            }

            if (self) {
                for (int i = 0; i < Size; i++) {
                    _value[i] -= t._value[i];
                }
                return this;
            }

            Tensor result = t.SameShape();
            for (int i = 0; i < Size; i++) {
                result._value[i] = _value[i] - t._value[i];
            }
            return result;
        }
        public Tensor Multiply(Tensor t, bool self = false) {
            if (!CheckShape(t)) {
                throw new ArgumentException($"Tensors need to be the same size to perform point-wise operation." +
                    $"  Left: {Shape.TOSTRING()}  Right: {t.Shape.TOSTRING()}");
            }

            if (self) {
                for (int i = 0; i < Size; i++) {
                    _value[i] *= t._value[i];
                }
                return this;
            }

            Tensor result = t.SameShape();
            for (int i = 0; i < Size; i++) {
                result._value[i] = _value[i] * t._value[i];
            }
            return result;
        }
        public Tensor Divide(Tensor t, bool self = false) {
            if (!CheckShape(t)) {
                throw new ArgumentException($"Tensors need to be the same size to perform point-wise operation." +
                    $"  Left: {Shape.TOSTRING()}  Right: {t.Shape.TOSTRING()}");
            }

            if (self) {
                for (int i = 0; i < Size; i++) {
                    _value[i] /= t._value[i];
                }
                return this;
            }

            Tensor result = t.SameShape();
            for (int i = 0; i < Size; i++) {
                result._value[i] = _value[i] / t._value[i];
            }
            return result;
        }



        public static Tensor operator +(Tensor left, Tensor right) {
            return left.Add(right);
        }
        public static Tensor operator -(Tensor left, Tensor right) {
            return left.Subtract(right);
        }
        public static Tensor operator *(Tensor left, Tensor right) {
            return left.Multiply(right);
        }
        public static Tensor operator /(Tensor left, Tensor right) {
            return left.Divide(right);
        }

        public static Tensor operator +(Tensor t, float f) {
            return t.Add(f);
        }
        public static Tensor operator -(Tensor t, float f) {
            return t.Subtract(f);
        }
        public static Tensor operator *(Tensor t, float f) {
            return t.Multiply(f);
        }
        public static Tensor operator /(Tensor t, float f) {
            return t.Divide(f);
        }
    }

    public static class TensorExtensions {
        public static float Max(this Tensor t) {
            float max = float.NegativeInfinity;
            foreach (var f in t) {
                if (f > max) {
                    max = f;
                }
            }

            return max;
        }

        public static Tensor Clamp(this Tensor t, float min, float max) {
            return t.PointWise((a) => a.Clamp(min, max));
        }

    }

}