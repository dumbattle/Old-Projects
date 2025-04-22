namespace DumbML {
    public class CrossEntropy : LossFunction {

        public override (float, Tensor) Compute(Tensor output, Tensor target) {
            float e = 0;
            Tensor error;

            System.Func<float, float, float> lossFunction =
                (o, t) => {
                    if (float.IsNaN(o) || float.IsNaN(t)) {
                        return 0;
                    }
                    e += -t * (float)System.Math.Log(o);
                    return o - t;
                };

            error = Tensor.PointWise(output,
                target,
                lossFunction);

            e /= error.Size;

            return  (e, error);
        }
    }
}