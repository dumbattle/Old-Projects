using UnityEngine;
using System;

namespace AntSim {

    public class HarvesterAI : AntAI {
        Harvester ant;
        Explore e;
        MoveToHarvest mth;
        ReturnHarvest rh;
        Harvest h;
        Idle i;

        public HarvesterAI(Harvester ant) : base(ant){
            this.ant = ant;
            e = new Explore(this);
            mth = new MoveToHarvest(this);
            rh = new ReturnHarvest(this);
            h = new Harvest(this);
            i = new Idle(this);

            current = e.Reset();
            hungerAction = new HarvesterHunger(this);
        }

        public bool IssueHarvestOrder(HexCoord h, float p) {
            if (p > current.priority) {
                current = mth.Reset(h, p);
                return true;
            }
            return false;
        }

        
        public override AntAction GetDefaultAction() {
           return i;
        }

        abstract class HarvesterAction : AntAction {
            protected HarvesterAI ai;
            public Harvester ant => ai.ant;
            protected HarvesterAction(HarvesterAI ai, float p) : base(p) {
                this.ai = ai;
            }

        }

        class Explore : HarvesterAction {
            HexCoord dest;
            public Explore(HarvesterAI ai) : base(ai, Priority.None) {
                dest = ant.colony.RandTile();
            }

            public Explore Reset() {
                dest = ant.colony.RandTile();
                return this;
            }
            public override AntAction Update() {
                Vector3 dir = (Vector3)dest.ToCartesian(AntSim.orientation, AntSim.Main.scale) - ant.transform.position;

                //arrive at destination
                if (dir.sqrMagnitude <= Ant.DIST_THRESH) {
                    dest = ant.colony.RandTile();

                    return this;
                }

                var food = LookForClosestFood(ant);
                if (food != null) {
                    return ai.mth.Reset(food.Value);
                }
                ant.Move(dir);

                return this;
            }
        }

        class MoveToHarvest : HarvesterAction {

            HexCoord food;

            public MoveToHarvest(HarvesterAI ai) : base(ai, Priority.Medium) { }

            public MoveToHarvest Reset(HexCoord f, float p = Priority.Medium) {
                food = f;
                priority = p;
                return this;
            }

            public override AntAction Update() {
                Vector3 dir = (Vector3)food.ToCartesian(AntSim.orientation, AntSim.Main.scale) - ant.transform.position;

                //arrive at destination
                if (dir.sqrMagnitude <= Ant.DIST_THRESH) {
                    return ai.h;
                }

                //food disappeared?
                if (ant.hex.ManhattanDist(food) <= ant.sight) {
                    var f = FoodField.Main[food];

                    if (f == null) {
                        return ai.i;
                    }
                }

                ant.Move(dir);
                return this;
            }
        }

        class ReturnHarvest : HarvesterAction {
            HexCoord home;
            public ReturnHarvest(HarvesterAI ai) : base(ai, Priority.Medium) {
                home = ant.homeTile;
            }

            public override AntAction Update() {
                Vector3 dir = (Vector3)home.ToCartesian(AntSim.orientation, AntSim.Main.scale) - ant.transform.position;

                //arrive at destination
                if (dir.sqrMagnitude <= Ant.DIST_THRESH) {
                    ant.colony.food += ant.currentCarrying;
                    ant.currentCarrying = 0;
                    return ai.i;
                }

                ant.Move(dir);
                return this;
            }
        }

        class Harvest : HarvesterAction {
            public Harvest(HarvesterAI ai) : base(ai, Priority.Medium) { }

            public override AntAction Update() {
                //verify at food
                if ((AntSim.Main.ToCartesion(ant.hex) - ant.transform.position).sqrMagnitude > Ant.DIST_THRESH) {
                    return ai.i;
                }

                //verify food exists
                var f = FoodField.Main[ant.hex];
                if (f == null) {
                    return ai.i;

                }

                //harvest
                ant.currentCarrying = f.Harvest(ant.carryCapacity);
                return ai.rh;
            }
        }

        class Idle : HarvesterAction {
            public Idle(HarvesterAI ai) : base(ai, Priority.None) { }

            public override AntAction Update() {
                if (ant.currentCarrying > 0) {
                    return ai.rh;
                }
                return ai.e;
            }
        }

        class HarvesterHunger : HungerAction {
            HarvesterAI ai;
            Harvester ant;
            public HarvesterHunger(HarvesterAI ai) : base(ai.ant) {
                this.ai = ai;
                ant = ai.ant;
            }

            public override AntAction Update() {
                //if carrying food eat that
                if (ant.currentCarrying > 0) {
                    int amount = Mathf.Min(ant.currentCarrying, (int)(ant.maxStanima - ant.stanima));

                    ant.currentCarrying -= amount;
                    ant.stanima += amount;

                   
                    return null;
                }
                return base.Update();
            }
        }
    }

    public class CensusTakerAI : AntAI {
        Type[] antTypes;
        CensusTaker ant;

        Wander w;
        Idle i;

        public CensusTakerAI(CensusTaker ant, params Type[] types) :base(ant) {
            foreach (var t in types) {
                if (!t.IsSubclassOf(typeof(Ant))) {
                    throw new ArgumentException("CensusTakerAI wrong type");
                }
            }
            this.ant = ant;
            antTypes = types;

            i = new Idle(this);
            w = new Wander(this);
            current = i;
        }
        public override AntAction GetDefaultAction() {
            return i;
        }


        abstract class CensusTakerAction : AntAction {
            protected CensusTakerAI ai;
            public CensusTaker ant => ai.ant;

            protected CensusTakerAction(CensusTakerAI ai, float p) : base(p) {
                this.ai = ai;
            }

        }


        class Wander : CensusTakerAction {
            HexCoord dest;
            public Wander(CensusTakerAI ai) : base(ai, Priority.None) {
                dest = ant.colony.RandTile();
            }

            public Wander Reset() {
                dest = ant.colony.RandTile();
                return this;
            }
            public override AntAction Update() {
                Vector3 dir = (Vector3)dest.ToCartesian(AntSim.orientation, AntSim.Main.scale) - ant.transform.position;

                //arrive at destination
                if (dir.sqrMagnitude <= Ant.DIST_THRESH) {
                    Reset();
                    return this;
                }


                ant.Move(dir);
                return this;
            }
        }

        class Idle : CensusTakerAction {
            public Idle(CensusTakerAI ai) : base(ai, Priority.None) { }

            public override AntAction Update() {
                return ai.w.Reset();
            }
        }
    }
}