using System.Collections.Generic;
using UnityEngine;

namespace Chess {
    public abstract class Piece {
        public static readonly Piece None = new _None();
        public static readonly Piece BlackKing = new _King(Color.black);
        public static readonly Piece BlackQueen = new _Queen(Color.black);
        public static readonly Piece BlackRook = new _Rook(Color.black);
        public static readonly Piece BlackBishop = new _Bishop(Color.black);
        public static readonly Piece BlackKnight = new _Knight(Color.black);
        public static readonly Piece BlackPawn = new _BlackPawn();

        public static readonly Piece WhiteKing = new _King(Color.white);
        public static readonly Piece WhiteQueen = new _Queen(Color.white);
        public static readonly Piece WhiteRook = new _Rook(Color.white);
        public static readonly Piece WhiteBishop = new _Bishop(Color.white);
        public static readonly Piece WhiteKnight = new _Knight(Color.white);
        public static readonly Piece WhitePawn = new _WhitePawn();

        public int value = 0;

        public Sprite Sprite = null;
        public Color color = Color.none;
        public abstract IEnumerable<Move> GetMoves(Board board, Vector2Int pos);
        public virtual bool CheckAttacksSquare(Board board, Vector2Int pos, Vector2Int target) {
            var moves = GetMoves(board, pos);
            foreach (var m in moves) {
                if (m.dest.x == target.x && m.dest.y == target.y) {
                    return true;
                }
            }
            return false;
        }


        private class _None : Piece {
            public override IEnumerable<Move> GetMoves(Board board, Vector2Int pos) {
                return null;
            }
        }
        private class _King : Piece {
            public _King(Color c) {
                color = c;
                Sprite = color == Color.white ? Chess.Main.W_K : Chess.Main.B_K;
            }

            public override IEnumerable<Move> GetMoves(Board board, Vector2Int pos) {
                //List<Move> result = new List<Move>();

                //normal moves
                for (int i = -1; i <= 1; i++) {
                    for (int j = -1; j <= 1; j++) {
                        if (i == 0 && j == 0) {
                            continue;
                        }

                        int x = pos.x + i;
                        int y = pos.y + j;

                        if (x < 0 || x > 7 || y < 0 || y > 7) {
                            continue;
                        }

                        Piece p = board[x, y];
                        if (p.color == color) {
                            continue;
                        }
                        yield return new Move(pos, (x, y));
                        //result.Add(new Move(pos, (x, y), p));
                    }
                }
                int ky = pos.y;

                //short castle
                if ((color == Color.white && board.whiteShortCastle) ||
                    (color == Color.black && board.blackShortCastle)) {
                    if (board[5, ky] == None &&
                        board[6, ky] == None) {
                        yield return new Move(pos, (6,ky));
                        //result.Add(new Move(pos, (6, ky)));
                    }
                }

                //long castle
                if ((color == Color.white && board.whiteLongCastle) ||
                    (color == Color.black && board.blackLongCastle)) {
                    if (board[3, ky] == None &&
                    board[2, ky] == None) {
                        yield return new Move(pos, (2,ky));
                        //result.Add(new Move(pos, (2, ky)));
                    }
                }
                //return result;
            }
        }
        private class _Queen : Piece {
            public _Queen(Color c) {
                color = c;
                Sprite = color == Color.white ? Chess.Main.W_Q : Chess.Main.B_Q;
                value = 9;
            }

            public override IEnumerable<Move> GetMoves(Board board, Vector2Int pos) {
                //List<Move> result = new List<Move>();
                if (color == Color.white) {
                    foreach (var m in WhiteRook.GetMoves(board, pos)) {
                        yield return m;
                    } 
                    foreach (var m in WhiteBishop.GetMoves(board, pos)) {
                        yield return m;
                    } 
                }
                else {
                    foreach (var m in BlackRook.GetMoves(board, pos)) {
                        yield return m;
                    } 
                    foreach (var m in BlackBishop.GetMoves(board, pos)) {
                        yield return m;
                    } 
                }
            }
        }
        private class _Rook : Piece {
            public _Rook(Color c) {
                color = c;
                Sprite = color == Color.white ? Chess.Main.W_R : Chess.Main.B_R;
                value = 5;
            }

