using DumbML;

namespace Flappy {
    public class BirdRL : Bird {
        public FlappyAgent ai;

        RLExperience exp;

        public BirdRL(Game g) : base(g) {
            ai = new FlappyAgent();
        }

        public override void Reset() {
            if (exp.state != null) {
                exp.reward = -1;
                ai.AddExperience(exp);
            }
            exp = null;

            base.Reset();
        }


        public override void Next() {
            ai.Train(32, 1);

            var t = g.GetState();

            if (exp.state != null) {
                exp.reward = 1;
                exp.nextState = t;
                ai.AddExperience(exp);
            }

            exp = ai.GetAction(t);

            if (exp.action == 0) {
                Jump();
            }

            FlappyBird.main.output = ai.Result.ToString();
            FlappyBird.main.advantage = ai.a.value.ToString();
            FlappyBird.main.value = ai.v.value.ToString();
            base.Next();
        }  
    }

}