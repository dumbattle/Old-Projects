namespace DumbML {
    public class State {
        public Tensor input;

        // use action and reward (and next state) to compute target
        public int action;
        public float reward;
        public float maxReward;

        public State(Tensor input, int action, float reward, float maxReward) {
            this.input = input;
            this.action = action;
            this.reward = reward;
        }
    }
}