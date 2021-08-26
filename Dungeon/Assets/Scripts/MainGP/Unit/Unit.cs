using UnityEngine;
using System.Collections.Generic;
using LPE.SpacePartition;



namespace MainGP {
    public abstract class Unit{
        public UnitInstanceData instanceData;
        protected WorldData world;


        public Unit(WorldData world, float radius, int hp, int team) {
            this.world = world;
            instanceData = new UnitInstanceData();
            instanceData.shape = new CircleShape(radius);
            instanceData.team = team;
            instanceData.health.max = hp;
            instanceData.health.current = hp;
        }

        public abstract void Update();

    }
    public class UnitInstanceData {

        public CircleShape shape;
        public Health health;

        public int team;


        public struct Health {
            public int max;
            public int current;
        }


    }

}
