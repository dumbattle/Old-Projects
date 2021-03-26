using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DumbML;
using Chess;

//namespace TicTacToe {
//    public class TTT : MonoBehaviour {
//        public GameObject tile;
//        SpriteRenderer[] sprites;

//        Board b;
//        void Start() {
//            sprites = new[]{
//                Instantiate(tile, new Vector2(-1,1), Quaternion.identity).GetComponent<SpriteRenderer>(),
//                Instantiate(tile, new Vector2(0,1), Quaternion.identity).GetComponent<SpriteRenderer>(),
//                Instantiate(tile, new Vector2(1,1), Quaternion.identity).GetComponent<SpriteRenderer>(),
//                Instantiate(tile, new Vector2(-1,0), Quaternion.identity).GetComponent<SpriteRenderer>(),
//                Instantiate(tile, new Vector2(0,0), Quaternion.identity).GetComponent<SpriteRenderer>(),
//                Instantiate(tile, new Vector2(1,0), Quaternion.identity).GetComponent<SpriteRenderer>(),
//                Instantiate(tile, new Vector2(-1,-1), Quaternion.identity).GetComponent<SpriteRenderer>(),
//                Instantiate(tile, new Vector2(0,-1), Quaternion.identity).GetComponent<SpriteRenderer>(),
//                Instantiate(tile, new Vector2(1,-1), Quaternion.identity).GetComponent<SpriteRenderer>()
//            };
//            tile.SetActive(false);
//            StartCoroutine(Next());
//        }

//        IEnumerator Next() {
//            b = new Board();

//            while (true) {
//                b = b.Next();
//                DrawPosition(b);
//                var r = b.Result();
//                if (r != GameResult.notDone) {
//                    print(r);
//                yield return null;
//                    StartCoroutine(Next());
//                    yield break;
//                }
//                yield return null;
//            }
//        }

//        void DrawPosition(Board b) {
//            sprites[0].color = b.a1 == -1 ? Color.red : b.a1 == 1 ? Color.green : Color.white;
//            sprites[1].color = b.b1 == -1 ? Color.red : b.b1 == 1 ? Color.green : Color.white;
//            sprites[2].color = b.c1 == -1 ? Color.red : b.c1 == 1 ? Color.green : Color.white;
//            sprites[3].color = b.a2 == -1 ? Color.red : b.a2 == 1 ? Color.green : Color.white;
//            sprites[4].color = b.b2 == -1 ? Color.red : b.b2 == 1 ? Color.green : Color.white;
//            sprites[5].color = b.c2 == -1 ? Color.red : b.c2 == 1 ? Color.green : Color.white;
//            sprites[6].color = b.a3 == -1 ? Color.red : b.a3 == 1 ? Color.green : Color.white;
//            sprites[7].color = b.b3 == -1 ? Color.red : b.b3 == 1 ? Color.green : Color.white;
//            sprites[8].color = b.c3 == -1 ? Color.red : b.c3 == 1 ? Color.green : Color.white;
//        }
//    }

//    public class Board : IMinMaxable {
//        public int player = 1;
//        public int a1, a2, a3, b1, b2, b3, c1, c2, c3;

//        public Board prev;


//        public Board Next() {
//            List<Vector2Int> moves = GetPossibleMoves();
//            if (moves.Count == 0) {
//                return this;
//            }
//            var move = GetRandomMove(moves);
//            //if (player == 1) {
//                move = MinMax(moves);
//            //}
//            return Move(move);
//        }

//        private List<Vector2Int> GetPossibleMoves() {
//            List<Vector2Int> moves = new List<Vector2Int>();
//            if (a1 == 0) {
//                moves.Add(new Vector2Int(-1, 1));
//            }
//            if (a2 == 0) {
//                moves.Add(new Vector2Int(0, 1));
//            }
//            if (a3 == 0) {
//                moves.Add(new Vector2Int(1, 1));
//            }
//            if (b1 == 0) {
//                moves.Add(new Vector2Int(-1, 0));
//            }
//            if (b2 == 0) {
//                moves.Add(new Vector2Int(0, 0));
//            }
//            if (b3 == 0) {
//                moves.Add(new Vector2Int(1, 0));
//            }
//            if (c1 == 0) {
//                moves.Add(new Vector2Int(-1, -1));
//            }
//            if (c2 == 0) {
//                moves.Add(new Vector2Int(0, -1));
//            }
//            if (c3 == 0) {
//                moves.Add(new Vector2Int(1, -1));
//            }

//            return moves;
//        }

//        Vector2Int GetRandomMove(List<Vector2Int> moves) {
//            return moves[Random.Range(0, moves.Count)];
//        }

//        Vector2Int MinMax (List<Vector2Int> moves) {
//            float[] scores = new float[moves.Count];
//            Board[] b = new Board[moves.Count];
//            for (int i = 0; i < scores.Length; i++) {
//                b[i] = Move(moves[i]);
//            }

//            return moves[MiniMax.GetBest(b, player == 1, 9)];
//        }

//        public Board Move (Vector2Int move) {
//            Board result = new Board();

//            result.a1 = a1;
//            result.a2 = a2;
//            result.a3 = a3;
//            result.b1 = b1;
//            result.b2 = b2;
//            result.b3 = b3;
//            result.c1 = c1;
//            result.c2 = c2;
//            result.c3 = c3;