            public override IEnumerable<Move> GetMoves(Board board, Vector2Int pos) {
                //List<Move> result = new List<Move>();

                //right
                for (int x = pos.x + 1; x <= 7; x++) {
                    Piece p = board[x, pos.y];
                    if (p.color == color) {
                        break;
                    }
                    if (p.color == Color.none) {
                        yield return new Move(pos, (x, pos.y));
                        continue;
                    }

                    yield return new Move(pos, (x, pos.y));
                    break;
                }

                //left
                for (int x = pos.x - 1; x >= 0; x--) {
                    Piece p = board[x, pos.y];
                    if (p.color == color) {
                        break;
                    }
                    if (p.color == Color.none) {
                        yield return new Move(pos, (x, pos.y));
                        continue;
                    }
                    yield return new Move(pos, (x, pos.y));
                    break;
                }

                //up
                for (int y = pos.y + 1; y <= 7; y++) {
                    Piece p = board[pos.x, y];
                    if (p.color == color) {
                        break;
                    }
                    if (p.color == Color.none) {
                        yield return new Move(pos, (pos.x, y));
                        //result.Add(new Move(pos, (pos.x, y), p));
                        continue;
                    }
                    yield return new Move(pos, (pos.x, y));
                    //result.Add(new Move(pos, (pos.x, y), p));
                    break;
                }

                //down
                for (int y = pos.y - 1; y >= 0; y--) {
                    Piece p = board[pos.x, y];
                    if (p.color == color) {
                        break;
                    }
                    if (p.color == Color.none) {
                        yield return new Move(pos, (pos.x, y));
                        //result.Add(new Move(pos, (pos.x,y), p));
                        continue;
                    }
                    yield return new Move(pos, (pos.x, y));
                    //result.Add(new Move(pos, (pos.x, y), p));
                    break;
                }

                //return result;
            }
        }
        private class _Bishop : Piece {
            public _Bishop(Color c) {
                color = c;
                Sprite = color == Color.white ? Chess.Main.W_B : Chess.Main.B_B;
                value = 3;
            }

            public override IEnumerable<Move> GetMoves(Board board, Vector2Int pos) {
                //List<Move> result = new List<Move>();

                //upright
                for (int i = 1; i < 8; i++) {
                    int x = pos.x + i;
                    int y = pos.y + i;

                    if (x > 7 || y > 7) {
                        break;
                    }

                    Piece p = board[x, y];
                    if (p.color == color) {
                        break;
                    }
                    if (p.color == Color.none) {
                        yield return new Move(pos, (x, y));
                        //result.Add(new Move(pos, (x, y), p));
                        continue;
                    }
                    yield return new Move(pos, (x, y));
                    //result.Add(new Move(pos, (x, y), p));
                    break;
                }

                //downright
                for (int i = 1; i < 8; i++) {
                    int x = pos.x + i;
                    int y = pos.y - i;

                    if (x > 7 || y < 0) {
                        break;
                    }

                    Piece p = board[x, y];
                    if (p.color == color) {
                        break;
                    }
                    if (p.color == Color.none) {
                        yield return new Move(pos, (x, y));
                        //result.Add(new Move(pos, (x, y), p));
                        continue;
                    }
                    yield return new Move(pos, (x, y));
                    //result.Add(new Move(pos, (x, y), p));
                    break;
                }

                //downleft
                for (int i = 1; i < 8; i++) {
                    int x = pos.x - i;
                    int y = pos.y - i;
                    
                    if (x < 0 || y < 0) {
                        break;
                    }

                    Piece p = board[x, y];
                    if (p.color == color) {
                        break;
                    }
                    if (p.color == Color.none) {
                        yield return new Move(pos, (x, y));
                        //result.Add(new Move(pos, (x, y), p));
                        continue;
                    }
                        yield return new Move(pos, (x, y));
                    //result.Add(new Move(pos, (x, y), p));
                    break;
                }

                //upleft
                for (int i = 1; i < 8; i++) {
                    int x = pos.x -i;
                    int y = pos.y + i;
                    
                    if (x < 0 || y > 7) {
                        break;
                    }

                    Piece p = board[x, y];
                    if (p.color == color) {
                        break;
                    }
                    if (p.color == Color.none) {
                        yield return new Move(pos, (x, y));
                        //result.Add(new Move(pos, (x, y), p));
                        continue;
                    }
                        yield return new Move(pos, (x, y));
                    //result.Add(new Move(pos, (x, y), p));
                    break;
                }
                //return result;
            }
        }
        private class _Knight : Piece {
            public _Knight(Color c) {
                color = c;
                Sprite = color == Color.white ? Chess.Main.W_N : Chess.Main.B_N;
                value = 3;
            }

