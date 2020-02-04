using System;

namespace DumbML {
    public class Model {
        public int[] outputShape => forward?.shape;
        public Operation forward { get; set; }

        public Placeholder[] inputs;
        public Tensor Result => forward.result;

        public Model() { }
        public Model(Operation op) {
            var inputs = op.GetOperations<Placeholder>().ToArray();
            op.Optimize();
            Build(op, inputs);
        }
        public Model(Operation op, params Placeholder[] inputs) {
            Build(op, inputs);
        }



        public void Build(Operation op) {
            var inputs = op.GetOperations<Placeholder>();
            Build(op, inputs.ToArray());
        }

        public void Build(Operation op, params Placeholder[] inputs) {
            forward = op;
            this.inputs = inputs;
        }

        public Tensor Compute(params Tensor[] input) {
            SetInputs(input);

            return forward.Eval().Copy();
        }

        public void SetInputs(params Tensor[] input) {
            for (int i = 0; i < input.Length; i++) {
                inputs[i].SetVal(input[i]);
            }
        }
        public Variable[] GetVariables() {
            return forward.GetVariables();
        }

        public Model Copy() {
            var op = forward.Copy();
            return new Model(op);
        }

        public Tensor[] GetWeights() {
            var vars = forward.GetVariables();
            Tensor[] result = new Tensor[vars.Length];

            for (int i = 0; i < result.Length; i++) {
                result[i] = vars[i].Value.Copy();
            }
            return result;
        }
        public void SetWeights(Tensor[] weights) {
            var v = GetVariables();

            if (v.Length != weights.Length) {
                throw new ArgumentException($"Number of weights does not match number of variables. Got: {weights.Length} Expected: {v.Length}");
            }

            for (int i = 0; i < v.Length; i++) {
                if (!weights[i].CheckShape(v[i].shape)) {
                    throw new ArgumentException($"Cannot set weight, wrong shape. Got: {weights[i].Shape} Expected: {v[i].shape} Index: {i}");
                }
                v[i].Value = weights[i].Copy();
            }
        }
    }

}