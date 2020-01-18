namespace DumbML {
    public class Model {
        public int[] outputShape => forward?.shape;
        public Operation forward { get; set; }

        public Placeholder[] inputs;
        public Tensor Result => forward.result;

        public Model() { }
        public Model(Operation op) {
            var inputs = op.GetOperations<Placeholder>().ToArray();
            Build(op, inputs);
        }
        public Model(Operation op, params Placeholder[] inputs) {
            Build(op, inputs);
        }



        public void Build(Operation op, params Placeholder[] inputs) {
            forward = op;
            this.inputs = inputs;
        }

        public virtual Tensor Compute(params Tensor[] input) {
            SetInputs(input);

            return forward.Eval();
        }

        public void SetInputs(params Tensor[] input) {
            for (int i = 0; i < input.Length; i++) {
                inputs[i].SetVal(input[i]);
            }
        }
        public Variable[] GetVariables() {
            return forward.GetVariables();
        }

        public Tensor[] GetWeights() {
            var vars = forward.GetVariables();
            Tensor[] result = new Tensor[vars.Length];

            for (int i = 0; i < result.Length; i++) {
                result[i] = vars[i].Value.Copy();
            }
            return result;
        }
    }

}