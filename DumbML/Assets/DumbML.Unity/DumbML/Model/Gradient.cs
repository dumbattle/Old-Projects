using System;

namespace DumbML {
    /// <summary>
    /// This is not a tensor.  Don't use as a tensor;
    /// This is just a way of holding many tensors of different sizes at once 
    /// while allowing to perform tensor operations on all of them.
    /// </summary>
    public class JaggedTensor {
        public static JaggedTensor Empty = new JaggedTensor();

        public Tensor Value { get; private set; }
        public JaggedTensor[] inner { get; private set; }
        public int Size {
            get {
                if (useInner) {
                    int s = 0;
                    for (int i = 0; i < inner.Length; i++) {
                        s += inner[i].Size;
                    }

                    return s;
                }
                else {
                    return Value.Size;
                }
            }
        }
        bool useInner { get { return Value == null; } }

        public JaggedTensor(Tensor value) {
            this.Value = value;
        }
        public JaggedTensor(params Tensor[] inner) {
            this.inner = new JaggedTensor[inner.Length];

            for (int i = 0; i < inner.Length; i++) {
                this.inner[i] = new JaggedTensor(inner[i]);
            }
        }

        public JaggedTensor(JaggedTensor[] inner) {
            this.inner = inner;
        }

        public JaggedTensor SameShape() {
            JaggedTensor result = new JaggedTensor();
            if (useInner) {
                result.inner = new JaggedTensor[inner.Length];

                for (int i = 0; i < inner.Length; i++) {
                    result.inner[i] = inner[i].SameShape();
                }
            }
            else {
                result.Value = Value.SameShape();
            }

            return result;
        }

        public void Clamp(float min, float max) {
            if (useInner) {

                for (int i = 0; i < inner.Length; i++) {
                    inner[i].Clamp(min, max);
                }
            }
            else {
                Value.Clamp(min, max);
            }
        }

        public JaggedTensor PointWise(Func<float, float> operation, bool self = false) {
            if (useInner) {
                JaggedTensor[] result = new JaggedTensor[inner.Length];

                for (int i = 0; i < inner.Length; i++) {
                    result[i] = inner[i].PointWise(operation,self);
                }

                return new JaggedTensor(result);
            }
            else {
                return new JaggedTensor(Value.PointWise(operation, self));
            }
        }
        public JaggedTensor PointWise(JaggedTensor g, Func<float, float, float> operation, bool self = false) {
            if (useInner) {
                JaggedTensor[] result = new JaggedTensor[inner.Length];

                for (int i = 0; i < inner.Length; i++) {
                    result[i] = inner[i].PointWise(g.inner[i], operation, self);
                }

                return new JaggedTensor(result);
            }
            else {
                return new JaggedTensor(Value.PointWise(g.Value, operation, self));
            }
        }


        public JaggedTensor Add(float val, bool self = false) {
            if (useInner) {
                JaggedTensor[] result = new JaggedTensor[inner.Length];

                for (int i = 0; i < inner.Length; i++) {
                    result[i] = inner[i].Add(val, self);
                }

                return new JaggedTensor(result);
            }
            else {
                return new JaggedTensor(Value.Add(val, self));
            }
        }
        public JaggedTensor Subtract(float val, bool self = false) {
            if (useInner) {
                JaggedTensor[] result = new JaggedTensor[inner.Length];

                for (int i = 0; i < inner.Length; i++) {
                    result[i] = inner[i].Subtract(val, self);
                }

                return new JaggedTensor(result);
            }
            else {
                return new JaggedTensor(Value.Subtract(val, self));
            }
        }
        public JaggedTensor Multiply(float val, bool self = false) {
            if (useInner) {
                JaggedTensor[] result = new JaggedTensor[inner.Length];

                for (int i = 0; i < inner.Length; i++) {
                    result[i] = inner[i].Multiply(val, self);
                }

                return new JaggedTensor(result);
            }
            else {
                return new JaggedTensor(Value.Multiply(val, self));
            }
        }
        public JaggedTensor Divide(float val, bool self = false) {
            if (useInner) {
                JaggedTensor[] result = new JaggedTensor[inner.Length];

                for (int i = 0; i < inner.Length; i++) {
                    result[i] = inner[i].Divide(val, self);
                }

                return new JaggedTensor(result);
            }
            else {
                return new JaggedTensor(Value.Divide(val, self));
            }
        }

        public JaggedTensor Add(JaggedTensor t, bool self = false) {
            if (useInner) {
                JaggedTensor[] result = new JaggedTensor[inner.Length];

                for (int i = 0; i < inner.Length; i++) {
                    result[i] = inner[i].Add(t.inner[i], self);
                }

                return new JaggedTensor(result);
            }
            else {
                return new JaggedTensor(Value.Add(t.Value, self));
            }
        }
        public JaggedTensor Subtract(JaggedTensor t, bool self = false) {
            if (useInner) {
                JaggedTensor[] result = new JaggedTensor[inner.Length];

                for (int i = 0; i < inner.Length; i++) {
                    result[i] = inner[i].Subtract(t.inner[i], self);
                }

                return new JaggedTensor(result);
            }
            else {
                return new JaggedTensor(Value.Subtract(t.Value, self));
            }
        }
        public JaggedTensor Multiply(JaggedTensor t, bool self = false) {
            if (useInner) {
                JaggedTensor[] result = new JaggedTensor[inner.Length];

                for (int i = 0; i < inner.Length; i++) {
                    result[i] = inner[i].Multiply(t.inner[i], self);
                }

                return new JaggedTensor(result);
            }
            else {
                return new JaggedTensor(Value.Multiply(t.Value, self));
            }
        }
        public JaggedTensor Divide(JaggedTensor t, bool self = false) {
            if (useInner) {
                JaggedTensor[] result = new JaggedTensor[inner.Length];

                for (int i = 0; i < inner.Length; i++) {
                    result[i] = inner[i].Divide(t.inner[i], self);
                }

                return new JaggedTensor(result);
            }
            else {
                return new JaggedTensor(Value.Divide(t.Value, self));
            }
        }



        public static JaggedTensor operator +(JaggedTensor g, float f) {
            return g.Add(f);
        }
        public static JaggedTensor operator -(JaggedTensor g, float f) {
            return g.Subtract(f);
        }
        public static JaggedTensor operator *(JaggedTensor g, float f) {
            return g.Multiply(f);
        }
        public static JaggedTensor operator /(JaggedTensor g, float f) {
            return g.Divide(f);
        }

        public static JaggedTensor operator +(JaggedTensor l, JaggedTensor r) {
            return l.Add(r);
        }
        public static JaggedTensor operator -(JaggedTensor l, JaggedTensor r) {
            return l.Subtract(r);
        }
        public static JaggedTensor operator *(JaggedTensor l, JaggedTensor r) {
            return l.Multiply(r);
        }
        public static JaggedTensor operator /(JaggedTensor l, JaggedTensor r) {
            return l.Divide(r);
        }
    }
}