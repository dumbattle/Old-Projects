namespace DumbML {
    public class MSE : LossFunction {
        public override (float, Tensor) Compute(Tensor output, Tensor target) {
            float e = 0;
            Tensor error;

            System.Func<float, float, float> lossFunction =
                (o, t) => {
                    if (float.IsNaN(o) || float.IsNaN(t)) {
                        return 0;
                    }
                    float er = o - t;
                    e += er * er;
                    return er;
                };

            error = Tensor.PointWise(output,
                target,
                lossFunction);

            e /= error.Size * 2;

            return (e, error);
        }
    }

    public class BinaryAlphaLoss : LossFunction {
        public override (float, Tensor) Compute(Tensor output, Tensor target) {
            float e = 0;
            Tensor error = output.SameShape();
            for (int i = 0; i < output.Size / 4; i++) {
                int v = i * 4 + 3;
                float er;
                error._value[v] = er = output._value[v] - target._value[v];
                e += er * er;
                if (target._value[v] == 0) {
                    continue;
                }
                v--;
                error._value[v] = er = output._value[v] - target._value[v];
                e += er * er;
                v--;
                error._value[v] = er = output._value[v] - target._value[v];
                e += er * er;
                v--;
                error._value[v] = er = output._value[v] - target._value[v];
                e += er * er;
            }
            e /= error.Size * 2;
            return (e, error);

        }
    }
}