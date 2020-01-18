using System.Collections.Generic;

namespace TD {
    public class CreepMap {
        public HexMap<LinkedList<Creep>> map;

        public LinkedList<Creep> this[HexCoord h] => map[h];
        public CreepMap(Game g) {
            map = new HexMap<LinkedList<Creep>>(g.radius);

            foreach (var h in new HexGrid(g.radius)) {
                map[h] = new LinkedList<Creep>();
            }
        }

        public void Add(Creep c, HexCoord h) {
            map[h].AddLast(c);
        }
        public void Move(Creep c, HexCoord from, HexCoord to) {
            map[from].Remove(c);
            map[to].AddLast(c);
        }
        public void Remove(Creep c, HexCoord h) {
            map[h].Remove(c);
        }
    }
}