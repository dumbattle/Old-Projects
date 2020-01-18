using System;

namespace MysteryDungeon {
    public abstract class Tile {
        public enum Type {
            floor,
            wall
        }

        public HexCoord position { get; private set; }
        public Unit occupant;
        public int height;
        public abstract Type type { get; }




        public Tile(HexCoord pos) {
            position = pos;
        }

        public virtual void SetUnit(Unit u) {
            if (occupant != null) {
                throw new InvalidOperationException($"Tile at {position} is already occupied");
            }
            occupant = u;
        }
        public void RemoveUnit() {
            occupant = null;
        }

        public abstract bool CanUnitEnter(Unit u);






        public class Floor : Tile {
            public override Type type => Type.floor;

            public Floor(HexCoord pos) : base(pos) { }

            public override bool CanUnitEnter(Unit u) {
                return occupant == null;
            }
        }

        public class Wall : Tile {
            public override Type type => Type.wall;

            public Wall(HexCoord pos) : base(pos) { }

            public override bool CanUnitEnter(Unit u) {
                return false;
            }
        }
    }

}