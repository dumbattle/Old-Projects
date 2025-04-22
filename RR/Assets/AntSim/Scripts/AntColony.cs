using System.Collections.Generic;
using UnityEngine;

namespace AntSim {
    public class AntColony {
        public HexCoord pos;
        public AntStats stats;
        public int food = 0;
        public float range;


        LinkedList<Ant> ants;
        public int population => ants.Count;
        public bool alive => population > 0;

        GameObject baseObj;



        public AntColony(HexCoord pos) {
            range = 10;
            this.pos = pos;
            stats = new AntStats();
            //stats.Mutate();
            baseObj = Object.Instantiate(ObjHolder.main.baseObj);
            baseObj.SetActive(false);
        }

        public void Initialize() {
            //setup base object
            baseObj.transform.position = AntSim.Main.ToCartesion(pos);
            baseObj.SetActive(true);

            //spawn initial population
            ants = new LinkedList<Ant>();
            for (int i = 0; i < AntSim.Main.numAnts; i++) {
                SpawnHarvester();
            }
            for (int i = 0; i < AntSim.Main.numScouts; i++) {
                SpawnScout();
            }
            SpawnCensusTaker();
        }


        public void Update() {
            int hCount = 0;
            int sCount = 0;
            var ant = ants.First;
            while (ant != null) {
                ant.Value.Update();

                if (!ant.Value.Active) {
                    var next = ant.Next;
                    ants.Remove(ant);
                    ant = next;
                }
                else {
                    ant = ant.Next;
                    if (ant != null && ant.Value as Harvester == null) {
                        sCount++;
                    }
                    else {
                        hCount++;
                    }
                }
            }

            var cost = stats.harvester.stanima *5 /*+ stats.scout.maxStanima*/;
            if (food > cost * 2) {
                SpawnHarvester();
                SpawnHarvester();
                SpawnHarvester();
                SpawnHarvester();
                SpawnScout();
                range += .2f;
                food -= (int)(cost);
            }

            if (population == 0) {
                Die();
            }
        }
        void Die() {
            baseObj.SetActive(false);
        }

        public void Kill() {
            foreach (var a in ants) {
                a.Die();
            }
            ants.Clear();
            Die();
        }

        void SpawnHarvester() {
            var a = Harvester.Get();
            a.Initialize(this);
            ants.AddLast(a);
        }

        void SpawnScout() {
            var s = Scout.Get();
            s.Initialize(this);
            ants.AddLast(s);
        }

        void SpawnCensusTaker() {
            var c = CensusTaker.Get();
            c.Initialize(this);
            ants.AddLast(c);
        }

        public HexCoord RandTile(int maxDist) {
            var h = new HexGrid(maxDist, pos).RandomCoord();
            if (AntSim.Main.grid.IsInRange(h)) {
                return h;
            }
            return RandTile(maxDist);
        }

        public HexCoord RandTile() {
            var h = new HexGrid((int)range, pos).RandomCoord();
            if (AntSim.Main.grid.IsInRange(h)) {
                return h;
            }
            return RandTile();
        }
    }
}
