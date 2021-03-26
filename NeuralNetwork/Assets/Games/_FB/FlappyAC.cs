using DumbML;

namespace Flappy {

    public class FlappyAC : ActorCritic {
        public FlappyAC() : base() {
            Build();
            var asset = FlappyBird.AiAsset;
            if (asset == null) {
                return;
            }

            var weights = asset.Load();
            if (weights == null) {
                return;
            }

            combinedAC.SetWeights(weights);
        }

        protected override Operation Input() {
            Operation x = new InputLayer(4).Build();
            x = new FullyConnected(30, ActivationFunction.Sigmoid).Build(x);
            return x;
        }
        protected override Operation Actor(Operation input) {
            Operation a = new FullyConnected(2).Build(input);

            a = a.Softmax();
            return a;
        }
        protected override Operation Critic(Operation input) {
            Operation c = new FullyConnected(1).Build(input);

            return c;
        }

        public override void EndTrajectory() {
            base.EndTrajectory();
            FlappyBird.AiAsset.Save(combinedAC.GetVariables());

        }

        protected override Optimizer Optimizer() {
            return new Adam();
        }
    }
}