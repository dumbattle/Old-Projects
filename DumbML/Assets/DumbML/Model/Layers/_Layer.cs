using System;
using System.Collections.Generic;
using FullSerializer;

namespace DumbML {
    [Serializable]
    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public /*abstract*/ class Layer {
        [fsProperty]
        public bool IsBuilt { get; protected set; }
        [fsProperty]
        public int[] inputShape { get; protected set; }
        [fsProperty]
        public int[] outputShape { get; protected set; }

        [fsProperty]
        public bool Trainable = true;
        [fsProperty]
        public int ID;
        [fsProperty]
        public Tensor output;

        public virtual void Build(Layer prevLayer) { }
        public virtual Tensor Compute(Tensor input) { return input; }

        public virtual void Update(JaggedTensor gradients) { }

        protected static Random rng = new Random();
        protected Layer () {
            ID = rng.Next(int.MinValue, int.MaxValue);
        }

        public virtual Tensor Forward(Tensor input, ref Context context) {
            context = new Context {
                input = input,
                output = Compute(input)
            };
            //UnityEngine.Debug.Log($"{input.Shape[0]} {context.input.Shape[0]} {context.output.Shape[0]}");

            return context.output;
        }
        /// <summary>
        /// Returns (input error, error gradient)
        /// </summary>
        public virtual (Tensor, JaggedTensor) Backwards(Context context, Tensor error) { return (error, new JaggedTensor(Tensor.Empty)); }

        public GD GradientDescent() {
            Tensor input = null;
            Tensor output = null;
            Context context = null;

            Func<Tensor, Tensor> fp =
                (i) => {
                    input = i;
                    output = Forward(i, ref context);
                    return output;
                };

            Func<Tensor, (Tensor, JaggedTensor)> bp =
                (error) => {
                    return Backwards(context, error);
                };
            ;

            return new GD(fp, bp);

        }

        public class GD {
            Func<Tensor, Tensor> _Forward;
            Func<Tensor, (Tensor, JaggedTensor)> _Backward;

            public GD(Func<Tensor, Tensor> Forward, Func<Tensor, (Tensor, JaggedTensor)> Backward) {
                _Forward = Forward;
                _Backward = Backward;
            }

            public Tensor Forward(Tensor input) {
                return _Forward(input);
            }
            public (Tensor, JaggedTensor) Backward(Tensor error) {
                return _Backward(error);
            }
        }



        public string ToJSON() {
            return JsonSerializer.Serialize(this);
        }
        public static Layer FromJson(string json) {
            return JsonSerializer.Deserialize<Layer>(json);
        }

        public class Context {
            public Tensor input;
            public Tensor output;

            Dictionary<string, object> _data = new Dictionary<string, object>();

            public T GetData<T>(string name) {
                object obj = _data[name];


                T result = (T)obj;


                return result;
            }

            public void SaveData (string name, object value) {
                _data.Add(name, value);
            }

        }
        public override int GetHashCode() {
            return ID;
        }
    }
}