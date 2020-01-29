namespace DumbML {
    public class ParallelNetwork : Layer {
        public ILayer[] inner;


        public ParallelNetwork(params ILayer[] networks) {
            inner = networks;
        }


        public override Operation Build(Operation input) {
            Operation[] outputs = new Operation[inner.Length];

            for (int i = 0; i < outputs.Length; i++) {
                outputs[i] = inner[i].Build(input);
            }

            return forward = new Max(outputs);
        }
    }
}