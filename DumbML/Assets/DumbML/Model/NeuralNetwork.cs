using System;
using System.Collections.Generic;
using FullSerializer;

namespace DumbML {
    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class NeuralNetwork : Layer {
        [fsProperty]
        public Layer inputLayer;
        [fsProperty]
        public List<Layer> Layers;

        protected NeuralNetwork() {
            Layers = new List<Layer>();
        }
        public NeuralNetwork(params int[] inputSize) {
            Layers = new List<Layer>();
            inputShape = inputSize;
            inputLayer = new InputLayer(inputSize);
        }

        public override Tensor Compute(Tensor input) {
            Tensor x = input;
            foreach (var l in Layers) {
                x = l.Compute(x);
            }

            output = x;
            return x;
        }

        public Tensor Compute(Array input) {
            Tensor x = Tensor.FromArray(input);

            foreach (var l in Layers) {
                x = l.Compute(x);
            }
            output = x;
            return x;
        }

        public T Add<T>(T layer) where T : Layer{
            Layers.Add(layer);
            return layer;
        }
        public override void Build(Layer prevLayer = null) {
            Layer l = prevLayer ?? inputLayer;

            for (int i = 0; i < Layers.Count; i++) {
                Layers[i].Build(l);
                l = Layers[i];
            }

            IsBuilt = true;
            this.outputShape = l.outputShape;
        }

        public override Tensor Forward(Tensor input, ref Context context) {
            Tensor output = input;
            Context[] ctk = new Context[Layers.Count];

            for (int i = 0; i < Layers.Count; i++) {
                Context c = new Context();
                (output) = Layers[i].Forward(output, ref c);
                ctk[i] = c;
            }

            context = new Context() {
                input = input,
                output = output
            };

            context.SaveData("ctk", ctk);
            base.output = output;
            
            return output;
        }

        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            JaggedTensor[] gradients = new JaggedTensor[Layers.Count];
            Context[] ctk = context.GetData<Context[]>("ctk");


            for (int i = Layers.Count - 1; i >= 0; i--) {
                JaggedTensor grad;
                (error, grad) = Layers[i].Backwards(ctk[i], error);
                gradients[i] = grad;
            }

            return (error, new JaggedTensor( gradients));
        }
        public override void Update(JaggedTensor gradients) {
            JaggedTensor[] inner = gradients.inner;

            for (int i = 0; i < Layers.Count; i++) {
                Layers[i].Update(inner[i]);
            }
        }
    }
}