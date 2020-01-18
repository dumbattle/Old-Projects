using System.Linq;
using System.Collections;
using UnityEngine;
using DumbML;

namespace Chess {
    public class Chess : MonoBehaviour {
        public static Chess Main;


        public GameObject whiteTile, blackTile;
        public GameObject pieceObj;

        public Sprite B_K, B_Q, B_R, B_B, B_N, B_P, W_K, W_Q, W_R, W_B, W_N, W_P;

        SpriteRenderer[,] board = new SpriteRenderer[8, 8];
        SpriteRenderer[,] tiles = new SpriteRenderer[8, 8];
        Game game;
        bool playing;

        public float exploration = .99f;
        public float loss = 0;
        public Color playerColor;

        ChessAgent agent;
        Channel channel;

        void Awake() {
            Main = this;
        }
        void Start() {
            whiteTile.SetActive(false);
            blackTile.SetActive(false);
            pieceObj.SetActive(false);

            SetupBoard();

            channel = Graph.GetChannel("loss");
            StartCoroutine(Next());
        }

        //void Update() {
        //    var result = game.Next();
        //    DrawBoard(game);

        //    if (result != GameResult.NotDone) {
        //        //channel.Feed(loss);
        //         loss = ChessTrainer.TrainEvaluators(agent.ai)[0];
        //        ResetGames();
        //    }
        //}

        IEnumerator Next() {
            //play 25 games
            //for (int i = 0; i < 5; i++) {
            //    game = new Game(agent ?? ChessPlayer.Random, ChessPlayer.Random);
            //    game.collectExperience = true;
            //    while (game.Next() == GameResult.NotDone) {
            //        DrawBoard(game);
            //        yield return null;
            //    }
            //    Debug.Log($"Random Game {i + 1} finished");
            //}
            agent = ChessPlayer.NewAgent();

            //init evals
            //var states = ChessTrainer.GetUniqueStates();
            //Debug.Log($"Number of Unique states: {states.Length}");

            //var scores = agent.ai.GetRandomScores(states.Length);
            //for (int i = 0; i < 1000; i++) {
            //    var losses = agent.ai.TrainEvaluators(states, scores);
            //    string log = $"Epoch {i + 1}: ";

            //    foreach (var lo in losses) {
            //        log += $"{lo}, ";
            //    }
            //    Debug.Log(log);
            //    yield return null;
            //}

            //train
            ChessTrainer.ClearMemory();
            int w = 0;
            int l = 0;
            int d = 0;



            for (int i = 0; ; i++) {
                game = new Game(ChessPlayer.MinMax, ChessPlayer.MinMax);
                game.collectExperience = true;

                //play game
                GameResult result;
                do {
                    result = game.Next();

                    DrawBoard(game);
                    yield return null;
                } while (result == GameResult.NotDone);
                if (result == GameResult.WhiteWins) {
                    w++;
                }
                else if (result == GameResult.BlackWins) {
                    l++;
                }
                else {
                    d++;
                }
                //for (int t = 0; t < 10; t++) {
                //    ChessTrainer.Train(agent.ai, 32);
                //}
                Debug.Log($"Game {i + 1}: {w}-{l}-{d}");
                //Debug.Log($"Game {i + 1}: {w}-{l}-{d} loss: {ChessTrainer.Train(agent.ai, 32)}");
            }
            //StartCoroutine(Next());
        }


        void DrawBoard(Game g) {
            UnityEngine.Color w = whiteTile.GetComponent<SpriteRenderer>().color;
            UnityEngine.Color b = blackTile.GetComponent<SpriteRenderer>().color;
            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    board[x, y].sprite = g.position[x, y].Sprite;
                    var c = (x + y) % 2 == 0 ? w : b;
                    tiles[x, y].color = c;
                }
            }
            HighlihtPlayerTiles();
        }
        void HighlihtPlayerTiles() {
            if (!HumanChessPlayer.tileSelected) {
                return;
            }

            foreach (var m in HumanChessPlayer.choices) {
                tiles[m.dest.x, m.dest.y].color *= .7f;
            }
        }
        void SetupBoard() {
            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    var t = (x + y) % 2 == 0 ? whiteTile : blackTile;
                    var tileObj = Instantiate(t, new Vector3(x - 3.5f, y - 3.5f), Quaternion.identity);
                    tileObj.SetActive(true);
                    tiles[x, y] = tileObj.GetComponent<SpriteRenderer>();

                    var obj = Instantiate(pieceObj, new Vector3(x - 3.5f, y - 3.5f), Quaternion.identity);
                    obj.SetActive(true);
                    board[x, y] = obj.GetComponent<SpriteRenderer>();
                }
            }
        }
    }
    public enum Stage {
        GatherInitial,
        TrainAux,
        TrainMain
    }
    public enum Color {
        none,
        white,
        black
    }
    public static class ColorExt {
       public static Color Opposite (this Color c) {
            if (c == Color.none) {
                return c;
            }

            if (c == Color.white) {
                return Color.black;
            }
            return Color.white;
        }
    }
}