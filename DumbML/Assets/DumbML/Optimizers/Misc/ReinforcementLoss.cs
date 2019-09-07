using System;

namespace DumbML {
    public class ReinforcementLoss : LossFunction {
        public override (float, Tensor) Compute(Tensor output, Tensor target) {
            float error = 0;

            Func<float, float, float> loss =
                (a, b) => {
                    float e = 0;
                    if (!float.IsNaN(b)) {
                        e = a - b;
                        error += e * e / 2;
                    }
                    return e;
                };

            Tensor result = Tensor.PointWise(output, target, loss);

            error /= result.Size;
            return (error, result);
        }
    }
}