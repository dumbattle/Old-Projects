using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DumbML;

namespace Chess {
    public abstract class ChessPlayer {
        public static ChessPlayer Random = new RandomAIChessPlayer();
        public static ChessPlayer MinMax = new ChessMinMax();

        static ChessPlayer _Human = new HumanChessPlayer();
        public static ChessPlayer Human { get { HumanChessPlayer.tileSelected = false; return _Human; } }



        public abstract Move GetMove(Board position);

        public static ChessAgent NewAgent() {
            return new ChessAgent(new ChessModel(10));
        }
    }

    public class RandomAIChessPlayer : ChessPlayer {
        public override Move GetMove(Board position) {
            //get possible moves
            List<Move> possible = position.GetPossibleMoves().ToList();

            //select one at random
            if (possible.Count == 0) {
                return new Move((Game.NO_POSSIBLE_MOVES, 0), (0, 0));
            }
            return possible[UnityEngine.Random.Range(0, possible.Count)];
        }
    }
    public class ChessMinMax : ChessPlayer {

        public override Move GetMove(Board position) {
            if (!position.GetPossibleMoves().Any()) {
                return new Move((Game.NO_POSSIBLE_MOVES, 0), (0, 0));
            }
            return MiniMax.GetBest(position, position.turn == Color.white, 3);
        }
    }


    public class ChessAgent : ChessPlayer {
        public float exploration = 0;
        public ChessModel ai;
        static Channel channel;
        static ChessAgent() {
            channel = Graph.GetChannel("Eval");
            channel.SetRange(-1, 1);
        }

        float[] scores = new float[218]; //max numbers of moves to consider;


        public ChessAgent(ChessModel network) {
            ai = network;
        }

        public override Move GetMove(Board position) {
            Move m;
            if (UnityEngine.Random.value < exploration) {
                m = ChessPlayer.Random.GetMove(position);
            }
            else {
                //get possible moves           
                List<Move> moves = position.GetPossibleMoves().ToList();


                if (moves.Count == 0) {
                    return new Move((Game.NO_POSSIBLE_MOVES, 0), (0, 0));
                }

                //evaluate each position
                for (int i = 0; i < moves.Count; i++) {
                    //var newPos = position.Copy();
                    position.MakeMove(moves[i]);

                    scores[i] = ai.Compute(position.ToFlatTensor());
                    position.Undo();
                }

                //get best
                int best = 0;

                for (int i = 1; i < moves.Count; i++) {
                    if ((position.turn == Color.white && scores[i] > scores[best]) || // white looks for highest score
                        position.turn == Color.black && scores[i] < scores[best]) {   // black looks for lowest score
                        best = i;
                    }
                }
                channel.Feed(scores[best]);
                m = moves[best];
            }

            return m;
        }
    }

    public class HumanChessPlayer : ChessPlayer {
        public static bool tileSelected;
        public static Vector2Int selectedSquare;
        public static List<Move> choices;

        public override Move GetMove(Board position) {
            var input = PlayerControlled(position, position.turn);

            return input ?? new Move((Game.THINKING, 0), (0, 0));
        }


        Move? PlayerControlled(Board position, Color c) {
            if (!Input.GetMouseButtonDown(0)) {
                return null;
            }

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int t = new Vector2Int((int)(mousePos.x + 4), (int)(mousePos.y + 4));

            if (!tileSelected) {
                Piece p = position[t.x, t.y];

                if (p.color == c) {
                    tileSelected = true;
                    selectedSquare = t;
                    var choices = p.GetMoves(position, selectedSquare);
                    HumanChessPlayer.choices = new List<Move>();
                    //evaluate legality
                    foreach (var m in choices) {
                        //Board newPos = position.Copy();
                        position.MakeMove(m);

                        if (!position.IsInCheck(c)) {
                            HumanChessPlayer.choices.Add(m);
                        }
                        position.Undo();
                    }
                }
                return null;
            }
            else {
                tileSelected = false;
                foreach (var m in choices) {
                    if (m.dest == t) {
                        return m;
                    }
                }


                Piece p = position[t.x, t.y];

                if (p.color == c) {
                    tileSelected = true;
                    selectedSquare = t;
                    var choices = p.GetMoves(position, selectedSquare);
                    HumanChessPlayer.choices = new List<Move>();
                    //evaluate legality
                    foreach (var m in choices) {
                        position.MakeMove(m);

                        if (!position.IsInCheck(c)) {
                            HumanChessPlayer.choices.Add(m);
                        }
                        position.Undo();
                    }
                }
                return null;
            }
            return null;
        }

    }
}