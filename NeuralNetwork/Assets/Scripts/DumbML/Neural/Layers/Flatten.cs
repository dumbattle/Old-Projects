
namespace DumbML {
    public class Flatten : Layer {
        public override Operation Build(Operation input) {
            return new FlattenOp(input);
        }
    }
}