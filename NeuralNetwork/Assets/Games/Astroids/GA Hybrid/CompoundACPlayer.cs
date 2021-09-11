using UnityEngine;
using DumbML;
using LPE;
using System.Collections.Generic;

namespace Astroids {
    public class CompoundACPlayer : Player {
        public string aout;
        Tensor[] inputCache;

        VariableStack1D astroidInput = new VariableStack1D(Parameters.AstroidDataSize);
        List<Tensor> atList = new List<Tensor>();
        CompoundAC ai;
        RLExperience exp;
        ObjectPool<Tensor> atPool = new ObjectPool<Tensor>(() => new Tensor(Parameters.AstroidDataSize));

        List<Tensor> activeAT = new List<Tensor>();
        ObjectPool<Tensor> pPool = new ObjectPool<Tensor>(() => new Tensor(2));

        List<Tensor> activePT = new List<Tensor>();

        Variable[] variables;
        int score;


        public CompoundACPlayer() {
        }
        public void SetPolicies(params Operation[] policies) {
            ai = new CompoundAC(policies);
            ai.Build();
            inputCache = new Tensor[policies.Length * 2 + 2];
            variables = ai.GetVariables();

        }
        public override void Update(GameData game) {
            if (exp != null) {
                exp.reward = 0;
                ai.AddExperience(exp);
                score++;
            }
            Tensor pp = pPool.Get();
            activePT.Add(pp);
            pp[0] = game.playerPos.x / game.mapSize.x;
            pp[1] = game.playerPos.y / game.mapSize.y;

            atList.Clear();
            foreach (var a in game.astroids) {
                var t = atPool.Get();
                t[0] = a.pos.x / game.mapSize.x;
                t[1] = a.pos.y / game.mapSize.y;
                t[2] = a.dir.x;
                t[3] = a.dir.y;
                t[4] = a.size;
                atList.Add(t);
                activeAT.Add(t);
            }
            astroidInput.SetValue(atList);
            for (int i = 0; i < inputCache.Length; i += 2) {

                inputCache[i] = pp;
                inputCache[i + 1] = astroidInput.value;
            }
            exp = ai.SampleAction(inputCache);
            Vector2 dir = new Vector2(0, 0);
            switch (exp.action) {
                case 0:
                    dir = new Vector2(0, 1);
                    break;
                case 1:
                    dir = new Vector2(1, 1).normalized;
                    break;
                case 2:
                    dir = new Vector2(1, 0);
                    break;
                case 3:
                    dir = new Vector2(1, -1).normalized;
                    break;
                case 4:
                    dir = new Vector2(0, -1);
                    break;
                case 5:
                    dir = new Vector2(-1, -1).normalized;
                    break;
                case 6:
                    dir = new Vector2(-1, 0);
                    break;
                case 7:
                    dir = new Vector2(-1, 1).normalized;
                    break;
            }


            game.playerPos += dir * Parameters.PlayerSpeed;
            //ai.criticModel.forward.Eval();
            //aout = ai.actorModel.forward.value.ToString();
        }
        public override void EndGame(GameData game) {
            if (exp != null) {
                exp.reward = -1;
                ai.AddExperience(exp);
                exp = null;
            }

            ai.EndTrajectory();
            foreach (var at in activeAT) {
                atPool.Return(at);
            }
            activeAT.Clear();
            foreach (var t in activePT) {
                pPool.Return(t);
            }
            activePT.Clear();
            score = 0;
        }
    }
}
/* Steps
 * Generate population of policies
 * 
 * Evaluate policies using an AC coumpound model
 *   - First train
 *   - Then eval => avg score across multiple games
 *   
 * Next generation of policies
 */

