//using UnityEngine;

//namespace Astroids {
//    public class ACPoolMain : AstroidMainBase {
//        delegate GameState GameState();

//        const int policyPoolSize = 100;

//        UnitPolicy[] policies;
//        CompoundAC agent;

//        GameState gs;

//        void Start() {
//            gs = Init;
//        }

//        private void Update() {
//            gs = gs() ?? gs;
//        }
//        void DisplayGame(GameData game) {
//            playerObj.transform.position = game.playerPos;
//            foreach (var a in activeAstroids) {
//                a.SetActive(false);
//                astroidPool.Return(a);
//            }
//            activeAstroids.Clear();
//            foreach (var a in game.astroids) {
//                var obj = astroidPool.Get();
//                obj.SetActive(true);
//                activeAstroids.Add(obj);

//                obj.transform.position = a.pos;
//                obj.transform.localScale = new Vector3(a.size, a.size);
//            }
//        }
//        GameState Init() {
//            InitEnvironment();
//            policies = new UnitPolicy[policyPoolSize];
//            for (int i = 0; i < policyPoolSize; i++) {
//                UnitPolicy up = new UnitPolicy();
//                up.op = ModelUtility.GeneratePolicy();
//                policies[i] = up;
//            }
//            return PolicyTraining;
//        }
//        GameState PolicyTraining() {
//            CompoundACPlayer[] players = new CompoundACPlayer[10];
//            for (int i = 0; i < 10; i++) {
//                players[i] = new CompoundACPlayer();
//                players[i].SetPolicies(
//                    policies[i + 0].op,
//                    policies[i + 1].op,
//                    policies[i + 2].op,
//                    policies[i + 3].op,
//                    policies[i + 4].op,
//                    policies[i + 5].op,
//                    policies[i + 6].op,
//                    policies[i + 7].op,
//                    policies[i + 8].op,
//                    policies[i + 9].op
//                );
//            }
//            return TrainPolicy(0);

//            GameState TrainPolicy(int i) {
//                if ()
//                CompoundACPlayer p = players[i];
//                GameController gc = new GameController();
//                gc.SetPlayer(p);
//                int count = 0;
//                return () => {
//                    bool done = gc.Update();
//                    if (done) {
//                        count++;
//                        gc.ResetGame();
//                        if (count >= 1000) {
//                            return TrainPolicy(i + 1);
//                        }
//                    }
//                    return (GameState)null;
//                };
//            }
//        }
//    }
//}
///* Steps
// * Generate population of policies
// * 
// * Evaluate policies using an AC coumpound model
// *   - First train
// *   - Then eval => avg score across multiple games
// *   
// * Next generation of policies
// */