//            if (move.x == -1) {
//                if (move.y == 1) {
//                    result.a1 = player;
//                }
//                else if (move.y == 0) {
//                    result.b1 = player;
//                }
//                else {
//                    result.c1 = player;
//                }
//            }
//            else if (move.x == 0) {
//                if (move.y == 1) {
//                    result.a2 = player;
//                }
//                else if (move.y == 0) {
//                    result.b2 = player;
//                }
//                else {
//                    result.c2 = player;
//                }

//            }
//            else {
//                if (move.y == 1) {
//                    result.a3 = player;
//                }
//                else if (move.y == 0) {
//                    result.b3 = player;
//                }
//                else {
//                    result.c3 = player;
//                }
//            }

//            result.prev = this;
//            result.player = player * -1;
//            return result;
//        }

//        public float GetScore() {
//            var r = Result();
//            if (r == GameResult.win) {
//                return 1;
//            }
//            if (r == GameResult.loss) {
//                return -1;
//            }
//            return 0;

//        }


//        public IEnumerable<IMinMaxable> GetNextStates() {
//            var m = GetPossibleMoves();
//            return from x in m select Move(x);
//        }

//        public GameResult Result() {
//            if (a1 != 0 && a1 == a2 && a2 == a3) {
//                return a1 == 1 ? GameResult.win : GameResult.loss;
//            }            
//            if (b1 != 0 && b1 == b2 && b2 == b3) {
//                return b1 == 1 ? GameResult.win : GameResult.loss;
//            }            
//            if (c1 != 0 && c1 == c2 && c2 == c3) {
//                return c1 == 1 ? GameResult.win : GameResult.loss;
//            }


//            if (a1 != 0 && a1 == b1 && b1 == c1) {
//                return a1 == 1 ? GameResult.win : GameResult.loss;
//            }  
//            if (a2 != 0 && a2 == b2 && b2 == c2) {
//                return a2 == 1 ? GameResult.win : GameResult.loss;
//            }
//            if (a3 != 0 && a3 == b3 && b3 == c3) {
//                return a3 == 1 ? GameResult.win : GameResult.loss;
//            }


//            if (a1 != 0 && a1 == b2 && b2 == c3) {
//                return a1 == 1 ? GameResult.win : GameResult.loss;
//            }
//            if (a3 != 0 && a3 == b2 && b2 == c1) {
//                return a3 == 1 ? GameResult.win : GameResult.loss;
//            }
//            if (a1 != 0 &&
//                a2 != 0 &&
//                a3 != 0 &&
//                b1 != 0 &&
//                b2 != 0 &&
//                b3 != 0 &&
//                c1 != 0 &&
//                c2 != 0 &&
//                c3 != 0) {
//                return GameResult.draw;
//            }

//            return GameResult.notDone;
//        }

//    }

//    public enum GameResult {
//        win,
//        loss,
//        draw,
//        notDone
//    }
//}
public static class MiniMax {
    static System.Random rng = new System.Random();

    public static Move GetBest(Board board, bool player1, int depth = 1, Func<Board, float> eval = null) {
        var moves = board.GetPossibleMoves().ToList();
        float[] scores = new float[moves.Count];
        _dict = new Dictionary<Tensor, float>();

        Parallel.For(0, scores.Length, (i) => {
            //for (int i = 0; i < scores.Length; i++) {
            var b = board.Copy();
            b.MakeMove(moves[i]);
            scores[i] = MinMax(b, eval ?? (a => a.GetScore()), depth, player1: !player1);
            //}
        });


        int result = 0;
        int player = player1 ? 1 : -1;

        for (int i = 0; i < scores.Length; i++) {
            float s = scores[i];
            if (s * player > scores[result] * player) {
                result = i;
            }
            else if (s * player == scores[result] * player && UnityEngine.Random.value < .5f) {
                result = i;
            }
        }
        return moves[result];
    }
    public static float MinMax(Board node, Func<Board, float> eval, int maxDepth = 1, float alpha = float.NegativeInfinity, float beta = float.PositiveInfinity, bool player1 = true ) {
        var t = node.ToFlatTensor();
        //if (dict.ContainsKey(t)) {
        //    return dict[t];
        //}

        var moves = node.GetPossibleMoves();

        if (--maxDepth <= 0 || !moves.Any()) {
            return eval(node);
        }
        int player = player1 ? 1 : -1;
        float result = player * -200;
        //reorder moves
        foreach (var b in moves) {
            node.MakeMove(b);
            var s = MinMax(node,eval, maxDepth, alpha, beta, !player1);
            node.Undo();

            if (s * player > result * player) {
                result = s;

                if (player == 1) {
                    alpha = result > alpha ? result : alpha;
                }
                else {
                    beta = result < beta ? result : beta;
                }

                if (alpha >= beta) {
                    break;
                }
            }
        }

        //dict.Add(t, result);
        return result;
    }


    static Dictionary<Tensor, float> _dict = new Dictionary<Tensor, float>();
    static object _lock = new object();

    static Dictionary<Tensor, float> dict {
        get { lock (_lock) { return _dict; } }
    }
}
