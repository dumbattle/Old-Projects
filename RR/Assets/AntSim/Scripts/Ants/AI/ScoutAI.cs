using UnityEngine;

namespace AntSim {
    public class ScoutAI : AntAI {
        Scout ant;

        float poiDecay;
        int minFoodAmount;
        float exploreDist;

        Explore e;
        Communicate c;
        Idle i;

        public ScoutAI (Scout ant) : base(ant) {
            this.ant = ant;

            poiDecay = ant.colony.stats.scoutPoiDecay;
            minFoodAmount = ant.colony.stats.scoutFoodThresh;
            exploreDist = ant.colony.stats.scoutExplore;
            e = new Explore(this);
            c = new Communicate(this);
            i = new Idle(this);

            current = i;
        }


        public override AntAction GetDefaultAction() {
            return i;
        }

        public abstract class ScoutAction : AntAction {
            public Scout ant;

            protected ScoutAI ai;

            public ScoutAction(ScoutAI ai, float p) : base(p) {
                this.ai = ai;
                ant = ai.ant;
            }

        }

        public class Explore : ScoutAction {
            HexCoord dest;

            public Explore(ScoutAI ai) : base(ai, Priority.None) { }

            public Explore Reset() {
                dest = ant.colony.RandTile((int)(ant.colony.range * ai.exploreDist));
                return this;
            }

            public override AntAction Update() {
                Vector3 dir = (Vector3)dest.ToCartesian(AntSim.orientation, AntSim.Main.scale) - ant.transform.position;

                //arrive at destination
                if (dir.sqrMagnitude <= Ant.DIST_THRESH) {
                    Reset();
                }

                //look for food
                var f = LookForFood();
                if (f.coord != null) {
                    var poi = f.coord.Value;
                    return ai.c.Reset(poi, f.amnt);
                }

                ant.Move(dir);
                return this;
            }

            (HexCoord? coord, float amnt) LookForFood() {
                //find closest food
                foreach (var h in new HexGrid(ant.sight, ant.hex)) { // look at each grid
                    if (!new HexGrid(AntSim.Main.mapRadius).IsInRange(h)) { // in map range?
                        continue;
                    }
                    var f = FoodField.Main[h];
                    if (f == null) { // has food?
                        continue;
                    }
                    if (f.amount < ai.minFoodAmount) {
                        continue;
                    }

                    return (h, f.amount);
                }

                return (null, 0);
            }
        }

        public class Communicate : ScoutAction {
            HexCoord dest, poi;
            float poiTimer;

            public Communicate(ScoutAI ai) : base(ai, Priority.Medium) { }


            public Communicate Reset(HexCoord poi, float timer) {

                dest = ant.colony.RandTile();
                this.poi = poi;
                poiTimer = timer;
                return this;
            }

            public override AntAction Update() {
                Vector3 dir = (Vector3)dest.ToCartesian(AntSim.orientation, AntSim.Main.scale) - ant.transform.position;

                //arrive at destination
                if (dir.sqrMagnitude <= Ant.DIST_THRESH) {
                    dest = ant.colony.RandTile(5);
                }

                //search for harvesters
                poiTimer -= ai.poiDecay * Time.deltaTime;
                foreach (var h in new HexGrid(ant.sight, ant.hex)) {
                    var l = AntSim.antMap.GetAnts(h);

                    if (l == null) {
                        continue;
                    }
                    foreach (var a in l) {
                        var har = a as Harvester;
                        if (har == null) {
                            continue;
                        }

                        if (har.IssueHarvestOrder(poi, Priority.Low)) {
                            poiTimer -= har.carryCapacity;
                            if (poiTimer <= 0) {
                                return ai.i;

                            }
                        }
                    }
                }

                if (poiTimer <= 0) {
                    return ai.i;
                }

                ant.Move(dir);
                return this;
            }
        }

        public class Idle : ScoutAction {
            public Idle(ScoutAI ai) : base(ai, Priority.None) { }

            public override AntAction Update() {
                return ai.e.Reset();
            }
        }
    }
}