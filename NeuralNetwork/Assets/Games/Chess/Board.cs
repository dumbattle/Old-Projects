using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using DumbML;


namespace Chess {
    public class Board : IEnumerable<Piece> {
        public Piece[][] board = new Piece[8][];

        public bool whiteLongCastle = true;
        public bool whiteShortCastle = true;
        public bool blackLongCastle = true;
        public bool blackShortCastle = true;

        public int enPassant = -1;

        public Color turn = Color.white;

        public Piece this[int x, int y] {
            get => board[x][y];
            set => board[x][y] = value;
        }
        public Piece this[(int x, int y) ind] {
            get => board[ind.x][ind.y];
            set => board[ind.x][ind.y] = value;
        }

        float score = 0;
        Stack<History> history = new Stack<History>();


        public Board(bool setup = true) {
            for (int x = 0; x < 8; x++) {
                board[x] = new Piece[8];
            }

            if (!setup) {
                return;
            }

            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    board[x][y] = Piece.None;
                }
            }

            board[0][0] = Piece.WhiteRook;
            board[1][0] = Piece.WhiteKnight;
            board[2][0] = Piece.WhiteBishop;
            board[3][0] = Piece.WhiteQueen;
            board[4][0] = Piece.WhiteKing;
            board[5][0] = Piece.WhiteBishop;
            board[6][0] = Piece.WhiteKnight;
            board[7][0] = Piece.WhiteRook;

            board[0][1] = Piece.WhitePawn;
            board[1][1] = Piece.WhitePawn;
            board[2][1] = Piece.WhitePawn;
            board[3][1] = Piece.WhitePawn;
            board[4][1] = Piece.WhitePawn;
            board[5][1] = Piece.WhitePawn;
            board[6][1] = Piece.WhitePawn;
            board[7][1] = Piece.WhitePawn;

            board[0][6] = Piece.BlackPawn;
            board[1][6] = Piece.BlackPawn;
            board[2][6] = Piece.BlackPawn;
            board[3][6] = Piece.BlackPawn;
            board[4][6] = Piece.BlackPawn;
            board[5][6] = Piece.BlackPawn;
            board[6][6] = Piece.BlackPawn;
            board[7][6] = Piece.BlackPawn;

