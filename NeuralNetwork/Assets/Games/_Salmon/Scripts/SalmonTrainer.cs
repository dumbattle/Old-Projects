using DumbML;
using UnityEngine;

namespace Swimming {
    public class SalmonTrainer {
        SalmonAC ai;
        SalmonGame game;
        Tensor[] stateBuffer = new Tensor[1];

        public SalmonTrainer(SalmonGame g, SalmonAC ai) {
            game = g;
            this.ai = ai;
        }

        /// <summary>
        /// Returns true if the episode is done
        /// </summary>
        public bool Next() {
            var state = game.GetState();

            stateBuffer[0] = state;
          

            var exp = ai.SampleAction(stateBuffer);
            MoveInput action = MoveInput.none;

            if (exp.action == 1) {
                action = MoveInput.up;
            }
            if (exp.action == 2) {  
                action = MoveInput.down;
            }
            var reward = game.Update(action);
            exp.reward = 1;
            ai.AddExperience(exp);
            //FlappyBird.main.output = ai.actorModel.Result?.ToString();
            //FlappyBird.main.actor = ai.actorModel.Result?.ToString();
            //FlappyBird.main.critic = ai.criticModel.Result?.ToString();

            if (game.done) {
                ai.EndTrajectory();
                game.Reset();
                return true;
            }
            return false;
        } 
    }
}