            public override IEnumerable<Move> GetMoves(Board board, Vector2Int pos) {
                //List<Move> result = new List<Move>();

                Piece p;
                int x;
                int y;
                
                x = pos.x + 1;
                y = pos.y + 2;
                if (x < 8 && y < 8) {
                    p = board[x, y];
                    if (p.color != color) {
                        yield return new Move(pos, (x, y));
                        //result.Add(new Move(pos, (x, y), p));
                    }
                }

                x = pos.x + 2;
                y = pos.y + 1;
                if (x < 8 && y < 8) {
                    p = board[x, y];
                    if (p.color != color) {
                        yield return new Move(pos, (x, y));
                        //result.Add(new Move(pos, (x, y), p));
                    }
                }

                x = pos.x + 2;
                y = pos.y - 1;
                if (x < 8 && y > -1) {
                    p = board[x, y];
                    if (p.color != color) {
                        yield return new Move(pos, (x, y));
                        //result.Add(new Move(pos, (x, y), p));
                    }
                }

                x = pos.x - 1;
                y = pos.y - 2;
                if (x > -1 && y > -1) {
                    p = board[x, y];
                    if (p.color != color) {
                        yield return new Move(pos, (x, y));
                        //result.Add(new Move(pos, (x, y), p));
                    }
                }

                x = pos.x - 2;
                y = pos.y - 1;
                if (x > -1 && y > -1) {
                    p = board[x, y];
                    if (p.color != color) {
                        yield return new Move(pos, (x, y));
                        //result.Add(new Move(pos, (x, y), p));
                    }
                }

                x = pos.x - 2;
                y = pos.y + 1;
                if (x > -1 && y < 8) {
                    p = board[x, y];
                    if (p.color != color) {
                        yield return new Move(pos, (x, y));
                        //result.Add(new Move(pos, (x, y), p));
                    }
                }

                x = pos.x - 1;
                y = pos.y + 2;
                if (x > - 1 && y < 8) {
                    p = board[x, y];
                    if (p.color != color) {
                        yield return new Move(pos, (x, y));
                        //result.Add(new Move(pos, (x, y), p));
                    }
                }

                x = pos.x + 1;
                y = pos.y - 2;
                if (x < 8 && y > -1) {
                    p = board[x, y];
                    if (p.color != color) {
                        yield return new Move(pos, (x, y));
                        //result.Add(new Move(pos, (x, y), p));
                    }
                }

                //return result;
            }
        }

        private class _BlackPawn : Piece {
            public _BlackPawn() {
                Sprite = Chess.Main.B_P;
                color = Color.black;
                value = 1;
            }

            public override IEnumerable<Move> GetMoves(Board board, Vector2Int pos) {
                //List<Move> result = new List<Move>();
                //move
                int y = pos.y - 1;
                if (y >= 0) {
                    Piece p = board[pos.x, y];

                    if (p == None) {

                        if (pos.y == 1) {
                        yield return new Move(pos, (pos.x, y),BlackQueen);
                            //result.Add(new Move(pos, (pos.x, y), BlackQueen));
                        yield return new Move(pos, (pos.x, y),BlackRook);
                            //result.Add(new Move(pos, (pos.x, y), BlackRook));
                        yield return new Move(pos, (pos.x, y),BlackBishop);
                            //result.Add(new Move(pos, (pos.x, y), BlackBishop));
                        yield return new Move(pos, (pos.x, y),BlackKnight);
                            //result.Add(new Move(pos, (pos.x, y), BlackKnight));
                        }
                        else {
                        yield return new Move(pos, (pos.x, y));
                            //result.Add(new Move(pos, (pos.x, y)));
                        }

                        if (pos.y == 6) {
                            y--;
                            p = board[pos.x, y];

                            if (p == None) {
                        yield return new Move(pos, (pos.x, y));
                                //result.Add(new Move(pos, (pos.x, y)));
                            }
                        }

                    }
                }

                //captures
                y = pos.y - 1;
                if (y > 0) {
                    int xl = pos.x - 1;
                    int xr = pos.x + 1;

                    if (xl >= 0) {
                        Piece p = board[xl, y];
                        if (p.color != Color.none && p.color != color) {
                            yield return new Move(pos, (xl, y), BlackQueen);
                            //result.Add(new Move(pos, (xl, y), BlackQueen));
                            if (pos.y == 1) {
                                yield return new Move(pos, (xl, y), BlackRook);
                                //result.Add(new Move(pos, (xl, y), BlackRook));
                                yield return new Move(pos, (xl, y), BlackBishop);
                                //result.Add(new Move(pos, (xl, y), BlackBishop));
                                yield return new Move(pos, (xl, y), BlackKnight);
                                //result.Add(new Move(pos, (xl, y), BlackKnight));
                            }
                        }
                        if (board.enPassant == xl && y == 2) {
                            yield return new Move(pos, (xl, y));
                            //result.Add(new Move(pos, (xl, y)));
                        }
                    }
                    if (xr < 8) {
                        Piece p = board[xr, y];
                        if (p.color != Color.none && p.color != color) {
                            yield return new Move(pos, (xr, y), BlackQueen);
                            //result.Add(new Move(pos, (xr, y), BlackQueen));
                            if (pos.y == 1) {
                                yield return new Move(pos, (xr, y), BlackRook);
                                //result.Add(new Move(pos, (xr, y), BlackRook));
                                yield return new Move(pos, (xr, y), BlackBishop);
                                //result.Add(new Move(pos, (xr, y), BlackBishop));
                                yield return new Move(pos, (xr, y), BlackKnight);
                                //result.Add(new Move(pos, (xr, y), BlackKnight));
                            }
                        }
                        if (board.enPassant == xr && y == 2) {
                            yield return new Move(pos, (xr, y));
                            //result.Add(new Move(pos, (xr, y)));
                        }
                    }
                }

                //return result;
            }