            board[0][7] = Piece.BlackRook;
            board[1][7] = Piece.BlackKnight;
            board[2][7] = Piece.BlackBishop;
            board[3][7] = Piece.BlackQueen;
            board[4][7] = Piece.BlackKing;
            board[5][7] = Piece.BlackBishop;
            board[6][7] = Piece.BlackKnight;
            board[7][7] = Piece.BlackRook;
        }




        public void MakeMove(Move m) {
            History h = new History(this, m);
            history.Push(h);

            Piece moving;
            h.took = board[m.dest.x][m.dest.y];
            board[m.dest.x][m.dest.y] = moving = board[m.start.x][m.start.y];
            board[m.start.x][m.start.y] = Piece.None;

            //castling
            if (moving == Piece.WhiteKing) {
                if (whiteShortCastle && m.start == new Vector2Int(4, 0) && board[7][0] == Piece.WhiteRook && m.dest == new Vector2Int(6, 0)) {

                    board[5][0] = Piece.WhiteRook;
                    board[7][0] = Piece.None;

                }
                else if (whiteLongCastle && m.start == new Vector2Int(4, 0) && board[0][0] == Piece.WhiteRook && m.dest == new Vector2Int(2, 0)) {

                    board[3][0] = Piece.WhiteRook;
                    board[0][0] = Piece.None;


                }
                whiteLongCastle = false;
                whiteShortCastle = false;
            }
            else if (moving == Piece.BlackKing) {
                if (blackShortCastle && m.start == new Vector2Int(4, 7) && board[7][7] == Piece.BlackRook && m.dest == new Vector2Int(6, 7)) {
                    board[5][7] = Piece.BlackRook;
                    board[7][7] = Piece.None;
                }

                else if (blackLongCastle && m.start == new Vector2Int(4, 7) && board[0][7] == Piece.BlackRook && m.dest == new Vector2Int(2, 7)) {
                    board[3][7] = Piece.BlackRook;
                    board[0][7] = Piece.None;
                }

                blackLongCastle = false;
                blackShortCastle = false;
            }

            //pawn
            else if (moving == Piece.WhitePawn) {
                if (m.start.y == 1 && m.dest.y == 3) {
                    enPassant = m.start.x;
                }
                else

                if (m.start.x != m.dest.x && enPassant == m.dest.x && m.start.y == 4) {
                    h.took = board[m.dest.x][m.dest.y - 1];
                    board[m.dest.x][m.dest.y - 1] = Piece.None;
                }

                if (m.dest.y == 7) {
                    board[m.dest.x][m.dest.y] = m.promote;
                }
            }
            else if (moving == Piece.BlackPawn) {
                if (m.start.y == 6 && m.dest.y == 4) {
                    enPassant = m.start.x;
                }
                else

                if (m.start.x != m.dest.x && enPassant == m.dest.x && m.start.y == 3) {
                    h.took = board[m.dest.x][m.dest.y + 1];
                    board[m.dest.x][m.dest.y + 1] = Piece.None;
                }
                if (m.dest.y == 0) {
                    board[m.dest.x][m.dest.y] = m.promote;
                }
            }

            if (whiteLongCastle && board[0][0] != Piece.WhiteRook) {
                whiteLongCastle = false;
            }
            if (whiteShortCastle && board[7][0] != Piece.WhiteRook) {
                whiteShortCastle = false;
            }
            if (blackLongCastle && board[0][7] != Piece.BlackRook) {
                blackLongCastle = false;
            }
            if (blackShortCastle && board[7][7] != Piece.BlackRook) {
                blackShortCastle = false;
            }

            turn = turn.Opposite();
            score += h.took.color == Color.white ? -h.took.value : h.took.value;
        }

        public void Undo() {
            History h = history.Pop();

            //extra data
            whiteLongCastle = h.whiteLongCastle;
            whiteShortCastle = h.whiteShortCastle;
            blackLongCastle = h.blackLongCastle;
            blackShortCastle = h.blackShortCastle;

            enPassant = h.enPassant;
            turn = h.turn;
            score = h.score;
            Move m = h.m;

            //get moving piece
            Piece moving;
            moving = board[m.dest.x][m.dest.y];
            var tookSqr = m.dest;

            board[m.start.x][m.start.y] = moving;

            //undo castling
            if (moving == Piece.WhiteKing) {
                if (m.start == new Vector2Int(4, 0) && m.dest == new Vector2Int(6, 0)) {
                    board[7][0] = Piece.WhiteRook;
                    board[5][0] = Piece.None;

                }
                else if (m.start == new Vector2Int(4, 0) && m.dest == new Vector2Int(2, 0)) {
                    board[0][0] = Piece.WhiteRook;
                    board[3][0] = Piece.None;
                }
            }
            else if (moving == Piece.BlackKing) {
                if (m.start == new Vector2Int(4, 7) && m.dest == new Vector2Int(6, 7)) {
                    board[7][7] = Piece.BlackRook;
                    board[5][7] = Piece.None;

                }
                else if (m.start == new Vector2Int(4, 7) && m.dest == new Vector2Int(2, 7)) {
                    board[0][7] = Piece.BlackRook;
                    board[3][7] = Piece.None;

                }
            }

            //undo pawn
            else if (moving == Piece.WhitePawn) {
                if (m.start.x != m.dest.x && enPassant == m.dest.x && m.start.y == 4) {
                    tookSqr = new Vector2Int(m.dest.x, m.dest.y - 1);
                }

                if (m.dest.y == 7) {
                    board[m.start.x][m.start.y] = Piece.WhitePawn;
                }
            }
            else if (moving == Piece.BlackPawn) {
                if (m.start.x != m.dest.x && enPassant == m.dest.x && m.start.y == 3) {
                    tookSqr = new Vector2Int(m.dest.x, m.dest.y + 1);

                }
                if (m.dest.y == 0) {
                    board[m.start.x][m.start.y] = Piece.BlackPawn;

                }
            }

            //undo capture
            board[m.dest.x][m.dest.y] = Piece.None;
            board[tookSqr.x][tookSqr.y] = h.took;
        }


        public IEnumerable<Move> GetPossibleMoves() {
            List<Move> result = new List<Move>();

            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    var p = board[x][y];

                    if (p.color == turn) {
                        var moves = p.GetMoves(this, new Vector2Int(x, y));

                        //evaluate legality
                        foreach (var m in moves) {
                            MakeMove(m);

                            if (!IsInCheck(turn.Opposite())) {
                                result.Add(m);
                            }
                            Undo();
                        }
                    }

                }
            }
            return result;
        }




        public float GetScore() {
            return score;
            int result = 0;

            for (int x = 0; x < 8; x++) {
                var r = board[x];
                for (int y = 0; y < 8; y++) {
                    var p = r[y];

                    if (p.color == Color.white) {
                        result += p.value;
                    }
                    else if (p.color == Color.black) {
                        result -= p.value;
                    }
                }
            }
            return result;
        }


        public Board Copy() {
            var result = new Board(false);

            for (int x = 0; x < 8; x++) {
                var rr = result.board[x];
                var tr = board[x];
                for (int y = 0; y < 8; y++) {
                    rr[y] = tr[y];
                }
            }

            result.whiteLongCastle = whiteLongCastle;
            result.whiteShortCastle = whiteShortCastle;
            result.blackLongCastle = blackLongCastle;
            result.blackShortCastle = blackShortCastle;

            result.enPassant = enPassant;
            result.turn = turn;

            return result;
        }

        public bool IsInCheck(Color c) {
            var king = c == Color.white ? Piece.WhiteKing : Piece.BlackKing;
            Vector2Int kingPos = new Vector2Int(-1, -1);


            for (int x = 0; x < 8 && kingPos.x == -1; x++) {
                for (int y = 0; y < 8 && kingPos.x == -1; y++) {
                    if (board[x][y] == king) {
                        kingPos = new Vector2Int(x, y);
                    }
                }
            }

            return CheckColorAttack(c == Color.white ? Color.black : Color.white, kingPos);
        }

        public bool CheckColorAttack(Color c, Vector2Int pos) {
            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {

                    var p = board[x][y];

                    if (p.color == c) {

                        if (p.CheckAttacksSquare(this, new Vector2Int(x, y), pos)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public Tensor ToFlatTensor() {
            Tensor t = new Tensor(8 * 8 * 12 + 5);
            int ind = 0;

            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    int offset = 0;
                    var p = board[x][y];
                    if (p == Piece.WhiteKing || p == Piece.BlackKing) { offset = 0; }
                    if (p == Piece.WhiteQueen || p == Piece.BlackBishop) { offset = 2; }
                    if (p == Piece.WhiteRook || p == Piece.BlackRook) { offset = 4; }
                    if (p == Piece.WhiteBishop || p == Piece.BlackBishop) { offset = 6; }
                    if (p == Piece.WhiteKnight || p == Piece.BlackKnight) { offset = 8; }
                    if (p == Piece.WhitePawn || p == Piece.BlackPawn) { offset = 10; }

                    if (p.color != Color.white) {
                        offset++;
                    }
                    if (p != Piece.None) {
                        t[ind + offset] = 1;
                    }
                    ind += 12;
                }
            }
            t[ind] = whiteShortCastle ? 1 : 0;
            ind++;
            t[ind] = whiteLongCastle ? 1 : 0;
            ind++;
            t[ind] = blackShortCastle ? 1 : 0;
            ind++;
            t[ind] = blackLongCastle ? 1 : 0;
            ind++;
            t[ind] = turn == Color.white ? 1 : -1;

            return t;
        }

        public IEnumerator<Piece> GetEnumerator() {
            foreach (var r in board) {
                foreach (var p in r) {
                    yield return p;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }



        class History {
            public Move m;

            public bool whiteLongCastle = true;
            public bool whiteShortCastle = true;
            public bool blackLongCastle = true;
            public bool blackShortCastle = true;

            public int enPassant = -1;

            public Color turn = Color.white;

            public Piece took = null;
            public float score;

            public History(Board b,Move m) {
                this.m = m;

                whiteLongCastle = b.whiteLongCastle;
                whiteShortCastle = b.whiteShortCastle;
                blackLongCastle = b.blackLongCastle;
                blackShortCastle = b.blackShortCastle;

                enPassant = b.enPassant;
                turn = b.turn;
                score = b.score;
            }
        }
    }


}
// possible moves:
// normal
// take
// castle
// promote
// en passant