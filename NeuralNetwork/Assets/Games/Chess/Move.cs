using UnityEngine;

namespace Chess {
    public struct Move {
        public Vector2Int start, dest;
        public Piece promote;

        public Move(Vector2Int start, Vector2Int dest, Piece promote = null) {
            this.start = start;
            this.dest = dest;

            this.promote = promote;
        }
        public Move((int x, int y) start, Vector2Int dest, Piece promote = null) {
            this.start = new Vector2Int(start.x, start.y);
            this.dest = dest;

            this.promote = promote;
        }
        public Move((int x, int y) start, (int x, int y) dest, Piece promote = null) {
            this.start = new Vector2Int(start.x, start.y);
            this.dest = new Vector2Int(dest.x, dest.y);

            this.promote = promote;
        }
        public Move(Vector2Int start, (int x, int y) dest, Piece promote = null) {
            this.start = start;
            this.dest = new Vector2Int(dest.x, dest.y);

            this.promote = promote;
        }
    }
}