            public override bool CheckAttacksSquare(Board board,Vector2Int pos, Vector2Int target) {
                return (target.y == pos.y - 1) &&
                       (target.x == pos.x + 1 || target.x == pos.x - 1);
            }
        }
        private class _WhitePawn : Piece {
            public _WhitePawn() {
                Sprite = Chess.Main.W_P;
                color = Color.white;
                value = 1;
            }
            public override IEnumerable<Move> GetMoves(Board board, Vector2Int pos) {
                //List<Move> result = new List<Move>();
                int y = pos.y + 1;

                //move
                if (y < 8) {
                    Piece p = board[pos.x, y];

                    if (p == None) {
                        if (pos.y == 6) {
                            yield return new Move(pos, (pos.x, y), WhiteQueen);
                            //result.Add(new Move(pos, (pos.x, y), WhiteQueen));
                            yield return new Move(pos, (pos.x, y), WhiteRook);
                            //result.Add(new Move(pos, (pos.x, y), WhiteRook));
                            yield return new Move(pos, (pos.x, y), WhiteBishop);
                            //result.Add(new Move(pos, (pos.x, y), WhiteBishop));
                            yield return new Move(pos, (pos.x, y), WhiteKnight);
                            //result.Add(new Move(pos, (pos.x, y), WhiteKnight));
                        }
                        else {
                            yield return new Move(pos, (pos.x, y));
                            //result.Add(new Move(pos, (pos.x, y)));
                        }
                        if (pos.y == 1) {
                            y++;
                            p = board[pos.x, y];

                            if (p == None) {
                        yield return new Move(pos, (pos.x, y));
                                //result.Add(new Move(pos, (pos.x, y)));
                            }
                        }
                    }
                }

                //captures
                y = pos.y + 1;
                int xl = pos.x - 1;
                int xr = pos.x + 1;
                if (y < 8) {
                    if (xl >= 0) {
                        Piece p = board[xl, y];
                        if (p.color != Color.none && p.color != color) {
                            yield return new Move(pos, (xl, y), WhiteQueen);
                            //result.Add(new Move(pos, (xl, y), WhiteQueen));
                            if (pos.y == 6) {
                                yield return new Move(pos, (xl, y), WhiteRook);
                                //result.Add(new Move(pos, (xl, y), WhiteRook));
                                yield return new Move(pos, (xl, y), WhiteBishop);
                                //result.Add(new Move(pos, (xl, y), WhiteBishop));
                                yield return new Move(pos, (xl, y), WhiteKnight);
                                //result.Add(new Move(pos, (xl, y), WhiteKnight));
                            }

                        }

                        if (board.enPassant == xl && y == 5) {
                            yield return new Move(pos, (xl, y));
                            //result.Add(new Move(pos, (xl, y)));
                        }
                    }
                    if (xr < 8) {
                        Piece p = board[xr, y];
                        if (p.color != Color.none && p.color != color) {
                            yield return new Move(pos, (xr, y),WhiteQueen);
                            //result.Add(new Move(pos, (xr, y), WhiteQueen));
                            if (pos.y == 6) {
                                yield return new Move(pos, (xr, y),WhiteRook);
                                //result.Add(new Move(pos, (xr, y), WhiteRook));
                                yield return new Move(pos, (xr, y),WhiteBishop);
                                //result.Add(new Move(pos, (xr, y), WhiteBishop));
                                yield return new Move(pos, (xr, y),WhiteKnight);
                                //result.Add(new Move(pos, (xr, y), WhiteKnight));
                            }
                        }
                        if (board.enPassant == xr && y == 5) {
                            yield return new Move(pos, (xr, y));
                            //result.Add(new Move(pos, (xr, y)));
                        }
                    }
                }

                //return result;
            }

            public override bool CheckAttacksSquare(Board board, Vector2Int pos, Vector2Int target) {
                return (target.y == pos.y + 1) &&
                       (target.x == pos.x + 1 || target.x == pos.x - 1);
            }
        }
    }
}