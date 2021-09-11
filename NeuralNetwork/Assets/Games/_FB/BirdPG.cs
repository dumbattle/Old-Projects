using DumbML;

namespace Flappy {
    public class BirdPG : Bird {
        FlappyAC ai;
        RLExperience exp;


        public BirdPG(Game g) : base(g) {
            ai = new FlappyAC();
        }

        public override void Next() {
            var state = g.GetState();

            if (exp != null) {
                exp.reward = 1;
                ai.AddExperience(exp);
            }

            exp = ai.SampleAction(state);
            FlappyBird.main.output = ai.actorModel.Result?.ToString();
            FlappyBird.main.actor = ai.actorModel.Result?.ToString();
            FlappyBird.main.critic = ai.criticModel.Result?.ToString();

            if (exp.action == 0) {
                Jump();
            }

            base.Next();
        }

        public override void Reset() {
            if (exp != null) {
                exp.reward = -1;
                ai.AddExperience(exp);
                exp = null;
            }

            ai.EndTrajectory();
            base.Reset();
        }

    }

}