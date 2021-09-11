using System.Collections.Generic;


namespace DumbML {
    public class BroadcastScalar : Operation {
        public BroadcastScalar(Operation scalar, params int[] shape) : base(shape, scalar) {
            if (scalar.shape.Length != 1 || scalar.shape[0] != 1) {
                throw new System.ArgumentException($"Input operation needs to have a shape of [1]. Got: {scalar.shape.ContentString()}");
            }
        }

        protected override void _Compute(Tensor[] operands, TensorCache result) {
            result.SetShape(shape);
            var s = operands[0][0];
            for (int i = 0; i < result.tensor.Size; i++) {
                result.tensor.value[i] = s;
            }
        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {
            float v = 0;
            for (int i = 0; i < e.value.Length; i++) {
                v += e.value[i];
            }
            result[0].value[0] += v;
        }
        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new BroadcastScalar(inner[0]._Copy(track));
        }
    }
}