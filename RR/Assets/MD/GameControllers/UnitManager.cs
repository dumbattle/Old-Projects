using System.Collections.Generic;


namespace MysteryDungeon {
    public class UnitManager {
        Game game;
        LinkedList<Unit> units = new LinkedList<Unit>();

        LinkedListNode<Unit> currentNode;

        public Unit currentUnit => currentNode.Value;

        public UnitManager(Game g) {
            game = g;            
        }

        public Unit SpawnLast(bool isPlayer = false, AIType ai = AIType.normal) {
            HexCoord? h = game.map.GetSpawnableTile();
            if (h == null) {
                return null;
            }

            Unit u = Unit.Get(game);
            u.Spawn(h.Value);
            u.playerControlled = isPlayer;
            u.ai = UnitAI.GetAi(u, isPlayer ? AIType.player : ai);

            if (currentNode != null) {
                units.AddBefore(currentNode, u);
            }
            else {
                units.AddLast(u);
                currentNode = units.First;
            }
            return u;
        }

        public Unit SpawnNext(bool isPlayer = false, AIType ai = AIType.normal) {
            HexCoord? h = game.map.GetSpawnableTile();
            if (h == null) {
                return null;
            }

            Unit u = Unit.Get(game);
            u.Spawn(h.Value);
            u.playerControlled = isPlayer;
            u.ai = UnitAI.GetAi(u, isPlayer ? AIType.player : ai);

            if (currentNode != null) {
                units.AddAfter(currentNode, u);
            }
            else {
                units.AddFirst(u);
                currentNode = units.First;
            }

            return u;
        }

        public void MoveNext() {
            currentNode = currentNode.Next ?? units.First;
        }
    }
}