namespace DumbML {
    public interface ILayer {
        Operation forward { get; set; }
        Operation Build(Operation input);
    }

    public abstract class Layer : Model, ILayer {
        public abstract Operation Build(Operation input);
    }

}