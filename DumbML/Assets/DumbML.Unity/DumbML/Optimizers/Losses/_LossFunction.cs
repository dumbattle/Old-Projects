namespace DumbML {
    public abstract class LossFunction {
        public abstract (float, Tensor) Compute(Tensor output, Tensor target);
    }
}