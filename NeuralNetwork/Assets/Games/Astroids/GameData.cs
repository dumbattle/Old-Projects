using System.Collections.Generic;
using UnityEngine;


namespace Astroids {
    public class GameData {
        public Vector2 mapSize;
        public Vector2 playerPos;

        public List<Astroid> astroids = new List<Astroid>();

        public bool AstroidInMap(Astroid ast) {
            var pos = ast.pos;
            if (pos.x < mapSize.x / -2f - 1f) {
                return false;
            }
            if (pos.x > mapSize.x / 2f + 1f) {
                return false;
            }
            if (pos.y < mapSize.y / -2f - 1f) {
                return false;
            }
            if (pos.y > mapSize.y / 2f + 1f) {
                return false;
            }
            return true;

        }

    }
}
