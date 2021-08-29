


namespace DumbML {
    public class RLExperience {
        public Tensor[] state;
        public Tensor output;
        public int action;
        public float reward;

        public Tensor nextState;
        public float weight;

        public RLExperience(Tensor[] state, Tensor output, int action) {
            this.state = state;
            this.output = output;
            this.action = action;
            weight = 1;
            reward = 0;
            nextState = null;
        }
        public RLExperience(Tensor state, Tensor output, int action) {
            this.state = new[] { state };
            this.output = output;
            this.action = action;
            weight = 1;
            reward = 0;
            nextState = null;
        }
    }
}
