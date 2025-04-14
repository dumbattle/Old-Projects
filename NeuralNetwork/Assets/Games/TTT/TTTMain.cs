using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;


namespace TTT {
    public class TTTMain : MonoBehaviour {
        public GameObject tile;
        [Range(0,2)]
        public int px;
        [Range(0,2)]
        public int py;
        Game game = new Game();
        GameObject[,] tiles = new GameObject[3, 3];

        Models player1;
        PPOTest ppo;
        private void Start() {
            player1 = new Models();
            ppo = new PPOTest(player1.actor, player1.critic);
            tile.SetActive(true);

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    var go = Instantiate(tile);
                    go.transform.position = new Vector3(x, y);
                    tiles[x + 1, y + 1] = go;
                }
            }

            tile.SetActive(false);
        }

        void Update() {
            // done 
            if (game.Winner() != 0) {
                game.Reset();
                ppo.EndTrajectory(game.Winner());
            }
            else if (game.Full()) {
                game.Reset();
                ppo.EndTrajectory(0);
            }
            else {
                if (game.CurrentPlayer() == 1) {
                    // agent 
                    var state = game.GetStateTensor();
                    player1.SetInput(state);

                    var probs = player1.actor.Eval();
                    var value = player1.critic.Eval();
                    int action = probs.Sample();
                    float reward = 0;

                    // random move
                    if (game.Tile(action) == 0) {
                        game.MakeMove(action);
                        var w = game.Winner();
                    }
                    else {
                        reward = -10;
                    }


                    ppo.AddExperience(state, probs, action, reward, value[0]);
                }
                else {
                    // random move
                    int x = Random.Range(0, 3);
                    int y = Random.Range(0, 3);
                    if (game.Tile(x, y) == 0) {
                        game.MakeMove(x, y);
                    }
                }
            }

            DrawGame();
        }

        void DrawGame() {
            for (int x = 0; x <= 2; x++) {
                for (int y = 0; y <= 2; y++) {
                    var v = game.Tile(x, y);
                    var c = v == 0 ? Color.white :
                            v == 1 ? Color.green :
                            Color.red;

                    tiles[x, y].GetComponent<SpriteRenderer>().color = c;
                }
            }
        }
    }
}
