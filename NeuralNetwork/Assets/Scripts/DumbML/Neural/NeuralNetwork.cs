using System.Collections.Generic;
using System.Linq;


namespace DumbML {
    public class NeuralNetwork : Model, ILayer {
        public List<ILayer> layers = new List<ILayer>();
        public int[] inputShape;


        public bool IsBuilt { get; private set; }

        bool _trainable = true;
        public bool Trainable {
            get {
                return _trainable;
            }
            set {
                _trainable = value;

                var vars = GetVariables();

                foreach (var v in vars) {
                    v.Trainable = _trainable;
                }
            }
        }


        public NeuralNetwork(params int[] inputShape) {
            this.inputShape = inputShape;
        }
        public void Add(ILayer l) {
            if (IsBuilt) {
                return;
            }
            layers.Add(l);
        }

        public Operation Build() {
            Placeholder inputPH = new Placeholder("Input", inputShape);
            Operation x = inputPH;

            for (int i = 0; i < layers.Count; i++) {
                x = layers[i].Build(x);
            }
            forward = x;

            IsBuilt = true;
            forward.Optimize();
            Build(forward, inputPH);
            return forward;
        }
        public Operation Build(Operation input) {
            Build();
            Operation x = input;

            for (int i = 0; i < layers.Count; i++) {
                x = layers[i].Build(x);
            }
            x.Optimize();
            return x;
        }
    }
}