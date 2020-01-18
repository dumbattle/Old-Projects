using System.Collections.Generic;
using UnityEngine;
using DumbML;

namespace Chess {
    public class Game {
        public const int NO_POSSIBLE_MOVES = -1;
        public const int THINKING = -2;
        public Board position = new Board();


        LinkedList<Tensor> history = new LinkedList<Tensor>();
        ChessPlayer whitePlayer, blackPlayer;

        public bool collectExperience = false;


        RingBuffer<Tensor> whiteMoves = new RingBuffer<Tensor>(100);
        RingBuffer<Tensor> blackMoves = new RingBuffer<Tensor>(100);


        public Game(ChessPlayer white, ChessPlayer black) {
            whitePlayer = white;
            blackPlayer = black;
        }
        public GameResult Next() {
            //get move
            Move m = position.turn == Color.white ? whitePlayer.GetMove(position) : blackPlayer.GetMove(position);

            //special cases
            if (m.start.x == NO_POSSIBLE_MOVES) {
                if (position.IsInCheck(Color.white)) {
                    //Debug.Log("Black Wins!");
                    EndGame(GameResult.BlackWins);
                    return GameResult.BlackWins;
                }
                else if (position.IsInCheck(Color.black)) {
                    //Debug.Log("White Wins!");
                    EndGame(GameResult.WhiteWins);
                    return GameResult.WhiteWins;
                }
                else {
                    //Debug.Log("Stalemate");
                    EndGame(GameResult.Draw);
                    return GameResult.Draw;
                }
            }
            if (m.start.x == THINKING) {
                return GameResult.NotDone;
            }

            //play move
            var t = position.ToFlatTensor();
            history.AddLast(t);
            //position = position.Copy();
            position.MakeMove(m);

            if (collectExperience) {
                if (position.turn == Color.white) {
                    whiteMoves.Add(position.ToFlatTensor());
                }else {
                    blackMoves.Add(position.ToFlatTensor());
                }
            }
            if (history.Count >= 500) {
                //Debug.Log("Draw due to move limit");
                EndGame(GameResult.Draw);
                return GameResult.Draw;
            }

            int repetition = 0;
            foreach (var h in history) {
                bool same = t.CompareData(h);


                if (same) {
                    repetition++;
                    if (repetition >= 3) {
                        //Debug.Log("Draw 3-fold repitition");
                        EndGame(GameResult.Draw);
                        return GameResult.Draw;
                    }
                }
            }
            return GameResult.NotDone;
        }

        void EndGame(GameResult result) {
            if (result == GameResult.WhiteWins) {
                foreach (var m in whiteMoves) {
                    ChessTrainer.wins.Add(m);
                }
                //foreach (var m in blackMoves) {
                //    ChessTrainer.losses.Add(m);
                //}
            }
            else if (result == GameResult.BlackWins) {
                foreach (var m in whiteMoves) {
                    ChessTrainer.losses.Add(m);
                }
                //foreach (var m in blackMoves) {
                //    ChessTrainer.wins.Add(m);
                //}
            }
            else {
                foreach (var m in whiteMoves) {
                    ChessTrainer.draws.Add(m);
                }
                //foreach (var m in blackMoves) {
                //    ChessTrainer.draws.Add(m);
                //}
            }
        }
    }
    public enum GameResult {
        NotDone,
        WhiteWins,
        BlackWins,
        Draw
    }
}
