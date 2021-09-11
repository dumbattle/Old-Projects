using System.Collections.Generic;

namespace DumbML {
    public class ReduceSum : Operation {
        int sumCount;
        public ReduceSum(Operation op) : base (null, op) {
            int dim = op.shape.Length - 1;
            shape = new int[dim];
            for (int i = 0; i < dim; i++) {
                shape[i] = op.shape[i];
            }
            sumCount = op.shape[dim];

        }
        protected override void _Compute(Tensor[] operands, TensorCache result) {

            result.SetShape(shape);
            float sum = 0;
            int count = 0;
            int slice = 0;
            foreach (float f in operands[0].value) {
                sum += f;
                count++;
                if (count == sumCount) {
                    result.tensor.value[slice] = sum;
                    sum = 0;
                    count = 0;
                    slice++;
                }
            }

        }

        protected override void _BackwardsPass(Tensor e, Tensor[] result) {

            int count = 0;
            int slice = 0;

            for (int i = 0; i < result[0].value.Length; i++) {
                result[0].value[i] += e.value[slice];
                count++;
                if (count == sumCount) {
                    count = 0;
                    slice++;
                }
            }
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new ReduceSum(inner[0]._Copy(track));
        }
    }

    
}