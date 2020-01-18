using System.Collections.Generic;

namespace AntSim {
    public class AntMap {
        HexMap<LinkedList<Ant>> map;


        public AntMap(int radius) {
            map = new HexMap<LinkedList<Ant>>(radius);

            foreach (var h in new HexGrid(radius)) {
                map[h] = new LinkedList<Ant>();
            }

        }

        public void AddAnt(Ant ant, HexCoord pos) {
            map[pos].AddFirst(ant);
        }
        public void MoveAnt(Ant ant, HexCoord prev, HexCoord newPos) {
            map[prev].Remove(ant);
            map[newPos].AddFirst(ant);
        }
        public void RemoveAnt(Ant ant, HexCoord pos) {
            map[pos].Remove(ant);
        }

        public LinkedList<Ant> GetAnts(HexCoord h) {
            if (map.IsInRange(h)) {
                if (map[h].Count == 0) {
                    return null;
                }

                return map[h];
            }
            return null;
        }
    }
}