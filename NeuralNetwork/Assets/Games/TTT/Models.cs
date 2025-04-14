using DumbML;


namespace TTT {
    public class Models {
        public Operation actor { get; private set; }
        public Operation critic { get; private set; }

        Placeholder input;


        public Models() {
            BuildModels();
        }

        public void SetInput(Tensor t) {
            input.SetVal(t);
        }

        void BuildModels() {
            input = new Placeholder("Input", 9);
            actor = new FullyConnected(32, ActivationFunction.Sigmoid).Build(input);
            actor = new FullyConnected(9).Build(actor);
            actor = new Softmax(actor);

            critic = new FullyConnected(32, ActivationFunction.Sigmoid).Build(input);
            critic = new FullyConnected(1).Build(critic);
        }
    }
}
